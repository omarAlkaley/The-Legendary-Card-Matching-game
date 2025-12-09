public class WaitingState : IGameState
{
	GameManager game;
	public WaitingState( GameManager gm ) { game = gm; }

	public void Enter() { /* nothing */ }

	public void OnCardSelected( Card card )
	{
		// We only accept card if it's the earliest in visual buffer
		// The GameManager will call this with the next visual flip, but double-check
		Card dequeued = game.DequeueVisual();
		if (dequeued == null || dequeued != card) return; // not ready

		// accept as first card
		game.ChangeState(new FirstFlipState(game , card));
	}
}
