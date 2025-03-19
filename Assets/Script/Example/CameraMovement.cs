using UnityEngine;

namespace VoiceChess.Example.CameraMoves
{
    public class CameraMovement : MonoBehaviour
    {
        private float _moveDuration = 1.5f;
        private static Transform _mainCamera;
        private static bool _isWhiteTurn = true;
        private static float _whiteZPosition = -5.59f;
        private static float _blackZPosition = 8f;
        private static Quaternion _whiteRotation;
        private static Quaternion _blackRotation;

        private static Vector3 _targetPosition;
        private static Quaternion _targetRotation;
        private static float _elapsedTime = 0f;
        private static bool _isMoving = false;

        private void Awake()
        {
            _mainCamera = transform;
        }

        private void Start()
        {
            _whiteRotation = _mainCamera.rotation;
            _blackRotation = Quaternion.Euler(0, 180, 0) * _whiteRotation;

            _mainCamera.position = new Vector3(_mainCamera.position.x, _mainCamera.position.y, _whiteZPosition);
            _mainCamera.rotation = _whiteRotation;
        }

        private void FixedUpdate()
        {
            CameraMove();
        }

        public static void SwitchCameraPosition()
        {
            _isWhiteTurn = !_isWhiteTurn;
            _targetPosition = new Vector3(_mainCamera.position.x, _mainCamera.position.y, _isWhiteTurn ? _whiteZPosition : _blackZPosition);
            _targetRotation = _isWhiteTurn ? _whiteRotation : _blackRotation;

            _elapsedTime = 0f;
            _isMoving = true;
        }

        private void CameraMove()
        {
            if (_isMoving)
            {
                _elapsedTime += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(_elapsedTime / _moveDuration);

                _mainCamera.position = Vector3.Lerp(_mainCamera.position, _targetPosition, t);
                _mainCamera.rotation = Quaternion.Slerp(_mainCamera.rotation, _targetRotation, t);

                if (t >= 1f)
                {
                    _isMoving = false;
                }
            }
        }
    }
}
