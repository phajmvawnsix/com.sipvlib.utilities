using SiPVLib.Utilities.Serialization;
using UnityEditor;
using UnityEngine;

namespace SiPVLib.Utilities.Editor.Serialization
{
    /// <summary>
    /// Draws <see cref="SiPVLib.Utilities.Serialization.SerializableDictionary{TKey,TValue}"/> as a
    /// paired key/value list, standing in for Odin's dictionary drawer.
    ///
    /// Matched by field name rather than type: the dictionary's backing <c>_keys</c>/<c>_values</c>
    /// lists are what Unity actually serializes, and a drawer registered on the dictionary type
    /// cannot reach them as <see cref="SerializedProperty"/>s. Concrete subclasses opt in by applying
    /// <see cref="SerializableDictionaryAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionaryAttribute))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var keys = property.FindPropertyRelative("_keys");
            if (keys == null) return EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            // Header + one row per entry + the add/clear row.
            return EditorGUIUtility.singleLineHeight
                   + (keys.arraySize + 1) * (EditorGUIUtility.singleLineHeight + Spacing)
                   + Spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keys = property.FindPropertyRelative("_keys");
            var values = property.FindPropertyRelative("_values");

            if (keys == null || values == null)
            {
                EditorGUI.LabelField(position, label.text, "Not a SerializableDictionary");
                return;
            }

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var headerRect = new Rect(position.x, position.y, position.width, lineHeight);

            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded,
                $"{label.text} ({keys.arraySize})", true);

            if (!property.isExpanded) return;

            var y = position.y + lineHeight + Spacing;
            using (new EditorGUI.IndentLevelScope())
            {
                for (var i = 0; i < keys.arraySize; i++)
                {
                    var rowRect = new Rect(position.x, y, position.width, lineHeight);
                    DrawRow(rowRect, keys, values, i);
                    y += lineHeight + Spacing;
                }

                DrawFooter(new Rect(position.x, y, position.width, lineHeight), keys, values);
            }
        }

        private static void DrawRow(Rect rect, SerializedProperty keys, SerializedProperty values, int index)
        {
            const float removeWidth = 22f;
            var half = (rect.width - removeWidth - Spacing * 2) * 0.5f;

            var keyRect = new Rect(rect.x, rect.y, half, rect.height);
            var valueRect = new Rect(rect.x + half + Spacing, rect.y, half, rect.height);
            var removeRect = new Rect(rect.xMax - removeWidth, rect.y, removeWidth, rect.height);

            EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(index), GUIContent.none);
            EditorGUI.PropertyField(valueRect, values.GetArrayElementAtIndex(index), GUIContent.none);

            if (GUI.Button(removeRect, "-"))
            {
                keys.DeleteArrayElementAtIndex(index);
                if (index < values.arraySize) values.DeleteArrayElementAtIndex(index);
            }
        }

        private static void DrawFooter(Rect rect, SerializedProperty keys, SerializedProperty values)
        {
            const float buttonWidth = 60f;

            var addRect = new Rect(rect.xMax - buttonWidth * 2 - Spacing, rect.y, buttonWidth, rect.height);
            var clearRect = new Rect(rect.xMax - buttonWidth, rect.y, buttonWidth, rect.height);

            if (GUI.Button(addRect, "Add"))
            {
                keys.InsertArrayElementAtIndex(keys.arraySize);
                values.InsertArrayElementAtIndex(values.arraySize);
            }

            if (GUI.Button(clearRect, "Clear"))
            {
                keys.ClearArray();
                values.ClearArray();
            }
        }
    }
}
