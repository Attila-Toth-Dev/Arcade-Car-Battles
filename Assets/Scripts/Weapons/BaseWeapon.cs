using UnityEngine;

using FishNet.Object;

using Inspector;

namespace Weapons
{
    public abstract class BaseWeapon : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject weaponModel;

        [Header("Properties")]
        [SerializeField] private float fireRate;
        [SerializeField] private float reloadTime;
        [SerializeField] private float damage;
        [SerializeField] private float maxAmmo;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private float currentAmmo;

        public abstract void Init();

        public abstract void Fire();

        public abstract void Reload();
    }
}
