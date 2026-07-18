using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SiPVLib.Utilities.Editor
{
    /// <summary>
    /// Editor processor that manages sub-assets for fields marked with SubAssetAttribute.
    /// Ensures that marked ScriptableObjects are properly registered as sub-assets of their parent.
    /// When a field is null, automatically creates a new instance of that field's type to ensure it's never null.
    /// </summary>
    public sealed class SubAssetProcessor : AssetModificationProcessor
    {
        private static readonly HashSet<string> ProcessingAssets = new();

        /// <summary>
        /// Called when assets are about to be saved. This is where we manage sub-asset relationships.
        /// </summary>
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var assetPath in paths)
            {
                ProcessAsset(assetPath);
            }

            return paths;
        }

        /// <summary>
        /// Processes a single asset to handle sub-asset relationships.
        /// </summary>
        private static void ProcessAsset(string assetPath)
        {
            // Only process ScriptableObject assets
            if (!assetPath.EndsWith(".asset"))
                return;

            // Guard against infinite recursion during save
            if (ProcessingAssets.Contains(assetPath))
                return;

            ProcessingAssets.Add(assetPath);

            try
            {
                var mainAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (mainAsset == null)
                    return;

                // Get all fields with SubAssetAttribute
                var fields = GetFieldsWithAttribute(mainAsset.GetType());

                if (fields.Count == 0)
                    return;

                bool hasChanges = false;

                foreach (var field in fields)
                {
                    var value = field.GetValue(mainAsset) as ScriptableObject;
                    var currentSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                        .OfType<ScriptableObject>()
                        .ToList();

                    // If field is null, create a new instance automatically
                    if (value == null)
                    {
                        var fieldType = field.FieldType;
                        if (fieldType.IsAbstract || fieldType.IsInterface)
                            continue; // Skip abstract or interface types

                        // Create new instance of the field's type
                        value = ScriptableObject.CreateInstance(fieldType) as ScriptableObject;
                        if (value != null)
                        {
                            value.name = field.Name;
                            field.SetValue(mainAsset, value);
                            AssetDatabase.AddObjectToAsset(value, assetPath);
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        // Check if the value is already a sub-asset of this asset
                        if (!currentSubAssets.Contains(value))
                        {
                            // Remove the value from its previous parent if it's a sub-asset elsewhere
                            RemoveSubAssetFromPreviousParent(value);

                            // Add as sub-asset
                            value.name = field.Name;
                            AssetDatabase.AddObjectToAsset(value, assetPath);
                            hasChanges = true;
                        }
                    }
                }

                if (hasChanges)
                {
                    EditorUtility.SetDirty(mainAsset);
                }
            }
            finally
            {
                ProcessingAssets.Remove(assetPath);
            }
        }

        /// <summary>
        /// Gets all fields of a type that have the SubAssetAttribute.
        /// </summary>
        private static List<FieldInfo> GetFieldsWithAttribute(System.Type type)
        {
            var fields = new List<FieldInfo>();
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in type.GetFields(bindingFlags))
            {
                if (field.GetCustomAttribute<SubAssetAttribute>() != null)
                {
                    fields.Add(field);
                }
            }

            return fields;
        }


        /// <summary>
        /// Removes a sub-asset from its previous parent asset if it exists in one.
        /// </summary>
        private static void RemoveSubAssetFromPreviousParent(ScriptableObject subAsset)
        {
            var assetPath = AssetDatabase.GetAssetPath(subAsset);
            if (string.IsNullOrEmpty(assetPath))
                return;

            // Check if it's a sub-asset (not the main asset)
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (mainAsset != subAsset)
            {
                // It's a sub-asset, remove it
                AssetDatabase.RemoveObjectFromAsset(subAsset);
            }
        }


        /// <summary>
        /// Called when an asset is deleted. Ensures sub-assets are properly cleaned up.
        /// </summary>
        static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var mainAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if (mainAsset == null)
                return AssetDeleteResult.DidNotDelete;

            var fields = GetFieldsWithAttribute(mainAsset.GetType());
            if (fields.Count == 0)
                return AssetDeleteResult.DidNotDelete;

            // Clean up all sub-assets
            var subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath)
                .OfType<ScriptableObject>()
                .ToList();

            foreach (var subAsset in subAssets)
            {
                Object.DestroyImmediate(subAsset, true);
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}

