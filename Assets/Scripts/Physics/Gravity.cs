using UnityEngine;

using FishNet.Object;

namespace Physics
{
    public class Gravity : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private new Rigidbody rigidbody;

        [Header("Properties")]
        [SerializeField] private float gravityMultiplier = 1.0f;

        private void FixedUpdate()
        {
            if (!base.IsOwner)
                return;

            ApplyGravity();
        }

        private void ApplyGravity()
        {
            Vector3 appliedGravity = UnityEngine.Physics.gravity * gravityMultiplier;
            rigidbody.AddForce(Vector3.down + appliedGravity, ForceMode.Acceleration);
        }
    }
}
