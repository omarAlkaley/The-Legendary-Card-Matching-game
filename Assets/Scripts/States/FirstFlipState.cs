public class FirstFlipState : IGameState
{
	private GameManager game;

	public FirstFlipState( GameManager game )
	{
		this.game = game;
	}

	public void Enter() { }

	public void OnCardSelected( Card card )
	{
		if (card.IsRevealed) return;

		game.SecondCard = card;
		card.Reveal();
		game.EventManager.CardRevealed(card);

		game.ChangeState(new CheckingMatchState(game));
	}
}
