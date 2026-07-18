using UnityEngine;

namespace SiPVLib.Utilities
{
    /// <summary>
    /// Attribute that marks a ScriptableObject field to be treated as a sub-asset of the parent.
    /// The marked ScriptableObject will be added as a sub-asset to the asset containing the parent object.
    /// When the field is set to null, the sub-asset will be properly cleaned up to prevent ghost assets and memory leaks.
    /// </summary>
    public class SubAssetAttribute : PropertyAttribute
    {
    }
}

