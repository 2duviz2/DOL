namespace DOL.Classes;

using DOL.UI;
using TMPro;
using UnityEngine;

public static class UIManager
{
    public static Canvas canvas;

    public static void CreateUI()
    {
        if (canvas) DestroyUI();

        UIUpdater.CreateInstance();

        canvas = Builder.Canvas();

        canvas.Text(new Vector2(0, 0), new Vector2(200, 200), $"{PluginInfo.Name} Multiplayer ({PluginInfo.Version})", 15);

        switch (NetworkManager.CurrentState)
        {
            case GameState.Offline:
                var createLobbyButton = canvas.Button(new Vector2(0, 0), new Vector2(200, 50), "Create lobby", 35, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);

                createLobbyButton.onClick.AddListener(() =>
                {
                    NetworkManager.CreateLobby();
                });

                var joinCopiedButton = canvas.Button(new Vector2(0, 25), new Vector2(200, 50), "Join copied lobby", 35, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);

                joinCopiedButton.onClick.AddListener(() =>
                {
                    joinCopiedButton.GetComponentInChildren<TMP_Text>().text = "Joining...";
                    NetworkManager.JoinCopiedLobby();
                });

                break;
            case GameState.Host or GameState.Client:
                var leaveLobbyButton = canvas.Button(new Vector2(0, 0), new Vector2(200, 50), "Leave lobby", 35, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);

                leaveLobbyButton.onClick.AddListener(() =>
                {
                    NetworkManager.LeaveLobby();
                });

                var copyCodeButton = canvas.Button(new Vector2(0, 25), new Vector2(200, 50), "Copy code", 35, TextAlignmentOptions.Center, Vector2.zero, Vector2.zero, Vector2.zero);

                copyCodeButton.onClick.AddListener(() =>
                {
                    GUIUtility.systemCopyBuffer = NetworkManager.Lobby.Value.Id.ToString();
                    copyCodeButton.GetComponentInChildren<TMP_Text>().text = "Copied!";
                });

                break;
        }
    }

    public static void DestroyUI()
    {
        if (!canvas) return;
        GameObject.Destroy(canvas.gameObject);
    }

    public static void RefreshUI() => CreateUI();
}

public class UIUpdater : MonoBehaviour
{
    public static UIUpdater instance;

    public GameState LastState;

    public static void CreateInstance()
    {
        if (instance) return;
        instance = new GameObject().AddComponent<UIUpdater>();
    }

    public void Update()
    {
        if (LastState != NetworkManager.CurrentState)
        {
            UIManager.RefreshUI();
            LastState = NetworkManager.CurrentState;
        }
    }
}