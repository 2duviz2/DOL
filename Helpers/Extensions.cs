namespace DOL.Helpers;

using UnityEngine;

public static class Extensions
{
    extension(RectTransform rect)
    {
        /// <summary> Sets both anchors and pivots to (0, 1) </summary>
        public void ApplyDefaults()
        {
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
        }
    }

    extension(GameObject obj)
    {
        /// <summary> Gets the object's RectTransform </summary>
        public RectTransform rectTransform() => obj.GetComponent<RectTransform>();
    }
}
