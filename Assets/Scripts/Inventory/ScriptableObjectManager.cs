using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace GameControll
{
    public static class ScriptableObjectManager
    {
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if (Resources.Load(typeof(T).ToString()) == null)
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString() + ".asset");

                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            else
            {
                Debug.Log(typeof(T).ToString() + "already created!");
            }
        }

        [MenuItem("Assets/Inventory/Create Consumable List Scriptable Object")]
        public static void CreateConsumable()
        {
            ScriptableObjectManager.CreateAsset<ConsumableScriptableObject>();
        }
        [MenuItem("Assets/Inventory/Create Spell List Scriptable Object")]
        public static void CreateSpellItemsList()
        {
            ScriptableObjectManager.CreateAsset<SpellItemScriptableObject>();
        }
        [MenuItem("Assets/Inventory/Create Weapon List Scriptable Object")]
        public static void CreateWeaponList()
        {
            ScriptableObjectManager.CreateAsset<WeaponScriptableObject>();
        }
    }
}
