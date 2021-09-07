using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] string remoteLayer = "RemotePlayer";
    [SerializeField] string dontDrawLayer = "DontDraw";
    [SerializeField] GameObject playerGraphics;
    [SerializeField] GameObject playerUIPrefab;
    private GameObject playerUIInstance;
    
    private Camera sceneCamera;

    private void Start()
    {
        //Disable other player's component so local inputs don't 
        //make the other player do stuff
        //also assign layers
        if(!isLocalPlayer)
        {
            disableComponents();
            assignLayer();
        }
        else
        {
            //disable scene camera if local player
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //disable local graphics
            setLayerRecursive(playerGraphics, LayerMask.NameToLayer(dontDrawLayer));

            //create UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }
        
        //call setup method in PlayerManager
        GetComponent<PlayerManager>().Setup();
    }

    //set player graphics objects and children to DontDraw layer
    private void setLayerRecursive(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
            setLayerRecursive(child.gameObject, newLayer);
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

    //activate sceneCamera when player disconnects
    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.unRegisterPlayer(transform.name);
    }
}
