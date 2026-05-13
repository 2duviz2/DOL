using DOL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        public RectTransform rectTransform => obj.GetComponent<RectTransform>();
    }

    extension(Canvas canvas)
    {
        public TMP_Text Text
        (
            Vector2 position,
            Vector2 sizeDelta,
            string defaultText = "Ella jura",
            float size = 20,
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft,
            Color? color = null,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null
        ) => Builder.Text(canvas.gameObject, position, sizeDelta, defaultText, size, alignment, color, anchorMin, anchorMax, pivot);

        public Button Button
        (
            Vector2 position,
            Vector2 sizeDelta,
            string defaultText = "Ella jura",
            float size = 20,
            TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft,
            Vector2? anchorMin = null,
            Vector2? anchorMax = null,
            Vector2? pivot = null
        ) => Builder.Button(canvas.gameObject, position, sizeDelta, defaultText, size, alignment, anchorMin, anchorMax, pivot);
    }
}
