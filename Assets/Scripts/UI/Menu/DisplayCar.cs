using UnityEngine;

namespace UI.Menu
{
    public class DisplayCar : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private float rotationSpeed;

        private void Update()
        {
            transform.Rotate(0.0f, rotationSpeed * Time.deltaTime, 0.0f);
        }
    }
}
