/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using AuroraFPSRuntime.WeaponModules;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.Utilities
{
    public sealed class WeaponCreationWizard : EditorWindow
    {
        private static readonly string UxmlRelativePath = "Base Content/Core/Editor/Editor Window/Weapon Creation Wizard/WeaponCreationWizardEditor.uxml";
        private static readonly string UssRelativePath = "Base Content/Core/Editor/Editor Window/Weapon Creation Wizard/WeaponCreationWizardStyle.uss";

        #region [First Person Weapon Properties]
        private IMGUIContainer weaponTemplatesGUI;
        private IMGUIContainer weaponWnformationGUI;
        private IMGUIContainer shootingSystemGUI;
        private IMGUIContainer reloadSystemGUI;
        private IMGUIContainer weaponModelGUI;
        private IMGUIContainer weaponLocationGUI;

        private List<string> weaponTemplatesPath;
        private int selectedWeaponTemplateIndex = -1;
        private GameObject selectedWeaponTemplate = null;

        private string weaponName = "Weapon Name";
        private string weaponType = "Group Name";
        private Sprite weaponImage;
        private float weaponSelectTime = 0.3f;
        private float weaponHideTime = 0.3f;

        private WeaponShootingSystem weaponShootingSystem;
        private Editor weaponShootingSystemEditor;

        private WeaponReloadSystem weaponReloadSystem;
        private Editor weaponReloadSystemEditor;

        private GameObject weaponModel;
        private Editor transformEditor;

        private List<Transform> weaponBodyList;
        private ReorderableList weaponBodyReorderableList;
        private List<Transform> weaponMagazineList;
        private ReorderableList weaponMagazineReorderableList;
        private List<Transform> weaponTriggerList;
        private ReorderableList weaponTriggerReorderableList;

        private string weaponSaveLocation = "Assets/";
        #endregion

        #region [Loot Weapon Properties]
        private IMGUIContainer lootWeaponTemplatesGUI;
        private IMGUIContainer lootWeaponInformationGUI;
        private IMGUIContainer lootWeaponLocationGUI;

        private List<string> lootWeaponTemplatesPath;
        private int selectedLootWeaponTemplateIndex = -1;
        private GameObject selectedLootWeaponTemplate = null;

        private EquippableItem lootWeaponEquippableItem;
        private GameObject lootWeaponModel;

        private string lootWeaponSaveLocation = "Assets/";
        #endregion

        #region [Loot Ammo Properties]
        private IMGUIContainer lootAmmoTemplatesGUI;
        private IMGUIContainer lootAmmoInformationGUI;
        private IMGUIContainer lootAmmoLocationGUI;

        private List<string> lootAmmoTemplatesPath;
        private int selectedLootAmmoTemplateIndex = -1;
        private GameObject selectedLootAmmoTemplate = null;

        private EquippableItem lootAmmoEquippableItem;
        private GameObject lootAmmoModel;
        private int ammoCount;

        private string lootAmmoSaveLocation = "Assets/";
        #endregion

        private int tabIndex;

        [MenuItem("Aurora FPS Engine/Utilities/Weapon Creation Wizard", false, 309)]
        public static void Open()
        {
            ApexSettings settings = ApexSettings.Current;
            string uxmlPath = Path.Combine(settings.GetRootPath(), UxmlRelativePath);
            string ussPath = Path.Combine(settings.GetRootPath(), UssRelativePath);
            if (!File.Exists(uxmlPath) || !File.Exists(ussPath))
            {
                Debug.LogAssertion($"<b><color=#FF0000>Missing UXML and USS files!</color></b>\n<i><color=#FF0000>UXML required: {uxmlPath}.</color></i>\n<i><color=#FF0000>USS required: {ussPath}.</color></i>");
                return;
            }

            WeaponCreationWizard window = GetWindow<WeaponCreationWizard>();
            window.titleContent = new GUIContent("Weapon Creation Wizard");
            window.minSize = new Vector2(325, 325);
            window.Show();
        }

        private void CreateGUI()
        {
            InitializeUXML();
            InitializeTabs();
            InitializeTemplates();

            CreateFirstPersonWeaponGUI();
            CreateLootWeaponGUI();
            CreateLootAmmoGUI();
        }

        private void OnFocus()
        {
            UpdateWeaponModelReorderableLists();
        }

        private void OnDestroy()
        {
            if (selectedWeaponTemplate != null)
            {
                DestroyImmediate(selectedWeaponTemplate);
            }

            if (selectedLootWeaponTemplate != null)
            {
                DestroyImmediate(selectedLootWeaponTemplate);
            }

            if (selectedLootAmmoTemplate != null)
            {
                DestroyImmediate(selectedLootAmmoTemplate);
            }
        }

        private bool IsChildOf(Transform parent, Transform child)
        {
            Transform sub = child.parent;

            if (sub == parent)
            {
                return true;
            }
            else if (sub == null)
            {
                return false;
            }

            return IsChildOf(parent, sub);
        }

        #region [Initialize Methods]
        private void InitializeUXML()
        {
            ApexSettings settings = ApexSettings.Current;
            string uxmlPath = Path.Combine(settings.GetRootPath(), UxmlRelativePath);
            string ussPath = Path.Combine(settings.GetRootPath(), UssRelativePath);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void InitializeTabs()
        {
            // Register tab callbacks.
            List<Button> tabButtons = rootVisualElement.Query<Button>(classes: "tab-button").ToList();
            List<VisualElement> contentPages = rootVisualElement.Query<VisualElement>(classes: "content-page").ToList();
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int index = i;
                tabButtons[i].clicked += () =>
                {
                    tabButtons[tabIndex].RemoveFromClassList("active");
                    contentPages[tabIndex].RemoveFromClassList("active");
                    tabIndex = index;
                    tabButtons[tabIndex].AddToClassList("active");
                    contentPages[tabIndex].AddToClassList("active");
                };
            }

            // Show last opened page.
            tabButtons[tabIndex].AddToClassList("active");
            contentPages[tabIndex].AddToClassList("active");
        }

        private void InitializeTemplates()
        {
            {
                weaponTemplatesPath = new List<string>();
                string path = Path.Combine(ApexSettings.Current.GetRootPath(), "Base Content/Core/Editor/Editor Resources/Templates/Weapon/First Person Weapon");
                if (Directory.Exists(path))
                {
                    string[] assetPaths = Directory.GetFiles(path, "*.prefab");
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPaths[i]);
                        if (asset.GetComponent<EquippableObjectIdentifier>() != null)
                        {
                            weaponTemplatesPath.Add(assetPaths[i]);
                        }
                    }
                }
            }

            {
                lootWeaponTemplatesPath = new List<string>();
                string path = Path.Combine(ApexSettings.Current.GetRootPath(), "Base Content/Core/Editor/Editor Resources/Templates/Weapon/Loot Weapon");
                if (Directory.Exists(path))
                {
                    string[] assetPaths = Directory.GetFiles(path, "*.prefab");
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPaths[i]);
                        if (asset.GetComponent<LootWeapon>() != null)
                        {
                            lootWeaponTemplatesPath.Add(assetPaths[i]);
                        }
                    }
                }
            }

            {
                lootAmmoTemplatesPath = new List<string>();
                string path = Path.Combine(ApexSettings.Current.GetRootPath(), "Base Content/Core/Editor/Editor Resources/Templates/Weapon/Loot Ammo");
                if (Directory.Exists(path))
                {
                    string[] assetPaths = Directory.GetFiles(path, "*.prefab");
                    for (int i = 0; i < assetPaths.Length; i++)
                    {
                        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPaths[i]);
                        if (asset.GetComponent<LootAmmo>() != null)
                        {
                            lootAmmoTemplatesPath.Add(assetPaths[i]);
                        }
                    }
                }
                
            }
        }
        #endregion

        #region [First Person Weapon Methods]
        private void CreateFirstPersonWeaponGUI()
        {
            VisualElement root = rootVisualElement;

            weaponTemplatesGUI = root.Q<IMGUIContainer>("weapon-templates-gui");
            weaponTemplatesGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;

                if (selectedWeaponTemplate == null)
                {
                    selectedWeaponTemplateIndex = -1;
                }

                string buttonName = selectedWeaponTemplateIndex < 0 ? "Select template..." : Path.GetFileNameWithoutExtension(weaponTemplatesPath[selectedWeaponTemplateIndex]);
                Rect buttonPosition = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                buttonPosition = EditorGUI.PrefixLabel(buttonPosition, new GUIContent("Template"));
                if (GUI.Button(buttonPosition, buttonName, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < weaponTemplatesPath.Count; i++)
                    {
                        menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(weaponTemplatesPath[i])), false, SelectWeaponTemplate, i);
                    }
                    menu.DropDown(buttonPosition);
                }
            };

            weaponWnformationGUI = root.Q<IMGUIContainer>("weapon-information-gui");
            weaponWnformationGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;
                if (selectedWeaponTemplate == null) return;

                weaponName = EditorGUILayout.TextField("Name", weaponName);
                weaponType = EditorGUILayout.TextField("Type", weaponType);
                weaponImage = (Sprite)EditorGUILayout.ObjectField("Item Image", weaponImage, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                weaponSelectTime = EditorGUILayout.FloatField("Select Time", weaponSelectTime);
                weaponHideTime = EditorGUILayout.FloatField("Hide Time", weaponHideTime);
            };

            shootingSystemGUI = root.Q<IMGUIContainer>("shooting-system-gui");
            shootingSystemGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;
                if (selectedWeaponTemplate == null) return;

                if (weaponShootingSystemEditor != null)
                {
                    weaponShootingSystemEditor.OnInspectorGUI();
                }
            };

            reloadSystemGUI = root.Q<IMGUIContainer>("reload-system-gui");
            reloadSystemGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;
                if (selectedWeaponTemplate == null) return;

                if (weaponReloadSystemEditor != null)
                {
                    weaponReloadSystemEditor.OnInspectorGUI();
                }
            };

            weaponModelGUI = root.Q<IMGUIContainer>("model-gui");
            weaponModelGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;
                if (selectedWeaponTemplate == null) return;

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    GameObject model = (GameObject)EditorGUILayout.ObjectField("Model", weaponModel, typeof(GameObject), false);

                    if (check.changed)
                    {
                        if (weaponModel != null)
                        {
                            DestroyImmediate(weaponModel);
                        }

                        if (model != null)
                        {
                            WeaponTemplatePivots pivots = selectedWeaponTemplate.GetComponent<WeaponTemplatePivots>();
                            weaponModel = Instantiate(model, pivots.GetCenterPivot().position, Quaternion.identity, selectedWeaponTemplate.transform);

                            if (transformEditor != null)
                            {
                                DestroyImmediate(transformEditor);
                            }
                            transformEditor = Editor.CreateEditor(weaponModel.transform);

                            UpdateWeaponModelReorderableLists();
                        }
                    }
                }


                if (weaponModel != null)
                {
                    transformEditor.OnInspectorGUI();

                    if (weaponBodyReorderableList != null)
                    {
                        weaponBodyReorderableList.DoLayoutList();
                    }

                    if (weaponMagazineReorderableList != null)
                    {
                        weaponMagazineReorderableList.DoLayoutList();
                    }

                    if (weaponTriggerReorderableList != null)
                    {
                        weaponTriggerReorderableList.DoLayoutList();
                    }
                }
            };

            weaponLocationGUI = root.Q<IMGUIContainer>("weapon-location-gui");
            weaponLocationGUI.onGUIHandler = () =>
            {
                if (selectedWeaponTemplate == null) return;

                Rect position = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                position.width -= 75 + EditorGUIUtility.standardVerticalSpacing;
                weaponSaveLocation = EditorGUI.TextField(position, weaponSaveLocation);
                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = 75;
                if (GUI.Button(position, "Create"))
                {
                    CreateWeapon();
                }
            };
        }

        private void SelectWeaponTemplate(object index)
        {
            selectedWeaponTemplateIndex = (int)index;

            GameObject template = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(weaponTemplatesPath[selectedWeaponTemplateIndex]));

            if (selectedWeaponTemplate != template)
            {
                if (selectedWeaponTemplate != null)
                {
                    DestroyImmediate(selectedWeaponTemplate);
                }
                selectedWeaponTemplate = template;

                weaponShootingSystem = selectedWeaponTemplate.GetComponent<WeaponShootingSystem>();
                if (weaponShootingSystem != null)
                {
                    weaponShootingSystemEditor = Editor.CreateEditor(weaponShootingSystem);
                }

                weaponReloadSystem = selectedWeaponTemplate.GetComponent<WeaponReloadSystem>();
                if (weaponReloadSystem != null)
                {
                    weaponReloadSystemEditor = Editor.CreateEditor(weaponReloadSystem);
                }
            }
        }

        private void UpdateWeaponModelReorderableLists()
        {
            if (weaponModel == null || weaponBodyReorderableList != null) return;

            weaponBodyList = new List<Transform>();
            weaponBodyReorderableList = new ReorderableList(weaponBodyList, typeof(Transform), true, true, true, true);
            weaponBodyReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Body");
            };
            weaponBodyReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                Transform element = weaponBodyList[index];
                element = (Transform)EditorGUI.ObjectField(rect, element?.name ?? "Null", element, typeof(Transform), true);
                weaponBodyList[index] = element;
            };
            weaponBodyReorderableList.onAddCallback = (a) =>
            {
                weaponBodyList.Add(null);
            };

            weaponMagazineList = new List<Transform>();
            weaponMagazineReorderableList = new ReorderableList(weaponMagazineList, typeof(Transform), true, true, true, true);
            weaponMagazineReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Magazine");
            };
            weaponMagazineReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                Transform element = weaponMagazineList[index];
                element = (Transform)EditorGUI.ObjectField(rect, element?.name ?? "Null", element, typeof(Transform), true);
                weaponMagazineList[index] = element;
            };
            weaponMagazineReorderableList.onAddCallback = (a) =>
            {
                weaponMagazineList.Add(null);
            };

            weaponTriggerList = new List<Transform>();
            weaponTriggerReorderableList = new ReorderableList(weaponTriggerList, typeof(Transform), true, true, true, true);
            weaponTriggerReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Trigger");
            };
            weaponTriggerReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                Transform element = weaponTriggerList[index];
                element = (Transform)EditorGUI.ObjectField(rect, element?.name ?? "Null", element, typeof(Transform), true);
                weaponTriggerList[index] = element;
            };
            weaponTriggerReorderableList.onAddCallback = (a) =>
            {
                weaponTriggerList.Add(null);
            };
        }

        private void CreateWeapon()
        {
            string folder = $"{weaponSaveLocation}{weaponName}";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            weaponBodyList.Add(weaponModel.transform);
            for (int i = 0; i < weaponBodyList.Count; i++)
            {
                selectedWeaponTemplate.GetComponent<WeaponTemplatePivots>().Configurate(weaponBodyList, weaponMagazineList, weaponTriggerList);
            }

            EquippableItem newItem = ScriptableObject.CreateInstance<EquippableItem>();
            AssetDatabase.CreateAsset(newItem, $"{folder}/{weaponName}.asset");

            EditorUtility.CopySerialized(selectedWeaponTemplate.GetComponent<EquippableObjectIdentifier>().GetItem(), newItem);

            selectedWeaponTemplate.GetComponent<EquippableObjectIdentifier>().SetItem(newItem);



            var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(AssetDatabase.GetAssetPath(selectedWeaponTemplate.GetComponent<Animator>().runtimeAnimatorController));

            AnimatorOverrideController overrideController = new AnimatorOverrideController(selectedWeaponTemplate.GetComponent<Animator>().runtimeAnimatorController);
            AssetDatabase.CreateAsset(overrideController, $"{folder}/{weaponName}.overrideController");

            selectedWeaponTemplate.GetComponent<Animator>().runtimeAnimatorController = overrideController;
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(selectedWeaponTemplate, $"{folder}/{weaponName}.prefab");
            
            newItem.SetItemName(weaponName);
            newItem.SetItemType(weaponType);
            newItem.SetFirstPersonObject(prefab);
            newItem.SetSelectTime(weaponSelectTime);
            newItem.SetSelectTime(weaponHideTime);
            newItem.SetItemImage(weaponImage);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            DestroyImmediate(selectedWeaponTemplate);
        }
        #endregion

        #region [Loot Weapon Methods]
        private void CreateLootWeaponGUI()
        {
            VisualElement root = rootVisualElement;

            lootWeaponTemplatesGUI = root.Q<IMGUIContainer>("loot-weapon-templates-gui");
            lootWeaponTemplatesGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;

                if (selectedLootWeaponTemplate == null)
                {
                    selectedLootWeaponTemplateIndex = -1;
                }

                string buttonName = selectedLootWeaponTemplateIndex < 0 ? "Select template..." : Path.GetFileNameWithoutExtension(lootWeaponTemplatesPath[selectedLootWeaponTemplateIndex]);
                Rect buttonPosition = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                buttonPosition = EditorGUI.PrefixLabel(buttonPosition, new GUIContent("Template"));
                if (GUI.Button(buttonPosition, buttonName, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < lootWeaponTemplatesPath.Count; i++)
                    {
                        menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(lootWeaponTemplatesPath[i])), false, SelectLootWeaponTemplate, i);
                    }
                    menu.DropDown(buttonPosition);
                }
            };

            lootWeaponInformationGUI = root.Q<IMGUIContainer>("loot-weapon-information-gui");
            lootWeaponInformationGUI.onGUIHandler = () =>
            {
                if (selectedLootWeaponTemplate == null) return;

                lootWeaponEquippableItem = (EquippableItem)EditorGUILayout.ObjectField("Item", lootWeaponEquippableItem, typeof(EquippableItem), false);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    GameObject model = (GameObject)EditorGUILayout.ObjectField("Model", lootWeaponModel, typeof(GameObject), false);

                    if (check.changed)
                    {
                        if (lootWeaponModel != null)
                        {
                            DestroyImmediate(lootWeaponModel);
                        }

                        if (model != null)
                        {
                            lootWeaponModel = Instantiate(model, selectedLootWeaponTemplate.transform.position, Quaternion.identity, selectedLootWeaponTemplate.transform);
                        }
                    }
                }
            };

            lootWeaponLocationGUI = root.Q<IMGUIContainer>("loot-weapon-location-gui");
            lootWeaponLocationGUI.onGUIHandler = () =>
            {
                if (selectedLootWeaponTemplate == null) return;

                Rect position = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                position.width -= 75 + EditorGUIUtility.standardVerticalSpacing;
                lootWeaponSaveLocation = EditorGUI.TextField(position, lootWeaponSaveLocation);
                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = 75;
                if (GUI.Button(position, "Create"))
                {
                    CreateLootWeapon();
                }
            };
        }

        private void SelectLootWeaponTemplate(object index)
        {
            selectedLootWeaponTemplateIndex = (int)index;
            GameObject template = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(lootWeaponTemplatesPath[selectedLootWeaponTemplateIndex]));

            if (selectedLootWeaponTemplate != template)
            {
                if (selectedLootWeaponTemplate != null)
                {
                    DestroyImmediate(selectedLootWeaponTemplate);
                }
                selectedLootWeaponTemplate = template;
            }
        }

        private void CreateLootWeapon()
        {
            string folder = $"{lootWeaponSaveLocation}{lootWeaponEquippableItem.GetItemName()}";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            selectedLootWeaponTemplate.GetComponent<LootWeapon>().SetItem(lootWeaponEquippableItem);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(selectedLootWeaponTemplate, $"{folder}/{lootWeaponEquippableItem.GetItemName()}[Loot Weapon].prefab");

            DestroyImmediate(selectedLootWeaponTemplate);
        }
        #endregion

        #region [Loot Ammo Methods]
        private void CreateLootAmmoGUI()
        {
            VisualElement root = rootVisualElement;

            lootAmmoTemplatesGUI = root.Q<IMGUIContainer>("loot-ammo-templates-gui");
            lootAmmoTemplatesGUI.onGUIHandler = () =>
            {
                EditorGUIUtility.labelWidth = 150;

                if (selectedLootAmmoTemplate == null)
                {
                    selectedLootAmmoTemplateIndex = -1;
                }

                string buttonName = selectedLootAmmoTemplateIndex < 0 ? "Select template..." : Path.GetFileNameWithoutExtension(lootAmmoTemplatesPath[selectedLootAmmoTemplateIndex]);
                Rect buttonPosition = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                buttonPosition = EditorGUI.PrefixLabel(buttonPosition, new GUIContent("Template"));
                if (GUI.Button(buttonPosition, buttonName, EditorStyles.popup))
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < lootAmmoTemplatesPath.Count; i++)
                    {
                        menu.AddItem(new GUIContent(Path.GetFileNameWithoutExtension(lootAmmoTemplatesPath[i])), false, SelectLootAmmoTemplate, i);
                    }
                    menu.DropDown(buttonPosition);
                }
            };

            lootAmmoInformationGUI = root.Q<IMGUIContainer>("loot-ammo-information-gui");
            lootAmmoInformationGUI.onGUIHandler = () =>
            {
                if (selectedLootAmmoTemplate == null) return;

                lootAmmoEquippableItem = (EquippableItem)EditorGUILayout.ObjectField("Item", lootAmmoEquippableItem, typeof(EquippableItem), false);

                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    GameObject model = (GameObject)EditorGUILayout.ObjectField("Model", lootAmmoModel, typeof(GameObject), false);

                    if (check.changed)
                    {
                        if (lootAmmoModel != null)
                        {
                            DestroyImmediate(lootAmmoModel);
                        }

                        if (model != null)
                        {
                            lootAmmoModel = Instantiate(model, selectedLootAmmoTemplate.transform.position, Quaternion.identity, selectedLootAmmoTemplate.transform);
                        }
                    }
                }

                ammoCount = EditorGUILayout.IntField("Count", ammoCount);
            };

            lootAmmoLocationGUI = root.Q<IMGUIContainer>("loot-ammo-location-gui");
            lootAmmoLocationGUI.onGUIHandler = () =>
            {
                if (selectedLootAmmoTemplate == null) return;

                Rect position = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                position.width -= 75 + EditorGUIUtility.standardVerticalSpacing;
                lootAmmoSaveLocation = EditorGUI.TextField(position, lootAmmoSaveLocation);
                position.x += position.width + EditorGUIUtility.standardVerticalSpacing;
                position.width = 75;
                if (GUI.Button(position, "Create"))
                {
                    CreateLootAmmo();
                }
            };
        }

        private void SelectLootAmmoTemplate(object index)
        {
            selectedLootAmmoTemplateIndex = (int)index;
            GameObject template = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(lootAmmoTemplatesPath[selectedLootAmmoTemplateIndex]));

            if (selectedLootAmmoTemplate != template)
            {
                if (selectedLootAmmoTemplate != null)
                {
                    DestroyImmediate(selectedLootAmmoTemplate);
                }
                selectedLootAmmoTemplate = template;
            }
        }

        private void CreateLootAmmo()
        {
            string folder = $"{lootAmmoSaveLocation}{lootAmmoEquippableItem.GetItemName()}";

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            selectedLootAmmoTemplate.GetComponent<LootAmmo>().SetWeaponItem(lootAmmoEquippableItem);
            selectedLootAmmoTemplate.GetComponent<LootAmmo>().SetCount(ammoCount);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(selectedLootAmmoTemplate, $"{folder}/{lootAmmoEquippableItem.GetItemName()}[Loot Ammo].prefab");

            DestroyImmediate(selectedLootAmmoTemplate);
        }
        #endregion
    }
}