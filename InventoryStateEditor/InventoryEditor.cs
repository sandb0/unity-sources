using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class InventoryEditor : EditorWindow {
    [MenuItem("Window/Inventory Editor")]
    public static void CreateWindowMenuItem() {
        EditorWindow.GetWindow(typeof(InventoryEditor));
    }

    private void OnEnable() {
        if (EditorPrefs.HasKey("InventoryAssetPath")) {
            string objectPath = EditorPrefs.GetString("InventoryAssetPath");
            Actions.inventoryStateAsset = AssetDatabase.LoadAssetAtPath(objectPath, typeof(InventoryState)) as InventoryState;
        }
    }

    private void OnGUI() {
        GUI.Header();
        GUI.Inventory();
    }

    private static class GUI {
        public static void Header() {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Inventory Editor", EditorStyles.boldLabel);

            if (!Actions.inventoryStateAsset) {
                if (GUILayout.Button("New Inventory")) {
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = Actions.inventoryStateAsset;

                    Actions.NewInventoryAsset();
                }
            }

            if (GUILayout.Button("Open Inventory")) {
                Actions.OpenInventoryAsset();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(30);
        }

        public static void Inventory() {
            if (!Actions.inventoryStateAsset) {
                return;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous", GUILayout.Width(75))) {
                Actions.PreviousItem();
            }

            if (GUILayout.Button("Next", GUILayout.Width(75))) {
                Actions.NextItem();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Item", GUILayout.Width(100))) {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = Actions.inventoryStateAsset;

                Actions.AddItem();
            }

            if (GUILayout.Button("Delete Item", GUILayout.Width(100))) {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = Actions.inventoryStateAsset;

                Actions.RemoveItem();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            InventoryItem();
        }

        public static void InventoryItem() {
            if (!Actions.inventoryStateAsset) {
                return;
            }

            if (!(Actions.inventoryStateAsset.items.Count > 0)) {
                return;
            }

            int itemIndex = Actions.itemIndex;

            string itemName = Actions.inventoryStateAsset.items[itemIndex].itemName;
            Actions.inventoryStateAsset.items[itemIndex].itemName = EditorGUILayout.TextField("Item Name", itemName);

            Texture2D itemIcon = Actions.inventoryStateAsset.items[itemIndex].itemIcon;
            Actions.inventoryStateAsset.items[itemIndex].itemIcon = EditorGUILayout.ObjectField("Item Icon", itemIcon, typeof (Texture2D), false) as Texture2D;

            bool isUnique = Actions.inventoryStateAsset.items[itemIndex].isUnique;
            Actions.inventoryStateAsset.items[itemIndex].isUnique = EditorGUILayout.Toggle("Is Unique", isUnique);
        }
    }

    private static class Actions {
        // ScriptableObject Asset.
        public static InventoryState inventoryStateAsset;
        public static int itemIndex;

        public static void NewInventoryAsset() {
            inventoryStateAsset = InventoryAsset.Create();

            if (inventoryStateAsset) {
                inventoryStateAsset.items = new List<InventoryItem>();

                string relativePath = AssetDatabase.GetAssetPath(inventoryStateAsset);
                EditorPrefs.SetString("InventoryAssetPath", relativePath);
            }
        }

        public static void OpenInventoryAsset() {
            string absolutePath = EditorUtility.OpenFilePanel("Select Inventory Asset File", "", "asset");

            if (absolutePath.StartsWith(Application.dataPath)) {
                string relativePath = absolutePath.Substring(Application.dataPath.Length - "Assets".Length);
                inventoryStateAsset = AssetDatabase.LoadAssetAtPath(relativePath, typeof(InventoryState)) as InventoryState;

                if (inventoryStateAsset) {
                    EditorPrefs.SetString("InventoryAssetPath", relativePath);
                }

                if (inventoryStateAsset.items == null) {
                    inventoryStateAsset.items = new List<InventoryItem>();
                }
            }
        }

        public static void AddItem() {
            InventoryItem item = new InventoryItem();

            inventoryStateAsset.items.Add(item);
            itemIndex = inventoryStateAsset.items.Count - 1;
        }

        public static void RemoveItem() {
            InventoryItem item = inventoryStateAsset.items[itemIndex];

            inventoryStateAsset.items.Remove(item);
        }

        public static void PreviousItem() {
            if (itemIndex <= 0) {
                return;
            }

            itemIndex--;
        }

        public static void NextItem() {
            if (itemIndex >= inventoryStateAsset.items.Count - 1) {
                return;
            }

            itemIndex++;
        }

        private static class InventoryAsset {
            public static InventoryState Create() {
                InventoryState asset = ScriptableObject.CreateInstance<InventoryState>();

                AssetDatabase.CreateAsset(asset, "Assets/InventoryState.asset");
                AssetDatabase.SaveAssets();

                return asset;
            }
        }
    }
}
