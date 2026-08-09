using GameTemplate.Features;
using UnityEditor;
using UnityEngine;

namespace GameTemplate.Editor
{
    public static class RectTransformUtilitiesMenu
    {
#if UNITY_EDITOR_OSX
        private const string KeepPivotShortcutText = " (Cmd+Opt+R)";
        private const string ResetPivotShortcutText = " (Cmd+Opt+T)";
        private const string AnchorsPositionShortcutText = " (Cmd+Opt+O)";
#else
        private const string KeepPivotShortcutText = " (Ctrl+Alt+R)";
        private const string ResetPivotShortcutText = " (Ctrl+Alt+T)";
        private const string AnchorsPositionShortcutText = " (Ctrl+Alt+O)";
#endif

        [MenuItem("GameTemplate/RectTransform/Set Rect To Anchors (Keep Pivot) %&r")]
        [MenuItem("CONTEXT/RectTransform/Set Rect To Anchors (Keep Pivot)" + KeepPivotShortcutText)]
        private static void SetRectToAnchorsKeepPivot(MenuCommand command)
        {
            ApplyToSelection(command, rt => rt.SetRectToAnchors(true), "Set Rect To Anchors");
        }

        [MenuItem("GameTemplate/RectTransform/Set Rect To Anchors (Reset Pivot) %&t")]
        [MenuItem("CONTEXT/RectTransform/Set Rect To Anchors (Reset Pivot)" + ResetPivotShortcutText)]
        private static void SetRectToAnchorsResetPivot(MenuCommand command)
        {
            ApplyToSelection(command, rt => rt.SetRectToAnchors(false), "Set Rect To Anchors");
        }

        [MenuItem("GameTemplate/RectTransform/Set Anchors To Position %&o")]
        [MenuItem("CONTEXT/RectTransform/Set Anchors To Position" + AnchorsPositionShortcutText)]
        private static void SetAnchorsPosition(MenuCommand command)
        {
            ApplyToSelection(command, rt => rt.SetAnchorsPosition(), "Set Anchors To Position");
        }

        private static void ApplyToSelection(MenuCommand command, System.Action<RectTransform> action, string undoLabel)
        {
            // Context menu invocation targets a single component; main menu / shortcut invocation targets the full selection.
            if (command != null && command.context is RectTransform singleTarget)
            {
                Undo.RecordObject(singleTarget, undoLabel);
                action(singleTarget);
                EditorUtility.SetDirty(singleTarget);
                return;
            }

            Transform[] selection = Selection.transforms;
            if (selection == null || selection.Length == 0)
            {
                return;
            }

            foreach (Transform transform in selection)
            {
                if (transform is not RectTransform rectTransform)
                {
                    continue;
                }

                Undo.RecordObject(rectTransform, undoLabel);
                action(rectTransform);
                EditorUtility.SetDirty(rectTransform);
            }
        }
    }
}
