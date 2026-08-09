using UnityEngine;

namespace GameTemplate.Features
{
    public static class RectTransformUtilities
    {
        /// <summary>
        /// Sets the RectTransform's anchors to match its current position and size, optionally keeping the pivot point unchanged.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="keepPivot"></param>
        public static void SetRectToAnchors(this RectTransform rectTransform, bool keepPivot = true)
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (parent == null)
            {
                return;
            }

            if (!keepPivot)
            {
                SetPivot(rectTransform, new Vector2(0.5f, 0.5f));
            }

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector2 localMin = parent.InverseTransformPoint(corners[0]);
            Vector2 localMax = parent.InverseTransformPoint(corners[2]);

            Rect parentRect = parent.rect;
            Vector2 anchorMin = new Vector2(
                (localMin.x - parentRect.x) / parentRect.width,
                (localMin.y - parentRect.y) / parentRect.height);
            Vector2 anchorMax = new Vector2(
                (localMax.x - parentRect.x) / parentRect.width,
                (localMax.y - parentRect.y) / parentRect.height);

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Sets the RectTransform's anchors to match its current position, anchors min = anchors max
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void SetAnchorsPosition(this RectTransform rectTransform)
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (parent == null)
            {
                return;
            }

            Vector2 size = rectTransform.rect.size;
            Vector2 localPivot = parent.InverseTransformPoint(rectTransform.position);

            Rect parentRect = parent.rect;
            Vector2 anchor = new Vector2(
                (localPivot.x - parentRect.x) / parentRect.width,
                (localPivot.y - parentRect.y) / parentRect.height);

            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Changes the pivot of a RectTransform without moving it on screen.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="pivot"></param>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(
                deltaPivot.x * size.x * rectTransform.localScale.x,
                deltaPivot.y * size.y * rectTransform.localScale.y);

            rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
        }
    }
}
