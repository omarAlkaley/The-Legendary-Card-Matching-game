using System.Collections;
using UnityEngine;

public class CheckingMatchState : IGameState
{
	GameManager game;
	Card a, b;
	bool comparisonStarted = false;

	public CheckingMatchState( GameManager g , Card a , Card b )
	{
		game = g; this.a = a; this.b = b;
	}

	public void Enter()
	{
		// start comparison but DO NOT prevent further visual flips
		comparisonStarted = true;
		game.StartGameCoroutine(CompareCoroutine());
	}

	// A2: OnCardSelected should ignore additional cards while checking, so do nothing here.
	public void OnCardSelected( Card card ) { /* intentionally ignored */ }

	System.Collections.IEnumerator CompareCoroutine()
	{
		// small wait for flip animation to mostly finish
		yield return new WaitForSeconds(0.35f);

		if (game.matchStrategy.IsMatch(a , b))
		{
			game.OnMatchFound(a , b);
		}
		else
		{
			game.OnMismatchFound(a , b);
			// schedule close after a delay; but allow user to keep flipping
			yield return new WaitForSeconds(0.4f);
			a.CloseVisual();
			b.CloseVisual();
		}

		// After processing, return to waiting state and try to consume any buffered visual flips
		game.ChangeState(new WaitingState(game));
		game.TryConsumeVisuals();
	}
}
