using UnityEngine;
public class ScoreManager
{
	public int Score { get; private set; }
	public int Moves { get; private set; }

	public void AddScore( int points )
	{
		Score += points;
		Debug.Log($"Score: {Score}");
	}

	public void IncrementMoves()
	{
		Moves++;
		Debug.Log($"Moves: {Moves}");
	}

	public void ResetScore()
	{
		Score = 0;
		Moves = 0;
	}

	public void SetScore( int score , int moves )
	{
		Score = score;
		Moves = moves;
	}
}