using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Uses your uploaded IGameState and IMatchStrategy (they stay unchanged). 
public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	[Header("References")]
	public CardFactory factory;
	public EventManager EventManager; // assign in inspector
	public MonoBehaviour matchStrategyBehaviour; // attach a component that implements IMatchStrategy or leave null to use ExactMatchStrategy

	[Header("Layout")]
	public int rows = 3;
	public int cols = 4;

	[Header("Scoring")]
	public int matchPoints = 100;
	public int mismatchPenalty = 10;

	// internal
	List<Card> cards = new List<Card>();
	IGameState currentState;
	public IMatchStrategy matchStrategy;

	// Visual buffer: stores cards that have been visually flipped but not logically consumed by FSM
	Queue<Card> visualBuffer = new Queue<Card>();

	// gameplay tracking
	public int Score { get; private set; } = 0;
	int matchedCount = 0;

	void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	void Start()
	{
		// setup match strategy
		if (matchStrategyBehaviour != null && matchStrategyBehaviour is IMatchStrategy)
			matchStrategy = matchStrategyBehaviour as IMatchStrategy;
		else
			matchStrategy = new ExactMatchStrategy(); // fallback (your uploaded file). :contentReference[oaicite:3]{index=3}

		// if there's a saved game, load, otherwise start new
		var data = SaveSystem.Load();
		if (data != null) LoadFromData(data);
		else StartNew(rows , cols);
	}

	#region FSM API
	public void ChangeState( IGameState state )
	{
		currentState = state;
		currentState?.Enter();
	}

	// Called by Card when it is visually flipped
	public void RegisterVisualFlip( Card card )
	{
		// A2: visual flips are always accepted and buffered
		visualBuffer.Enqueue(card);

		// If current state can accept cards right now, let it try to consume the next pair(s).
		TryConsumeVisuals();
	}

	// Attempt to feed visual flips to the FSM when appropriate
	public void TryConsumeVisuals()
	{
		// If no state or state doesn't accept, just return.
		if (currentState == null) return;

		// Let the current state pull cards from visualBuffer by repeatedly calling OnCardSelected
		// but states must be written to accept being called only when they can.
		while (visualBuffer.Count > 0)
		{
			// Peek the next visual flip; let current state decide if it wants it.
			// We pass it to the state via OnCardSelected - state implementations will ignore if they're not ready.
			Card next = visualBuffer.Peek();
			int prevCount = visualBuffer.Count;
			currentState.OnCardSelected(next);

			// If the state consumed it, it should have removed it from the buffer by calling DequeueVisual() helper below.
			// To keep states simple, we provide DequeueVisual() they should call when they accept a card.
			if (visualBuffer.Count == prevCount)
			{
				// state didn't accept the card (e.g., in Checking state) -> stop trying
				break;
			}
			// else, continue while loop in case the state accepted and can accept more
		}
	}

	// Called by state to indicate it accepted a visual flip
	public Card DequeueVisual()
	{
		if (visualBuffer.Count == 0) return null;
		return visualBuffer.Dequeue();
	}
	#endregion

	#region Game Setup / Save / Load
	public void StartNew( int r , int c )
	{
		rows = r; cols = c;
		matchedCount = 0;
		Score = 0;
		EventManager.DispatchScore(Score);

		// build ids (pairs)
		int total = rows * cols;
		if (total % 2 != 0) { total -= 1; cols = total / rows; } // ensure even

		int[] ids = new int[total];
		int pairCount = total / 2;
		for (int i = 0; i < pairCount; i++) { ids[i * 2] = i; ids[i * 2 + 1] = i; }

		// shuffle ids
		for (int i = 0; i < ids.Length; i++) { int rIdx = Random.Range(i , ids.Length); int tmp = ids[rIdx]; ids[rIdx] = ids[i]; ids[i] = tmp; }

		// create visuals
		factory.Clear();
		cards = factory.CreateCards(rows , cols , ids);

		// initial state
		ChangeState(new WaitingState(this));
	}

	void LoadFromData( GameData d )
	{
		rows = d.rows; cols = d.cols;
		Score = d.score;
		EventManager.DispatchScore(Score);

		factory.Clear();
		cards = factory.CreateCards(rows , cols , d.cardIds);

		// apply matched flags
		matchedCount = 0;
		for (int i = 0; i < d.matched.Length && i < cards.Count; i++)
		{
			if (d.matched[i])
			{
				cards[i].SetMatched();
				matchedCount++;
			}
		}

		ChangeState(new WaitingState(this));
	}

	public void SaveProgress()
	{
		var data = new GameData();
		data.rows = rows; data.cols = cols;
		int total = rows * cols;
		data.cardIds = new int[total];
		data.matched = new bool[total];

		for (int i = 0; i < total; i++)
		{
			var c = factory.grid.transform.GetChild(i).GetComponent<Card>();
			data.cardIds[i] = c.Id;
			data.matched[i] = c.IsRevealed && c.IsLocked; // IsLocked indicates matched
		}

		data.score = Score;
		SaveSystem.Save(data);
	}

	#endregion

	#region Scoring + Match handling (used by states)
	public void OnMatchFound( Card a , Card b )
	{
		matchedCount += 2;
		Score += matchPoints;
		EventManager.DispatchScore(Score);
		a.SetMatched();
		b.SetMatched();
		EventManager.PlayMatchSFX();
		SaveProgress();

		// check win
		if (matchedCount >= cards.Count)
		{
			EventManager.DispatchGameOver();
		}
	}

	public void OnMismatchFound( Card a , Card b )
	{
		Score = Mathf.Max(0 , Score - mismatchPenalty);
		EventManager.DispatchScore(Score);
		EventManager.PlayMismatchSFX();

		// close them after a small delay (state will schedule closes)
		SaveProgress();
	}
	#endregion

	public Coroutine StartGameCoroutine( IEnumerator coroutine ) => StartCoroutine(coroutine);
}
