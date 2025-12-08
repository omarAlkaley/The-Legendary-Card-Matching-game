using UnityEngine;

public class CheckingMatchState : IGameState
{
	private GameManager game;

	public CheckingMatchState( GameManager game )
	{
		this.game = game;
	}

	public void Enter()
	{
		game.StartCoroutine(CheckMatchCoroutine());
	}

	private System.Collections.IEnumerator CheckMatchCoroutine()
	{
		yield return new WaitForSeconds(0.5f);

		bool isMatch = game.MatchStrategy.IsMatch(game.FirstCard , game.SecondCard);

		if (isMatch)
		{
			game.EventManager.Match(game.FirstCard , game.SecondCard);

			game.MatchedCards += 2;

			if (game.MatchedCards == game.Cards.Count)
			{
				game.EventManager.GameComplete();
				game.ChangeState(new EndState(game));
				yield break;
			}

			game.ChangeState(new WaitingState(game));
		}
		else
		{
			game.EventManager.Mismatch(game.FirstCard , game.SecondCard);

			yield return new WaitForSeconds(0.6f);
			game.FirstCard.Hide();
			game.SecondCard.Hide();

			game.ChangeState(new WaitingState(game));
		}
	}

	public void OnCardSelected( Card card ) { }
}
