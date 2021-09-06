using Mirror;
using UnityEngine;
using System.Collections;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

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
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
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
        //Kill player and disable player's components & colliders
        isDead = true;
        Debug.Log(transform.name + " has died. F");

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = false;
        }        

        //Respawn player
        StartCoroutine(respawn());
    }

    private IEnumerator respawn()
    {
        //Respawn settings
        yield return new WaitForSeconds(GameManager.instance.importantSettings.respawnTime);

        setDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

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

        Collider col = GetComponent<Collider>();
        if(col != null)
        {
            col.enabled = true;
        }
    }
}