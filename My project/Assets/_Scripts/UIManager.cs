using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text movesText;
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;


    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }


    public void UpdateScore(int score)
    {
        if (scoreText) scoreText.text = "Score: " + score.ToString();
    }


    public void UpdateMoves(int moves)
    {
        if (movesText) movesText.text = "Moves: " + moves.ToString();
    }


    public void ShowGameOver(int finalScore)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText) finalScoreText.text = "Final Score: " + finalScore.ToString();
        }
    }

    public void OnPlayAgain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}