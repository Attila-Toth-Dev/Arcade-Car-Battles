using UnityEngine;

using FishNet.Object;

using Inspector;

namespace Weapons
{
    public abstract class BaseWeapon : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] protected GameObject WeaponModel;

        [Header("Properties")]
        [SerializeField] protected float FireRate;
        [SerializeField] protected float ReloadTime;
        [SerializeField] protected float Damage;
        [SerializeField] protected float MaxAmmo;

        [Header("Debugging")]
        [SerializeField, ReadOnly] protected float CurrentAmmo;

        public abstract void Init();

        public abstract void Fire();

        public abstract void Reload();
    }
}
