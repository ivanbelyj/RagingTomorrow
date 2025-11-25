/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public class ApexGroup : ApexLayout
    {
        public const float HeaderHeigth = 23;
        public const float BorderWidth = 1.0f;

        public readonly static Padding GroupPadding = new Padding(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 4.0f);
        public readonly static Padding ChildrenPadding = new Padding(0.0f, 1.0f, 4.0f, 3.0f, 0.0f, 4.0f);

        public const float TitleLeftPadding = 3.0f;

        public const float ChildSpace = 2.0f;
        public const float ChildFoldoutLeftPadding = 18.0f;
        public const float ChildFoldoutRightPadding = 7.0f;
        public const float ChildContainerLeftPadding = 3.0f;

        protected string groupName;

        public ApexGroup(SerializedProperty serializedProperty, string title, List<ApexSerializedField> children) : base(serializedProperty, children)
        {
            this.groupName = title;
        }

        public ApexGroup(SerializedProperty serializedProperty, string title, params ApexSerializedField[] children) : base(serializedProperty, children)
        {
            this.groupName = title;
        }

        public override void DrawField(Rect position)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            position = GroupPadding.PaddingRect(position);

            Rect headerPosition = new Rect(position.x, position.y, position.width, HeaderHeigth);
            EditorGUI.DrawRect(headerPosition, ApexSettings.HeaderColor);

            Rect titlePosition = new Rect(headerPosition.x + TitleLeftPadding, position.y + 3, position.width, singleLineHeight);
            GUI.Label(titlePosition, groupName);

            GroupPadding.CalculateHeightDifference(out float groupHeigthDifference);
            float verticalBroderHeight = position.height - groupHeigthDifference;

            Color borderColor = ApexSettings.GroupBorderColor;
            Rect topBorderPosition = new Rect(headerPosition.x, headerPosition.y, headerPosition.width, BorderWidth);
            EditorGUI.DrawRect(topBorderPosition, borderColor);

            Rect buttomBorderPosition = new Rect(headerPosition.x, headerPosition.y + headerPosition.height, headerPosition.width, BorderWidth);
            EditorGUI.DrawRect(buttomBorderPosition, borderColor);

            Rect leftBorderPosition = new Rect(headerPosition.x, headerPosition.y, BorderWidth, verticalBroderHeight);
            EditorGUI.DrawRect(leftBorderPosition, borderColor);

            Rect rightBorderPosition = new Rect(headerPosition.x + headerPosition.width, headerPosition.y, BorderWidth, verticalBroderHeight);
            EditorGUI.DrawRect(rightBorderPosition, borderColor);

            Rect dynamicButtomBorderPosition = new Rect(headerPosition.x, headerPosition.y + verticalBroderHeight, headerPosition.width, BorderWidth);
            EditorGUI.DrawRect(dynamicButtomBorderPosition, borderColor);

            Rect childPosition = new Rect(position.x, position.y + HeaderHeigth, position.width, singleLineHeight);
            childPosition = ChildrenPadding.PaddingRect(childPosition);
            Rect childFoldoutPosition = new Rect(position.x + ChildFoldoutLeftPadding, childPosition.y - 1, position.width - ChildFoldoutRightPadding - ChildFoldoutLeftPadding, singleLineHeight);
            for (int i = 0; i < children.Count; i++)
            {
                ApexSerializedField child = children[i];
                if (child.IsVisible())
                {
                    Rect currentChildPosition = childPosition;
                    currentChildPosition.height = child.GetFieldHeight();

                    ApexFoldout foldout = child as ApexFoldout;
                    if (foldout != null)
                    {
                        currentChildPosition = childFoldoutPosition;
                        currentChildPosition.height = foldout.GetFieldHeight();
                    }

                    child.DrawField(currentChildPosition);
                    childPosition.y += foldout?.GetFieldHeight() ?? child.GetFieldHeight();
                    childPosition.y += EditorGUIUtility.standardVerticalSpacing;
                    childFoldoutPosition.y = childPosition.y;
                }
            }
        }

        public override float GetFieldHeight()
        {
            GroupPadding.CalculateHeightDifference(out float groupHeigthDifference);
            ChildrenPadding.CalculateHeightDifference(out float childrenHeightDifference);
            float hegiht = HeaderHeigth + groupHeigthDifference + childrenHeightDifference;
            for (int i = 0; i < children.Count; i++)
            {
                ApexSerializedField child = children[i];
                if (child.IsVisible())
                {
                    hegiht += child.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return hegiht;
        }

        public override void Add(ApexSerializedField field)
        {
            if(field is ApexFoldout foldout)
            {
                foldout.IsInsideGroup(true);
                base.Add(foldout);
            }
            else
            {
                base.Add(field);
            }
        }

        /// <summary>
        /// Group Apex serialized field if any of field contains Apex group/foldout attribute.
        /// </summary>
        public static void Wrap(ref List<ApexSerializedField> apexSerializedFields)
        {
            List<ApexSerializedField> tempList = new List<ApexSerializedField>(apexSerializedFields.Count);
            for (int i = 0; i < apexSerializedFields.Count; i++)
            {
                ApexSerializedField apexSerializedField = apexSerializedFields[i];
                ApexFoldout apexFoldout = apexSerializedField as ApexFoldout;
                if (apexFoldout != null)
                {
                    for (int j = 0; j < apexFoldout.Count; j++)
                    {
                        ApexSerializedField foldoutChild = apexFoldout.GetChild(j);
                        GroupAttribute groupAttribute = ApexReflection.GetAttribute<GroupAttribute>(foldoutChild.TargetSerializedProperty);
                        if (groupAttribute != null)
                        {
                            bool complete = false;
                            for (int k = 0; k < tempList.Count; k++)
                            {
                                ApexSerializedField updatedApexField = tempList[k];
                                ApexGroup apexGroup = updatedApexField as ApexGroup;
                                if (apexGroup != null && apexGroup.GetGroupName() == groupAttribute.name)
                                {
                                    for (int t = 0; t < apexGroup.Count; t++)
                                    {
                                        ApexSerializedField groupField = apexGroup.GetChild(t);
                                        ApexFoldout groupFoldout = groupField as ApexFoldout;
                                        if (groupFoldout != null && groupFoldout.GetName() == apexFoldout.GetName())
                                        {
                                            groupFoldout.Add(foldoutChild);
                                            apexGroup.SetChild(t, groupField);
                                            tempList[k] = apexGroup;
                                            complete = true;
                                            break;
                                        }
                                    }

                                    if (!complete)
                                    {
                                        ApexFoldout nestedApexFoldout = new ApexFoldout(foldoutChild.TargetSerializedProperty, apexFoldout.GetName(), apexFoldout.GetStyle(), foldoutChild);
                                        apexGroup.Add(nestedApexFoldout);
                                        tempList[k] = apexGroup;
                                        complete = true;
                                    }
                                }

                                if (complete)
                                {
                                    break;
                                }
                            }

                            if (!complete)
                            {
                                ApexFoldout nestedApexFoldout = new ApexFoldout(foldoutChild.TargetSerializedProperty, apexFoldout.GetName(), apexFoldout.GetStyle(), foldoutChild);
                                ApexGroup apexGroup = new ApexGroup(apexFoldout.TargetSerializedProperty, groupAttribute.name, nestedApexFoldout);
                                tempList.Add(apexGroup);
                            }
                        }
                        else
                        {
                            bool hasFoldout = false;
                            for (int h = 0; h < tempList.Count; h++)
                            {
                                ApexSerializedField updatedApexField = tempList[h];
                                ApexFoldout updatedApexFoldout = updatedApexField as ApexFoldout;
                                if (updatedApexFoldout != null && updatedApexFoldout.GetName() == apexFoldout.GetName())
                                {
                                    updatedApexFoldout.Add(foldoutChild);
                                    tempList[h] = updatedApexFoldout;
                                    hasFoldout = true;
                                    break;
                                }
                            }

                            if (!hasFoldout)
                            {
                                ApexFoldout newApexFoldout = new ApexFoldout(foldoutChild.TargetSerializedProperty, apexFoldout.GetName(), apexFoldout.GetStyle(), foldoutChild);
                                tempList.Add(newApexFoldout);
                            }
                        }
                    }
                }
                else
                {
                    GroupAttribute groupAttribute = ApexReflection.GetAttribute<GroupAttribute>(apexSerializedField.TargetSerializedProperty);
                    if (groupAttribute != null)
                    {
                        ApexProperty apexProperty = apexSerializedField as ApexProperty;
                        if (apexProperty != null)
                        {
                            apexSerializedField = apexProperty;
                        }

                        bool complete = false;
                        for (int j = 0; j < tempList.Count; j++)
                        {
                            ApexSerializedField updatedApexField = tempList[j];
                            ApexGroup apexGroup = updatedApexField as ApexGroup;
                            if (apexGroup != null && apexGroup.GetGroupName() == groupAttribute.name)
                            {
                                apexGroup.Add(apexSerializedField);
                                tempList[j] = apexGroup;
                                complete = true;
                                break;
                            }
                        }

                        if (!complete)
                        {
                            ApexGroup apexGroup = new ApexGroup(apexSerializedField.TargetSerializedProperty, groupAttribute.name, apexSerializedField);
                            tempList.Add(apexGroup);
                        }
                    }
                    else
                    {
                        tempList.Add(apexSerializedField);
                    }
                }
            }
            apexSerializedFields = tempList;
        }

        #region [Getter / Setter]
        public string GetGroupName()
        {
            return groupName;
        }

        public void SetGroupName(string value)
        {
            groupName = value;
        }
        #endregion
    }
}