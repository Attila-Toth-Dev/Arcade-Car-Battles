using UnityEngine;

namespace Weapons.Types
{
    public class PistolWeapon : BaseWeapon
    {
        public override void Fire()
        {
            Debug.Log("Pistol Fired");
        }

        public override void Init()
        {
        }

        public override void Reload()
        {
        }
    }
}