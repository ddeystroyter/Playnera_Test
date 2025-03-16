using UnityEngine;

namespace Playnera_Test
{
    public class CameraController : MonoBehaviour
    {
        public Camera ActiveCamera { get => activeCamera; }
        public Camera activeCamera;

        public Transform minXBound;
        public Transform maxXBound;

        private float minXCamPosition;
        private float maxXCamPosition;


        private const float Y = 0f;
        private const float Z = -10f;

        [SerializeField]
        [Range(0.01f, 1f)]
        private float _dragSensivity = 0.1f;

        private void Start()
        {
            OnCameraChanged();
            MoveActiveCameraByDeltaX(-20);
        }

        private void OnCameraChanged()
        {
            activeCamera = Camera.main;

            float height = activeCamera.orthographicSize * 2;
            float width = height * activeCamera.aspect;

            float halfWidthOfScreenInUnits = height * activeCamera.aspect / 2;
            minXCamPosition = minXBound.position.x + halfWidthOfScreenInUnits;
            maxXCamPosition = maxXBound.transform.position.x - halfWidthOfScreenInUnits;
        }

        public void MoveActiveCameraByDeltaX(float deltaX)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + deltaX * _dragSensivity, minXCamPosition,maxXCamPosition), Y, Z);
        }

    }
}

