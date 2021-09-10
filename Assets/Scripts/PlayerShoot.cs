using Mirror;
using UnityEngine;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask mask;
    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

    private RaycastHit _hit;
    
    private void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.getCurrentWeapon();
        
        if(currentWeapon.fireRate <= 0f)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    //called on server when shoot
    [Command] private void cmdOnShoot()
    {
        rpcDoMuzzleFlash();
    }

    //called on server when hit
    [Command] private void cmdOnHit(Vector3 hit, Vector3 normal)
    {
        rpcDoHitEffect(hit, normal);
    }

    //called on all clients when shoot
    [ClientRpc] private void rpcDoMuzzleFlash()
    {
        weaponManager.getCurrentGraphics().muzzleFlash.Play();
    }

    //called on all clients when shoot
    [ClientRpc] private void rpcDoHitEffect(Vector3 hit, Vector3 normal)
    {
       GameObject _hitEffect = (GameObject) Instantiate(weaponManager.getCurrentGraphics().hitEffectPrefab, hit, Quaternion.LookRotation(normal));
       Destroy(_hitEffect, 2f);
    }

    //[Client] attribute makes sure server doesn't run Shoot()
    [Client] private void Shoot()
    {   
        if(!isLocalPlayer)
        {
            return;
        }

        Debug.Log("SHOTS FIRED");

        cmdOnShoot();

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.weaponRange, mask))
        {
            Debug.Log(_hit.collider.tag);
            if(_hit.collider.tag == "Player") 
            {
                cmdPlayerShot(_hit.collider.name, currentWeapon.weaponDamage);
            }

            cmdOnHit(_hit.point, _hit.normal);
        }
    }

    //[Command] attribute runs funtion on server
    [Command] private void cmdPlayerShot(string playerID, float damage)
    {
        Debug.Log(playerID + " has been shot");

        PlayerManager player = GameManager.getPlayer(playerID);
        player.rpcTakeDamage(damage);
    }

}
