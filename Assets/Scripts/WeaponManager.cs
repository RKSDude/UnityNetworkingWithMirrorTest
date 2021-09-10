using Mirror;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private string weaponLayerName = "Weapon";
    [SerializeField] private PlayerWeapon primaryWeapon;
    
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;
    
    private void Start()
    {
        equipWeapon(primaryWeapon);
    }

    public PlayerWeapon getCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics getCurrentGraphics()
    {
        return currentGraphics;
    }
    
    //assign weapon to currentWeapon and set weapon graphics layer
    private void equipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponIns = (GameObject) Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics =  _weaponIns.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
        {
            Debug.LogError("No weaponGraphics component found on the object: " + _weaponIns.name);
        }

        if(isLocalPlayer)
        {
            Util.setLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }
}
