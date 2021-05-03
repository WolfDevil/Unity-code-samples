using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera = null;

    [Inject] private Settings _settings = null;
    private Vector3 _targetCameraPosition;
    private float _targetOrtographicSize;
    private float _defaultOrtographicSize;

    private void Awake()
    {
        _targetCameraPosition = _camera.transform.position;
        AspectRatioCorrection();
    }

    private void Update()
    {
        UpdateCameraPosition();
    }

    public void MoveCameraByBlockHeight(float y)
    {
        _targetCameraPosition = new Vector3(_targetCameraPosition.x, y + 4.5f, _targetCameraPosition.z);
    }

    public void ZoomOutCamera(float y)
    {
        _targetOrtographicSize = Mathf.Clamp(_targetOrtographicSize * y / 2f, _defaultOrtographicSize, 100f);
        _targetCameraPosition = new Vector3(_targetCameraPosition.x, y / 2 + 4.5f, _targetCameraPosition.z);
    }

    public void ZoomInCamera()
    {
        _targetOrtographicSize = _defaultOrtographicSize;
    }

    private void AspectRatioCorrection()
    {
        var newOrtographicSize = _camera.aspect.Map(0.75f, 0.4f, 1.2f, 2f);
        newOrtographicSize = Mathf.Clamp(newOrtographicSize, 1f, 2.5f);
        _camera.orthographicSize = newOrtographicSize;
        _defaultOrtographicSize = newOrtographicSize;
    }

    private void UpdateCameraPosition()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _targetCameraPosition,
            1 - _settings.MoveViscosity
        );
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetOrtographicSize, 1 - _settings.MoveViscosity);
    }

    [Serializable]
    public class Settings
    {
        [Range(0f, 1f)] public float MoveViscosity;
    }
}
