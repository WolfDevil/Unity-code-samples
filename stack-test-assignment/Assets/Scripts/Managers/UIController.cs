using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Text _scoreText;

    public void ChangeState(EGameState state)
    {
        switch(state)
        {
            case EGameState.MENU:
                {
                    _animator.SetTrigger("ShowMenu");
                    break;
                }
            case EGameState.PLAY:
                {
                    _animator.SetTrigger("ShowIngame");
                    break;
                }
            case EGameState.LOSE:
                {
                    _animator.SetTrigger("ShowLose");
                    break;
                }
        }
    }

    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
    }
}
