namespace DOL.UI;

using DOL.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Builder
{
    static TMP_FontAsset _font;

    public static TMP_FontAsset font
    {
        get
        {
            if (!_font) LoadFont();
            return _font;
        }
    }

    public static void LoadFont() => _font = Plugin.Ass<TMP_FontAsset>("Fonts & Materials/Smart 9 SDF");

    /// <summary> Creates a pre-configured empty canvas </summary>
    /// <param name="width"> Width of the canvas </param>
    /// <param name="height"> Height of the canvas </param>
    /// <returns> The created canvas </returns>
    public static Canvas Canvas(float width = 1920, float height = 1080)
    {
        GameObject canvasObj = new GameObject("RuntimeCanvas")
        {
            layer = LayerMask.NameToLayer("UI")
        };

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        canvas.sortingOrder = 10000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(width, height);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        canvasObj.AddComponent<GraphicRaycaster>();

        Plugin.LogInfo("Canvas created!");

        return canvas;
    }

    /// <summary> Creates a text object with the default font </summary>
    /// <param name="container"> The object where the text will be created as a child of </param>
    /// <param name="position"> Position of the text's transform </param>
    /// <param name="sizeDelta"> Size of the text's transform </param>
    /// <param name="defaultText"> Text of the text </param>
    /// <param name="size"> Size of the text's font </param>
    /// <param name="alignment"> Sets the alignment for the text </param>
    /// <returns> The created text </returns>
    public static TMP_Text Text
    (
        GameObject container,
        Vector2 position,
        Vector2 sizeDelta,
        string defaultText = "Ella jura",
        float size = 20,
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft,
        Color? color = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null,
        Vector2? pivot = null
    )
    {
        GameObject obj = new GameObject("Text");
        obj.transform.SetParent(container.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin ?? new Vector2(0, 1);
        rect.anchorMax = anchorMax ?? new Vector2(0, 1);
        rect.pivot = pivot ?? new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;

        TMP_Text Text = obj.AddComponent<TextMeshProUGUI>();
        Text.text = defaultText;
        Text.font = font;
        Text.fontSize = size;
        Text.alignment = alignment;
        Text.raycastTarget = false;
        Text.color = color ?? Color.white;

        return Text;
    }

    public static Button Button
    (
        GameObject container,
        Vector2 position,
        Vector2 sizeDelta,
        string defaultText = "Ella jura",
        float size = 20,
        TextAlignmentOptions alignment = TextAlignmentOptions.TopLeft,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null,
        Vector2? pivot = null
    )
    {
        GameObject obj = new GameObject("Button");
        obj.transform.SetParent(container.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin ?? new Vector2(0, 1);
        rect.anchorMax = anchorMax ?? new Vector2(0, 1);
        rect.pivot = pivot ?? new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;

        Button Button = obj.AddComponent<Button>();

        var img = Image(obj, position, sizeDelta);
        Button.targetGraphic = img;

        Text(obj, position, sizeDelta, defaultText, size, alignment, Color.black);

        return Button;
    }

    public static Image Image
    (
        GameObject container,
        Vector2 position,
        Vector2 sizeDelta,
        Color? color = null,
        Vector2? anchorMin = null,
        Vector2? anchorMax = null,
        Vector2? pivot = null
    )
    {
        GameObject obj = new GameObject("Image");
        obj.transform.SetParent(container.transform, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin ?? new Vector2(0, 1);
        rect.anchorMax = anchorMax ?? new Vector2(0, 1);
        rect.pivot = pivot ?? new Vector2(0, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = sizeDelta;

        Image Image = obj.AddComponent<Image>();
        Image.color = color ?? new Color(1, 1, 1);

        return Image;
    }
}
