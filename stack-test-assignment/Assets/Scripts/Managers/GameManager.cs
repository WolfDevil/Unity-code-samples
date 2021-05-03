using System;
using UnityEngine;
using Zenject;

public enum EGameState
{
    MENU,
    PLAY,
    LOSE
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer _startCubeRenderer;

    [Inject] private CubeManager _cubeManager;
    [Inject] private CameraController _cameraController;
    [Inject] private UIController _uiController;
    [Inject] private ColorSchemeManager _colorSchemeManager;
    [Inject] private RawImageGradient _backgroundGradient;

    private EGameState _gameState = EGameState.MENU;

    private Action _onClick;
    private int _score;
    private int _highScore;

    private void Start()
    {
        LoadHighScore();
        ChangeState(EGameState.MENU);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _onClick?.Invoke();
    }

    public void ChangeState(EGameState state)
    {
        var prevState = _gameState;
        _gameState = state;
        StateMachineUpdate();
        _uiController.ChangeState(state);
    }

    private void StateMachineUpdate()
    {
        switch (_gameState)
        {
            case EGameState.MENU:
                {
                    _colorSchemeManager.RandomizeColorSceme();
                    _backgroundGradient.SetColor(_colorSchemeManager.GetBGColors());
                    _startCubeRenderer.material = _colorSchemeManager.GetCubeMaterial(0f);
                    _cubeManager.OnScoreUpdate = null;
                    _uiController.UpdateScoreText(_highScore);
                    _cubeManager.HandleMenuState();
                    _cameraController.MoveCameraByBlockHeight(0f);
                    _cameraController.ZoomInCamera();
                    _onClick = () => ChangeState(EGameState.PLAY);
                    _cubeManager.OnYUpdate = null;
                    _cubeManager.OnFail = null;
                    break;
                }
            case EGameState.PLAY:
                {
                    _score = 0;
                    _uiController.UpdateScoreText(_score);
                    _cameraController.ZoomInCamera();
                    _cubeManager.HandleNewGame();
                    _cubeManager.OnScoreUpdate = AddScore;
                    _onClick = _cubeManager.HandleClick;
                    _cubeManager.OnYUpdate = _cameraController.MoveCameraByBlockHeight;
                    _cubeManager.OnFail = () => ChangeState(EGameState.LOSE);
                    break;
                }
            case EGameState.LOSE:
                {
                    UpdateHighScore();
                    _cubeManager.OnScoreUpdate = null;
                    _cameraController.ZoomOutCamera(_cubeManager.LastCubeYPos);
                    _onClick = () => ChangeState(EGameState.MENU);
                    _cubeManager.OnYUpdate = null;
                    _cubeManager.OnFail = null;
                    break;
                }
        }
    }

    private void AddScore()
    {
        _score++;
        _uiController.UpdateScoreText(_score);
    }

    private void UpdateHighScore()
    {
        if (_score > _highScore) _highScore = _score;
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HIGH_SCORE", _highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        if (PlayerPrefs.HasKey("HIGH_SCORE"))
        {
            _highScore = PlayerPrefs.GetInt("HIGH_SCORE");
        }
        else
        {
            _highScore = 0;
        }

    }
}
