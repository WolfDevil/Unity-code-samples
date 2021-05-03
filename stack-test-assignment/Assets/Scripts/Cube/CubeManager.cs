using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CubeManager : MonoBehaviour
{
    public Action<float> OnYUpdate = null;
    public Action OnFail = null;
    public Action OnScoreUpdate = null;
    [HideInInspector] public float LastCubeYPos { get; private set; }

    private Settings _settings;
    private Cube.Factory _cubeFcatory;
    private ColorSchemeManager _colorSchemeManager;
    private Cube _currentCube = null;
    private Vector3 _lastCubePosition;
    private Vector3 _lastCubeScale;
    private bool _isCubesResetted;
    private List<Cube> _cubes = new List<Cube>();

    [Inject]
    public void Construct(Cube.Factory cubeFactory, Settings setting, ColorSchemeManager colorSchemeManager)
    {
        _cubeFcatory = cubeFactory;
        _settings = setting;
        _colorSchemeManager = colorSchemeManager;
    }

    public void ResetCubes()
    {
        if (_cubes.Count > 0)
        {
            _cubes.ForEach(cube => StartCoroutine(cube.DestroyCube()));
            _cubes.Clear();
        }
        _lastCubePosition = new Vector3(0f, -0.5f, 0f);
        _lastCubeScale = Vector3.one;
        OnYUpdate?.Invoke(0f);
        LastCubeYPos = 0f;
        _currentCube = null;
        _isCubesResetted = true;
    }

    public void HandleNewGame()
    {
        ResetCubes();
        SpawnCube();
    }

    public void HandleMenuState()
    {
        var delay = _cubes.Count > 0 ? 1f : 0f;
        ResetCubes();
    }

    public void HandleClick()
    {
        var isCubePlaced = _currentCube?.Stop();
        if (isCubePlaced ?? true && _cubes.Count > 0)
        {
            OnScoreUpdate?.Invoke();
            SpawnCube();
            MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.MediumImpact);
        }
    }

    private void HandleFail()
    {
        OnFail?.Invoke();
        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.Failure);
    }

    private void SpawnCube()
    {
        var spawnerNum = UnityEngine.Random.Range(0, _settings.SpawnerSettings.Length);
        var spawner = _settings.SpawnerSettings[spawnerNum];

        var cube = _cubeFcatory.Create();

        if (!_isCubesResetted)
        {
            _lastCubePosition = _currentCube.transform.position;
            _lastCubeScale = _currentCube.transform.localScale;
        }

        var x = spawner.MoveDirection == MoveDirection.X ? spawner.SpawnPosition.x : _lastCubePosition.x;
        var z = spawner.MoveDirection == MoveDirection.Z ? spawner.SpawnPosition.z : _lastCubePosition.z;
        var y = _isCubesResetted ? _lastCubePosition.y + _lastCubeScale.y / 2 + cube.transform.localScale.y / 2 : _lastCubePosition.y + cube.transform.localScale.y;

        cube.transform.position = new Vector3(x, y, z);

        cube.OnPlacementFailAction = HandleFail;
        var material = _colorSchemeManager.GetCubeMaterial(y);
        cube.Initialize(spawner.MoveDirection, _lastCubePosition, _lastCubeScale, material);

        OnYUpdate?.Invoke(y);
        LastCubeYPos = y;

        _isCubesResetted = false;
        _currentCube = cube;
        _cubes.Add(cube);
    }

    [Serializable]
    public class Settings
    {
        public SpawnerSettings[] SpawnerSettings;
    }

    [Serializable]
    public class SpawnerSettings
    {
        public Vector3 SpawnPosition;
        public MoveDirection MoveDirection;
    }
}
