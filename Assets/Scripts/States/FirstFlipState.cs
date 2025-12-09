using System.Collections;

public class FirstFlipState : IGameState
{
	GameManager game;
	Card first;

	public FirstFlipState( GameManager g , Card firstCard )
	{
		game = g; first = firstCard;
	}

	public void Enter() { }

	public void OnCardSelected( Card card )
	{
		// Attempt to take next visual flip
		Card dequeued = game.DequeueVisual();
		if (dequeued == null || dequeued != card) return; // not accepted

		// Guard: same card?
		if (dequeued == first) return;

		// Move to checking state with first & second
		game.ChangeState(new CheckingMatchState(game , first , dequeued));
	}
}
