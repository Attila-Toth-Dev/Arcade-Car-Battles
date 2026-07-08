using UnityEngine;

using Inspector;
using Controllers;

namespace Physics
{
    public class SlopeCheck : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CarController controller;

        [Header("Properties")]
        [SerializeField] private float maxSlopeAngle;

        [Header("Debugging")]
        [SerializeField, ReadOnly] private bool isOnSlope;
        [SerializeField, ReadOnly] private bool isExitingSlope;
        [SerializeField, ReadOnly] private float currentSlopeAngle;

        private RaycastHit slopeHit;

        private void FixedUpdate()
        {
            OnSlope();
        }

        public bool OnSlope()
        {
            if (UnityEngine.Physics.Raycast(transform.position, Vector3.down, out slopeHit, controller.ColliderRadius * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(slopeHit.normal, Vector3.up);
                currentSlopeAngle = angle;
            
                isOnSlope = angle < maxSlopeAngle && angle != 0;
                return isOnSlope;
            }

            return false;
        } 

        public Vector3 GetSlopeMoveDirection(Vector3 _currentMoveDirection)
        {
            return Vector3.ProjectOnPlane(_currentMoveDirection, slopeHit.normal).normalized;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isOnSlope ? Color.orange : Color.green;
            Gizmos.DrawLine(transform.position, -transform.up * (controller.ColliderRadius * 0.5f + 0.3f));
        }
    }
}
