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
    public class ApexTab : ApexSerializedField
    {
        public const float HeaderHeigth = 20;
        public const float BorderWidth = 1.0f;

        public readonly static Padding GroupPadding = new Padding(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 4.0f);
        public readonly static Padding ChildrenPadding = new Padding(0.0f, 0.0f, 4.0f, 3.0f, 0.0f, 4.0f, 1.0f);

        public const float TitleLeftPadding = 3.0f;

        public const float ChildFoldoutLeftPadding = 18.0f;
        public const float ChildFoldoutRightPadding = 7.0f;
        public const float ChildContainerLeftPadding = 3.0f;

        protected string name;
        public Dictionary<string, List<ApexSerializedField>> sections;
        private int selectedIndex;

        public ApexTab(SerializedProperty source, string name) : base(source)
        {
            this.name = name;
            this.sections = new Dictionary<string, List<ApexSerializedField>>();
        }

        public override void DrawField(Rect position)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            position = GroupPadding.PaddingRect(position);

            string[] tabLabels = new string[sections.Count];
            int index = 0;
            foreach (var item in sections)
            {
                tabLabels[index] = item.Key;
                index++;
            }

            Rect headerPosition = new Rect(position.x + 1, position.y, position.width - 1, HeaderHeigth);
            selectedIndex = GUI.Toolbar(headerPosition, selectedIndex, tabLabels, EditorStyles.toolbarButton);
            headerPosition.x -= 1;
            headerPosition.width += 1;

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
            Rect childFoldoutPosition = new Rect(position.x + ChildFoldoutLeftPadding, childPosition.y, position.width - ChildFoldoutRightPadding - ChildFoldoutLeftPadding, childPosition.height);

            index = 0;
            foreach (var item in sections)
            {
                if(selectedIndex == index)
                {
                    List<ApexSerializedField> children = item.Value;
                    for (int i = 0; i < children.Count; i++)
                    {
                        ApexSerializedField child = children[i];

                        if (child.IsVisible())
                        {
                            Rect childTargetPosition = childPosition;

                            ApexFoldout foldout = child as ApexFoldout;
                            if (foldout != null)
                            {
                                childTargetPosition = childFoldoutPosition;
                                childTargetPosition.height = foldout.GetFieldHeight();
                            }
                            else if (child.TargetSerializedProperty.hasVisibleChildren)
                            {
                                childTargetPosition = childFoldoutPosition;
                                childTargetPosition.height = child.GetFieldHeight();
                            }
                            else
                            {
                                childTargetPosition.height = child.GetFieldHeight();
                            }

                            child.DrawField(childTargetPosition);
                            childPosition.y += childTargetPosition.height + EditorGUIUtility.standardVerticalSpacing;
                            childFoldoutPosition.y = childPosition.y;
                        }
                    }
                }
                index++;
            }
        }


        public override void DrawFieldLayout()
        {
            Rect position = GUILayoutUtility.GetRect(0, GetFieldHeight());
            DrawField(position);
        }

        public override float GetFieldHeight()
        {
            GroupPadding.CalculateHeightDifference(out float groupHeigthDifference);
            ChildrenPadding.CalculateHeightDifference(out float childrenHeightDifference);
            float hegiht = HeaderHeigth + groupHeigthDifference + childrenHeightDifference;

            int index = 0;
            foreach (var item in sections)
            {
                if (selectedIndex == index)
                {
                    List<ApexSerializedField> children = item.Value;
                    for (int i = 0; i < children.Count; i++)
                    {
                        ApexField child = children[i];
                        if (child.IsVisible())
                        {
                            hegiht += children[i].GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                }
                index++;
            }
            return hegiht;
        }

        public static void Wrap(ref List<ApexSerializedField> properties)
        {
            List<ApexSerializedField> updateList = new List<ApexSerializedField>();
            for (int i = 0; i < properties.Count; i++)
            {
                ApexSerializedField property = properties[i];
                TabGroupAttribute tabAttribute = ApexReflection.GetAttribute<TabGroupAttribute>(property.TargetSerializedProperty);
                if(tabAttribute != null)
                {
                    bool hasSameTab = false;
                    for (int j = 0; j < updateList.Count; j++)
                    {
                        ApexTab tempTab = updateList[j] as ApexTab;
                        if(tempTab != null)
                        {
                            if (tempTab.name == tabAttribute.name)
                            {
                                if (tempTab.sections.TryGetValue(tabAttribute.title, out List<ApexSerializedField> tabChilds))
                                {
                                    tabChilds.Add(property);
                                }
                                else
                                {
                                    tabChilds = new List<ApexSerializedField>();
                                    tabChilds.Add(property);
                                    tempTab.sections.Add(tabAttribute.title, tabChilds);
                                }
                                hasSameTab = true;
                                break;
                            }
                        }
                    }

                    if(!hasSameTab)
                    {
                        ApexTab apexTab = new ApexTab(property.TargetSerializedProperty, tabAttribute.name);
                        List<ApexSerializedField> tabChilds = new List<ApexSerializedField>();
                        tabChilds.Add(property);
                        apexTab.sections.Add(tabAttribute.title, tabChilds);
                        updateList.Add(apexTab);
                    }
                }
                else
                {
                    updateList.Add(property);
                }
            }

            properties = updateList;
        }



        #region [Getter / Setter]
        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }
        #endregion
    }
}