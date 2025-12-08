using UnityEngine;

public class EndState : IGameState
{
	private GameManager game;

	public EndState( GameManager game )
	{
		this.game = game;
	}

	public void Enter()
	{
		Debug.Log("GAME COMPLETE!");
	}

	public void OnCardSelected( Card card ) { }
}
