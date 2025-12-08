public class WaitingState : IGameState
{
	private GameManager game;

	public WaitingState( GameManager game )
	{
		this.game = game;
	}

	public void Enter() { }

	public void OnCardSelected( Card card )
	{
		card.Reveal();
		game.EventManager.CardRevealed(card);

		game.FirstCard = card;
		game.ChangeState(new FirstFlipState(game));
	}
}
