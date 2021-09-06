using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerManager))]
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

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager _player = GetComponent<PlayerManager>();
        GameManager.registerPlayer(_netID, _player);
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

        GameManager.unRegisterPlayer(transform.name);
    }
}
