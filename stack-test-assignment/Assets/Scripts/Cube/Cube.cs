using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public enum MoveDirection
{
    X,
    Z
}

public class Cube : MonoBehaviour
{
    public Action OnPlacementFailAction = null;

    [SerializeField] private MeshRenderer _meshRenderer = null;

    [Inject] private Settings _settings = null;
    private Action _updateAction = null;
    private MoveDirection _moveDirection;
    private float _speed;
    private Vector3 _lastCubePosition;
    private Vector3 _lastCubeScale;



    public void Initialize(MoveDirection direction, Vector3 lastCubePosition, Vector3 lastCubeScale, Material material = null)
    {
        _lastCubePosition = lastCubePosition;
        _lastCubeScale = lastCubeScale;
        _moveDirection = direction;
        _speed = _settings.Speed;
        transform.localScale = new Vector3(_lastCubeScale.x, transform.localScale.y, _lastCubeScale.z);
        if (material != null) _meshRenderer.material = material;

        _updateAction = MoveCube;
    }

    private void Update()
    {
        _updateAction?.Invoke();
    }

    private void MoveCube()
    {
        switch (_moveDirection)
        {
            case MoveDirection.X:
                {
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -_settings.MoveDistance, _settings.MoveDistance), transform.position.y, transform.position.z);
                    if (Mathf.Abs(0f - transform.position.x) >= _settings.MoveDistance) _speed *= -1f;
                    transform.position += Vector3.right * Time.deltaTime * _speed;
                    break;
                }
            case MoveDirection.Z:
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, -_settings.MoveDistance, _settings.MoveDistance));
                    if (Mathf.Abs(0f - transform.position.z) >= _settings.MoveDistance) _speed *= -1f;
                    transform.position += Vector3.forward * Time.deltaTime * _speed;
                    break;
                }
            default:
                {
                    Debug.LogError("MovingCube.MoveCube(): Not implemented move direction.");
                    break;
                }
        }
    }

    private void SplitCubeZ(float hangover, float direction)
    {
        float newZSize = _lastCubeScale.z - Mathf.Abs(hangover);
        float fallingCubeSize = transform.localScale.z - newZSize;
        float newZPos = _lastCubePosition.z + (hangover / 2);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, newZSize);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPos);

        float cubeEdge = transform.position.z + (newZSize / 2f * direction);
        float fallingCubeZPos = cubeEdge + fallingCubeSize / 2f * direction;
        SpawnDropCube(fallingCubeZPos, fallingCubeSize);
    }

    private void SplitCubeX(float hangover, float direction)
    {
        float newXSize = _lastCubeScale.x - Mathf.Abs(hangover);
        float fallingCubeSize = transform.localScale.x - newXSize;
        float newXPos = _lastCubePosition.x + (hangover / 2);
        transform.localScale = new Vector3(newXSize, transform.localScale.y, transform.localScale.z);
        transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);

        float cubeEdge = transform.position.x + (newXSize / 2f * direction);
        float fallingCubeXPos = cubeEdge + fallingCubeSize / 2f * direction;
        SpawnDropCube(fallingCubeXPos, fallingCubeSize);
    }

    private void SpawnDropCube(float fallingCubePos, float fallingCubeSize)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<MeshRenderer>().material = _meshRenderer.material;
        switch (_moveDirection)
        {
            case MoveDirection.X:
                {
                    cube.transform.localScale = new Vector3(fallingCubeSize, transform.localScale.y, transform.localScale.z);
                    cube.transform.position = new Vector3(fallingCubePos, transform.position.y, transform.position.z);
                    break;
                }
            case MoveDirection.Z:
                {
                    cube.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fallingCubeSize);
                    cube.transform.position = new Vector3(transform.position.x, transform.position.y, fallingCubePos);
                    break;
                }
            default:
                {
                    Debug.LogError("MovingCube.SpawnDropCube(): Not implemented move direction.");
                    break;
                }
        }
        cube.AddComponent<Rigidbody>();
        Destroy(cube, 2f);
    }

    private void DropFailedCube()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(Vector3.down * 10f);
    }

    public bool Stop()
    {
        _speed = 0f;

        var lastCubeXZPos = new Vector3(_lastCubePosition.x, transform.position.y, _lastCubePosition.z);
        if (Vector3.Distance(transform.position, lastCubeXZPos) > _settings.MaxCenterDistance)
        {
            switch (_moveDirection)
            {
                case MoveDirection.X:
                    {
                        float hangover = transform.position.x - _lastCubePosition.x;
                        if (Mathf.Abs(hangover) >= _lastCubeScale.x)
                        {
                            DropFailedCube();
                            OnPlacementFailAction?.Invoke();
                            return false;
                        }
                        float direction = hangover > 0f ? 1f : -1f;
                        SplitCubeX(hangover, direction);
                        break;
                    }
                case MoveDirection.Z:
                    {
                        float hangover = transform.position.z - _lastCubePosition.z;
                        if (Mathf.Abs(hangover) >= _lastCubeScale.z)
                        {
                            DropFailedCube();
                            OnPlacementFailAction?.Invoke();
                            return false;
                        }
                        float direction = hangover > 0f ? 1f : -1f;
                        SplitCubeZ(hangover, direction);
                        break;
                    }
                default:
                    {
                        Debug.LogError("MovingCube.Stop(): Not implemented move direction.");
                        break;
                    }
            }
            return true;
        }
        transform.position = lastCubeXZPos;
        return true;
    }

    public IEnumerator DestroyCube()
    {
        _updateAction = null;
        GetComponent<Collider>().enabled = false;
        int axis = UnityEngine.Random.Range(0, 2);
        int dirRand = UnityEngine.Random.Range(0, 2);
        var dir = dirRand == 0 ? -1 : 1;
        var vector = axis == 0 ? new Vector3(dir, 0f, 0f) : new Vector3(0f, 0f, dir);
        float duration = Time.time + 1f;
        while (Time.time < duration)
        {
            var newPos = transform.position + vector * 0.3f;
            transform.position = Vector3.Lerp(transform.position, newPos, 0.3f);
            yield return null;
        }
        Destroy(gameObject, UnityEngine.Random.value);
        yield break;
    }

    [Serializable]
    public class Settings
    {
        [Range(0f, 10f)] public float Speed;
        [Range(0f, 3f)] public float MoveDistance;
        [Range(0f, 0.5f)] public float MaxCenterDistance;
    }

    public class Factory : PlaceholderFactory<Cube> { }
}
