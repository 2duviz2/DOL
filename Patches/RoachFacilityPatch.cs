namespace DOL.Patches;

using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[HarmonyPatch(typeof(Image))]
public static class ImagePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Image), "OnEnable")]
    public static void Postfix(Image __instance)
    {
        if (__instance.name == "Roach Icon" && SceneManager.GetActiveScene().name == "Main-Menu")
        {
            __instance.color = Color.white;
            __instance.gameObject.AddComponent<RoachListener>();
        }
    }
}

[HarmonyPatch(typeof(TextMeshProUGUI))]
public static class TextMeshProUGUIPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TextMeshProUGUI), "OnEnable")]
    public static void Postfix(TextMeshProUGUI __instance)
    {
        if (__instance.name == "Cost Text" && SceneManager.GetActiveScene().name == "Main-Menu")
        {
            __instance.color = Color.white;
            __instance.gameObject.AddComponent<RoachListener>();
        }
    }
}

public class RoachListener : MonoBehaviour
{
    Image _image;
    TextMeshProUGUI _text;

    public void Start()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();
        _image = gameObject.GetComponent<Image>();
    }

    public void Update()
    {
        if (_text && _text.text == "PURCHASED") return;

        if (transform.parent.parent.Find("CannotPurchasePanel").gameObject.activeSelf)
        {
            UpdateColor(Color.red);
        }
        else
        {
            UpdateColor(Color.green);
        }
    }

    void UpdateColor(Color color)
    {
        _text?.color = color;
        _image?.color = color;
    }
}