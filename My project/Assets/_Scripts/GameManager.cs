using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GridManager gridManager; 
    public UIManager uiManager; 


    public int startingMoves = 5;
    public int moves;
    public int score;


    public bool IsGameOver { get; private set; }


    void Start()
    {
        moves = startingMoves;
        score = 0;
        IsGameOver = false;
        uiManager.UpdateScore(score);
        uiManager.UpdateMoves(moves);
    }


    public void AddScore(int n)
    {
        score += n;
        uiManager.UpdateScore(score);
    }


    public void UseMove()
    {
        if (IsGameOver) return;
        moves--;
        uiManager.UpdateMoves(moves);
        if (moves <= 0)
        {
            EndGame();
        }
    }


    void EndGame()
    {
        IsGameOver = true;
        uiManager.ShowGameOver(score);
    }
}