/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(SearchableEnumAttribute))]
    public sealed class SearchableEnumView : PropertyView, IPropertyValidatorReceiver
    {
        private SearchableEnumAttribute searchableEnumAttribute;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            searchableEnumAttribute = viewAttribute as SearchableEnumAttribute;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelPosition = EditorGUI.PrefixLabel(position, label);

            Rect popupPosition = new Rect(labelPosition.x, labelPosition.y, position.x + position.width - labelPosition.x, position.height);
            if (GUI.Button(popupPosition, property.enumDisplayNames[property.enumValueIndex], EditorStyles.popup))
            {
                GUI.changed = false;
                SearchableMenu searchableMenu = new SearchableMenu();
                for (int i = 0; i < property.enumDisplayNames.Length; i++)
                {

                    int indexCopy = i;
                    string enumName = property.enumDisplayNames[indexCopy];

                    if (searchableEnumAttribute.HideValues?.Any(v => v == enumName) ?? false)
                    {
                        continue;
                    }

                    bool isActive = !searchableEnumAttribute.DisableValues?.Any(v => v == enumName) ?? true;
                    searchableMenu.AddItem(new GUIContent(enumName), isActive, () =>
                    {
                        property.enumValueIndex = indexCopy;
                        property.serializedObject.ApplyModifiedProperties();
                    });
                }

                if (searchableEnumAttribute.Sort)
                {
                    searchableMenu.SortItems();
                }

                searchableMenu.ShowAsDropdown(popupPosition, new Vector2(popupPosition.width, searchableEnumAttribute.Height));
            }
        }

        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Enum;
        }
    }
}