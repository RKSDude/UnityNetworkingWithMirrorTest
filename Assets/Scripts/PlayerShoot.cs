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

    private void Shoot()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.weaponRange, mask))
        {
            Debug.Log("We hit: " + _hit.collider.name);
        }
    }

}
