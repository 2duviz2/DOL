namespace DOL.Classes;

using UnityEngine;

public class NetworkEntity : MonoBehaviour
{
    public void Update()
    {
        if (NetworkManager.CurrentState == GameState.Offline)
        {
            OfflineUpdate();
        }
        else
        {
            NetUpdate();
        }
    }

    public virtual void NetUpdate()
    {

    }

    public virtual void OfflineUpdate()
    {

    }
}
