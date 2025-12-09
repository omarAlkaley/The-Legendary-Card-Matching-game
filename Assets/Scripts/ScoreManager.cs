using UnityEngine;
public class ScoreManager
{
	public int Score { get; private set; }
	public int Moves { get; private set; }
	
	private int matchStreak = 0;
	public void AddScore( int points )
	{
		Score += points;
	}

	public void AddStreakMatch( int basePoints )
	{
		matchStreak++;

		// Always add base points
		Score += basePoints;

		// Apply streak bonuses
		int bonus = 0;

		if (matchStreak == 2)
			bonus = 50;
		else if (matchStreak == 3)
			bonus = 100;
		else if (matchStreak >= 4)
			bonus = 150;

		Score += bonus;
	}

	public int GetCurrentStreak()
	{
		return matchStreak;
	}

	public void ResetStreak()
	{
		matchStreak = 0;
	}

	public void IncrementMoves()
	{
		Moves++;
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