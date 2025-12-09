public class EndState : IGameState
{
	GameManager game;
	public EndState( GameManager g ) { game = g; }
	public void Enter()
	{
		// end logic handled by GameManager's DispatchGameOver
	}
	public void OnCardSelected( Card card ) { /* ignore */ }
}
