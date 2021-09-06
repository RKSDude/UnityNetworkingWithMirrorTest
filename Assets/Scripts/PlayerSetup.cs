using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] string remoteLayer = "RemotePlayer";
    private Camera sceneCamera;

    private void Start()
    {
        if(!isLocalPlayer)
        {
            disableComponents();
            assignLayer();
        }
        else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    private void assignLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayer);
    }
    
    private void disableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void onDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
