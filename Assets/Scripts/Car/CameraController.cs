using UnityEngine;

using FishNet.Object;

namespace Car
{
    public class CameraController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform target;

        [Header("Properties")]
        [SerializeField] private float cameraHeight = 6.0f;
        [SerializeField] private float cameraDistance = 6.0f;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!base.IsOwner)
            {
                gameObject.SetActive(false);
                return;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            playerCamera.transform.LookAt(target);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!base.IsOwner)
                return;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void LateUpdate()
        {
            if (!base.IsOwner)
                return;

            if(target == null)
            {
                Debug.LogWarning("Target is not assigned for the camera controller.", this);
                return;
            }

            Vector3 camPos = new Vector3(target.position.x - cameraDistance, target.position.y + cameraHeight, target.position.z - cameraDistance);
            
            playerCamera.transform.position = camPos;
            playerCamera.transform.LookAt(target);
        }
    }
}
