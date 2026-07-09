using UnityEngine;

using FishNet.Object;

namespace Controllers
{
    public class CameraController : NetworkBehaviour
    {
        #region Getters & Setters
        
        public Vector3 Forward
        {
            get => forward;
        }

        public Vector3 Right
        {
            get => right;
        }

        public Camera Camera
        {
            get => camera;
        }

        #endregion

        [Header("References")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform target;

        [Header("Properties")]
        [SerializeField] private float cameraHeight = 6.0f;
        [SerializeField] private float cameraDistance = 6.0f;

        private Vector3 forward;
        private Vector3 right;

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

            camera.transform.LookAt(target);
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

            forward = camera.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            right = camera.transform.right;
            right.y = 0.0f;
            right.Normalize();

            Vector3 camPos = new Vector3(target.position.x - cameraDistance, target.position.y + cameraHeight, target.position.z - cameraDistance);

            camera.transform.position = camPos;
            camera.transform.LookAt(target);
        }
    }
}
