using UnityEngine;

namespace SLIDDES.Multiplayer.Couch.UI
{
    /// <summary>
    /// Extension class for RectTransform
    /// </summary>
    public static class RectTransformExtensions
    {
        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static void SetHeight(this RectTransform rt, float height)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRect(this RectTransform rt, float top, float bottom, float left, float right)
        {
            SetTop(rt, top);
            SetBottom(rt, bottom);
            SetLeft(rt, left);
            SetRight(rt, right);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        /// <summary>
        /// Set the width & height of the rectTransform
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetSize(this RectTransform rt, float width, float height)
        {
            rt.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// Set the width & height of the rectTransform
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="size"></param>
        public static void SetSize(this RectTransform rt, Vector2 size)
        {
            rt.sizeDelta = size;
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetWidth(this RectTransform rt, float width)
        {
            rt.sizeDelta = new Vector2(width, rt.sizeDelta.y);
        }
    }
}