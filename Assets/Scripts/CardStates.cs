public interface ICardState
{
	void Enter( UICard card );
	void OnClick( UICard card );
}

public class CardFaceDownState : ICardState
{
	public void Enter( UICard card ) { }

	public void OnClick( UICard card )
	{
		card.NotifyBoardManager();
	}
}

public class CardFaceUpState : ICardState
{
	public void Enter( UICard card ) { }

	public void OnClick( UICard card )
	{
		// Already flipped, ignore
	}
}

public class CardMatchedState : ICardState
{
	public void Enter( UICard card ) { }

	public void OnClick( UICard card )
	{
		// Matched cards can't be clicked
	}
}