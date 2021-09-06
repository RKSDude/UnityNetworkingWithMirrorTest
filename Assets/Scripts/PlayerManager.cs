using Mirror;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SyncVar] private float currentHealth;

    private void Start()
    {
        setDefaults();
    }

    public void takeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
    }

    public void setDefaults()
    {
        currentHealth = maxHealth;
    }
}