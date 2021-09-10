using UnityEngine;
using Mirror;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] string remoteLayer = "RemotePlayer";
    [SerializeField] string dontDrawLayer = "DontDraw";
    [SerializeField] GameObject playerGraphics;
    
    [SerializeField] GameObject playerUIPrefab;
    [HideInInspector] public GameObject playerUIInstance;

    private void Start()
    {
        //Disable other player's component so local inputs don't 
        //make the other player do stuff
        //also assign layers
        if(!isLocalPlayer)
        {
            assignLayer();
            disableComponents();
        }
        else
        {
            //disable local graphics
            Util.setLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayer));

            //create UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
            //configure UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
            {
                Debug.LogError("No PlayerUI component found on PlayerUI prefab");
            }

            ui.setController(GetComponent<PlayerController>());
        }

        //call setup method from PlayerManager
        GetComponent<PlayerManager>().Setup();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //Register player on start
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

        if(isLocalPlayer)
        {
            GameManager.instance.setSceneCamera(true);

            GameManager.unRegisterPlayer(transform.name);
        }
    }
}
