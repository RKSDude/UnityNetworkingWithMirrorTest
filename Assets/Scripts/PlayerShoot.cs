using Mirror;
using UnityEngine;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;
    private RaycastHit _hit;
    public PlayerWeapon weapon;
    
    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client] private void Shoot()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.weaponRange, mask))
        {
            if(_hit.collider.tag == "Player") 
            {
                cmdPlayerShot(_hit.collider.name, weapon.weaponDamage);
            }
        }
    }

    [Command] private void cmdPlayerShot(string playerID, float damage)
    {
        Debug.Log(playerID + " has been shot");

        PlayerManager player = GameManager.getPlayer(playerID);
        player.takeDamage(damage);
    }

}
