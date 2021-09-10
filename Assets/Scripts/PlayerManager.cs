using Mirror;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject spawnEffect;

    [SerializeField] private GameObject[] disableObjOnDeath;
    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    
    //[SyncVar] attribute syncs vars across clients
    [SyncVar] private float currentHealth;
    [SyncVar] private bool _isDead = false;
    public bool isDead
    {
        get{return _isDead;}
        protected set{_isDead = value;}
    }
    private bool firstSetup = true;

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            rpcTakeDamage(9999999999999999);
            Debug.Log(transform.name + " has killed themselves. F");
        }
    }

    public void Setup()
    {
        if(isLocalPlayer)
        {
            //disable scene camera
            GameManager.instance.setSceneCamera(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }
        cmdBroadcastJoin();
    }

    [Command] private void cmdBroadcastJoin()
    {
        rpcSetupPlayerOnAllClients(); 
    }

    [ClientRpc] private void rpcSetupPlayerOnAllClients()
    {   
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            firstSetup = false;
        }
        setDefaults();
    }

    [ClientRpc] public void rpcTakeDamage(float damageAmount)
    {
        //Make player take damage
        //[ClientRpc] attribute syncs currentHealth across all clients
        currentHealth -= damageAmount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(isDead)
        {
            return;
        }
        if(currentHealth <= 0)
        {
            die();
        }
    }

    private void die()
    {
        //Kill player and disable player's components
        isDead = true;
        Debug.Log(transform.name + " has died. F");

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //disable components (i.e graphics)
        for (int i = 0; i < disableObjOnDeath.Length; i++)
        {
            disableObjOnDeath[i].SetActive(false);
        }

        //disable colliders
        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = false;
        }        

        //spawn death effect
        GameObject deathGFX = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathGFX, 1.5f);

        //enable scene camera
        if(isLocalPlayer)
        {
            GameManager.instance.setSceneCamera(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        //Respawn player
        StartCoroutine(respawn());
    }

    private IEnumerator respawn()
    {
        //Respawn settings
        yield return new WaitForSeconds(GameManager.instance.importantSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        Setup();

        Debug.Log(transform.name + " has respawned");
    }

    public void setDefaults()
    {
        //reset stuff back to default
        //called in respawn();
        isDead = false;
        currentHealth = maxHealth;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        for (int i = 0; i < disableObjOnDeath.Length; i++)
        {
            disableObjOnDeath[i].SetActive(true);
        }

        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }

        //create spawn effect
        GameObject spawnGFX = (GameObject) Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(spawnGFX, 1.5f);
    }
}