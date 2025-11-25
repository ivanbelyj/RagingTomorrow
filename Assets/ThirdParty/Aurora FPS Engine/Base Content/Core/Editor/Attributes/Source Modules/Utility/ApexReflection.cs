/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public static class ApexReflection
    {
        public readonly static Dictionary<Type, PropertyView> Views;
        public readonly static Dictionary<Type, PropertyPainter> Decorators;
        public readonly static Dictionary<Type, PropertyValidator> Validators;
        public readonly static Dictionary<Type, PropertyDrawer> Drawers;

        static ApexReflection()
        {
            Views = new Dictionary<Type, PropertyView>();
            Decorators = new Dictionary<Type, PropertyPainter>();
            Validators = new Dictionary<Type, PropertyValidator>();
            Drawers = new Dictionary<Type, PropertyDrawer>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                ViewTarget viewTarget = GetAttribute<ViewTarget>(type);
                if (viewTarget != null)
                {
                    if (Activator.CreateInstance(type) is PropertyView propertyView)
                    {
                        Views.Add(viewTarget.target, propertyView);
                    }
                }

                PainterTarget decoratorTarget = GetAttribute<PainterTarget>(type);
                if (decoratorTarget != null)
                {
                    if (Activator.CreateInstance(type) is PropertyPainter propertyDecorator)
                    {
                        Decorators.Add(decoratorTarget.target, propertyDecorator);
                    }
                }

                ValidatorTarget validatorTarget = GetAttribute<ValidatorTarget>(type);
                if (validatorTarget != null)
                {
                    if (Activator.CreateInstance(type) is PropertyValidator propertyValidator)
                    {
                        Validators.Add(validatorTarget.target, propertyValidator);
                    }
                }

                DrawerTarget drawerTarget = GetAttribute<DrawerTarget>(type);
                if (drawerTarget != null)
                {
                    if (Activator.CreateInstance(type) is PropertyDrawer propertyDrawer)
                    {
                        Drawers.Add(drawerTarget.target, propertyDrawer);
                        if (drawerTarget.SubClasses && !drawerTarget.target.IsGenericType)
                        {
                            IEnumerable<Type> subclases = FindSubclassesOf(drawerTarget.target);
                            foreach (Type subClass in subclases)
                            {
                                if (!Drawers.ContainsKey(subClass))
                                {
                                    Drawers.Add(subClass, propertyDrawer);
                                }
                            }
                        }
                        else if (drawerTarget.SubClasses && drawerTarget.target.IsGenericType)
                        {
                            Assembly[] _assemblies = AppDomain.CurrentDomain.GetAssemblies();
                            for (int i = 0; i < _assemblies.Length; i++)
                            {
                                Assembly _assembly = _assemblies[i];
                                Type[] _types = _assembly.GetTypes();
                                for (int j = 0; j < _types.Length; j++)
                                {
                                    Type _type = _types[j];
                                    if (IsSubclassOfGeneric(drawerTarget.target, _type))
                                    {
                                        if (!Drawers.ContainsKey(_type))
                                        {
                                            Drawers.Add(_type, propertyDrawer);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool HasPropertyDrawer(SerializedProperty property)
        {
            Type propertyType = GetPropertyType(property);
            if (propertyType == null)
            {
                object propertyObject = GetObjectOfProperty(property);
                if (propertyObject != null)
                {
                    propertyType = propertyObject.GetType();
                }
            }

            if (propertyType != null && Drawers.ContainsKey(propertyType))
            {
                return true;
            }
            return false;
        }

        public static bool HasPropertyView(SerializedProperty property)
        {
            return GetAttribute<ViewAttribute>(property) != null;
        }

        public static IEnumerable<Type> FindSubclassesOf(Type type, bool directDescendants = false)
        {
            Assembly assembly = type.Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => directDescendants ? t.BaseType == type : t.IsSubclassOf(type));
            return subclasses;
        }

        public static IEnumerable<Type> FindSubclassesOf<T>(bool directDescendants = false)
        {
            Assembly assembly = typeof(T).Assembly;
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => directDescendants ? t.BaseType == typeof(T) : t.IsSubclassOf(typeof(T)));
            return subclasses;
        }

        public static IEnumerable<Type> FindSubclassesOf<T>(Assembly assembly, bool directDescendants = false)
        {
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => directDescendants ? t.BaseType == typeof(T) : t.IsSubclassOf(typeof(T)));
            return subclasses;
        }

        public static Type FindType(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    if (type.Name == name)
                    {
                        return type;
                    }
                }
            }
            return null;
        }

        public static Type[] GetAllSubTypes(Type target)
        {
            List<Type> result = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type[] types = assembly.GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type type = types[j];
                    if (type.IsSubclassOf(target))
                    {
                        result.Add(type);
                    }
                }
            }
            return result.ToArray();
        }

        public static bool IsSubclassOfGeneric(Type generic, Type target)
        {
            while (target != null && target != typeof(object))
            {
                var cur = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
                if (generic == cur)
                {
                    return true;
                }
                target = target.BaseType;
            }
            return false;
        }

        public static T GetAttribute<T>(Type target) where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(target, typeof(T));
        }

        public static T GetAttribute<T>(SerializedProperty property) where T : class
        {
            T[] attributes = GetAttributes<T>(property);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(Type target) where T : Attribute
        {
            return (T[])Attribute.GetCustomAttributes(target, typeof(T));
        }

        public static T[] GetAttributes<T>(SerializedProperty property) where T : class
        {
            FieldInfo fieldInfo = GetField(GetDeclaringObjectOfProperty(property), property.name);
            if (fieldInfo == null)
            {
                return new T[] { };
            }

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static Type GetPropertyType(SerializedProperty property)
        {
            object declaringObject = GetDeclaringObjectOfProperty(property);
            if(declaringObject != null)
            {
                Type type = declaringObject.GetType();
                do
                {
                    FieldInfo fieldInfo = type.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo != null)
                    {
                        return fieldInfo.FieldType;
                    }
                    type = type.BaseType;
                } while (type != null);
            }

            return null;

        }

        /// <summary>
        /// Return parent property of target property.
        /// Otherwise return target itself.
        /// </summary>
        public static SerializedProperty GetParentProperty(SerializedProperty property)
        {
            string[] paths = property.propertyPath.Split('.');
            if(paths != null && paths.Length > 1)
            {
                Array.Resize<string>(ref paths, paths.Length - 1);
                string path = string.Join(".", paths);
                return property.serializedObject.FindProperty(path);
            }
            return property;
        }

        /// <summary>
        /// Return parent property of target property.
        /// Otherwise return target itself.
        /// </summary>
        public static SerializedProperty FindPropertyRelativeParent(this SerializedProperty property, string propertyName)
        {
            string[] paths = property.propertyPath.Split('.');
            if (paths != null && paths.Length > 1)
            {
                Array.Resize<string>(ref paths, paths.Length - 1);
                string path = string.Join(".", paths);
                return property.serializedObject.FindProperty(path).FindPropertyRelative(propertyName);
            }
            return property.serializedObject.FindProperty(propertyName);
        }

        public static Type GetListElementType(Type listType)
        {
            if (listType.IsGenericType)
            {
                return listType.GetGenericArguments()[0];
            }
            else
            {
                return listType.GetElementType();
            }
        }

        public static Type GetManagedReferenceType(SerializedProperty property)
        {
            string[] baseTypeAndAssemblyName = property.managedReferenceFieldTypename.Split(' ');
            string baseTypeString = string.Format("{0}, {1}", baseTypeAndAssemblyName[1], baseTypeAndAssemblyName[0]);
            return Type.GetType(baseTypeString);
        }

        public static bool TryGetDisplayContent(SerializedProperty property, out GUIContent name)
        {
            LabelAttribute displayName = GetAttribute<LabelAttribute>(property);
            if (displayName != null)
            {
                name = new GUIContent(displayName.name);
                TooltipAttribute tooltip = GetAttribute<TooltipAttribute>(property);
                if(tooltip != null)
                {
                    name.tooltip = tooltip.tooltip;
                }
                return true;
            }
            name = null;
            return false;
        }

        public static List<SerializedProperty> CopyVisibleChildren(this SerializedProperty serializedProperty)
        {
            List<SerializedProperty> copyProperties = new List<SerializedProperty>();
            foreach (SerializedProperty child in serializedProperty.GetVisibleChildren())
            {
                copyProperties.Add(child.Copy());
            }
            return copyProperties;
        }

        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.Next(false));
            }
        }

        public static IEnumerable<SerializedProperty> GetVisibleChildren(this SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.NextVisible(false);
            }

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }

        public static object GetObjectOfProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }

        public static object GetDeclaringObjectOfProperty(SerializedProperty property)
        {
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        public static FieldInfo GetFieldInfoOfProperty(SerializedProperty property)
        {
            FieldInfo GetField(Type type, string path)
            {
                return type.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }

            var parentType = property.serializedObject.targetObject.GetType();
            var splits = property.propertyPath.Split('.');
            var fieldInfo = GetField(parentType, splits[0]);
            for (var i = 1; i < splits.Length; i++)
            {
                if (splits[i] == "Array")
                {
                    i += 2;
                    if (i >= splits.Length)
                        continue;

                    var type = fieldInfo.FieldType.IsArray
                        ? fieldInfo.FieldType.GetElementType()
                        : fieldInfo.FieldType.GetGenericArguments()[0];

                    fieldInfo = GetField(type, splits[i]);
                }
                else
                {
                    fieldInfo = i + 1 < splits.Length && splits[i + 1] == "Array"
                        ? GetField(parentType, splits[i])
                        : GetField(fieldInfo.FieldType, splits[i]);
                }

                if (fieldInfo == null)
                    throw new Exception("Invalid FieldInfo. " + property.propertyPath);

                parentType = fieldInfo.FieldType;
            }

            return fieldInfo;
        }

        public static bool TryDeepFindMethods(Type target, string name, out MethodInfo[] methods)
        {
            Type type = target;
            List<MethodInfo> matchMethods = new List<MethodInfo>();
            do
            {
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                if (methodInfos != null)
                {
                    for (int i = 0; i < methodInfos.Length; i++)
                    {
                        MethodInfo methodInfo = methodInfos[i];
                        if(methodInfo.Name == name)
                        {
                            matchMethods.Add(methodInfo);
                        }
                    }
                }
                type = type.BaseType;
            } while (type != null);

            methods = matchMethods.ToArray();
            return methods != null;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }

        private static FieldInfo GetFieldInfo(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetFieldInfo(object source, string name, int index)
        {
            IEnumerable enumerable = GetFieldInfo(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }

        public static IEnumerable<SerializedProperty> GetFirstLevelChildren(SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }

        public static IEnumerable<FieldInfo> GetAllFields(object target, Func<FieldInfo, bool> predicate)
        {
            if (target == null)
            {
                yield break;
            }

            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<FieldInfo> fieldInfos = types[i]
                    .GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var fieldInfo in fieldInfos)
                {
                    yield return fieldInfo;
                }
            }
        }

        public static IEnumerable<PropertyInfo> GetAllProperties(object target, Func<PropertyInfo, bool> predicate)
        {
            if (target == null)
            {
                Debug.LogError("The target object is null. Check for missing scripts.");
                yield break;
            }

            List<Type> types = new List<Type>()
            {
                target.GetType()
            };

            while (types.Last().BaseType != null)
            {
                types.Add(types.Last().BaseType);
            }

            for (int i = types.Count - 1; i >= 0; i--)
            {
                IEnumerable<PropertyInfo> propertyInfos = types[i]
                    .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(predicate);

                foreach (var propertyInfo in propertyInfos)
                {
                    yield return propertyInfo;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetAllMethods(object target, Func<MethodInfo, bool> predicate)
        {
            if (target == null)
            {
                return null;
            }

            IEnumerable<MethodInfo> methodInfos = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(predicate);

            return methodInfos;
        }

        public static FieldInfo GetField(object target, string fieldName)
        {
            return GetAllFields(target, f => f.Name.Equals(fieldName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            return GetAllProperties(target, p => p.Name.Equals(propertyName, StringComparison.InvariantCulture)).FirstOrDefault();
        }

        public static MethodInfo GetMethod(object target, string methodName)
        {
            return GetAllMethods(target, m => m.Name.Equals(methodName, StringComparison.InvariantCulture)).FirstOrDefault();
        }
    }
}