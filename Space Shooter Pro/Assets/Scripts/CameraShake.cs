using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _cameraInitialPosition;
    private float _shakeMagnitude = 0.05f, _shakeTime = 0.5f;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;

        if(_mainCamera == null)
            Debug.Log("no main camera found");
        else
            _cameraInitialPosition = _mainCamera.transform.position;
    }


    public void ShakeCamera()
    {
        InvokeRepeating(nameof(StartShakeCamera), 0f, 0.005f);
        Invoke(nameof(StopShakeCamera), _shakeTime);
    }
    private void StartShakeCamera()
    {
        var transformPosition = _mainCamera.transform.position;
        transformPosition.x += Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        transformPosition.y += Random.value * _shakeMagnitude * 2 - _shakeMagnitude;
        _mainCamera.transform.position = transformPosition;
    }
    private void StopShakeCamera()
    {
        CancelInvoke("StartShakeCamera");
        _mainCamera.transform.position = _cameraInitialPosition;
    }  
    
}