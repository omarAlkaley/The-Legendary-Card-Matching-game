using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;

	public List<Card> Cards { get; private set; }
	public Card FirstCard { get; set; }
	public Card SecondCard { get; set; }
	public int MatchedCards { get; set; }

	public EventManager EventManager { get; private set; }
	public IMatchStrategy MatchStrategy { get; private set; }

	private IGameState currentState;

	[Header("Settings")]
	public int pairCount = 8;
	[SerializeField] private CardFactory cardFactory;

	private void Awake()
	{
		Instance = this;
		EventManager = GetComponent<EventManager>();
	}

	private void Start()
	{
		MatchStrategy = new ExactMatchStrategy();
		Cards = cardFactory.CreateCardPairs(pairCount);

		ChangeState(new WaitingState(this));
	}

	public void ChangeState( IGameState newState )
	{
		currentState = newState;
		currentState.Enter();
	}

	public void SelectCard( Card card )
	{
		currentState.OnCardSelected(card);
	}
}
