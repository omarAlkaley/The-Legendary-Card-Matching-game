using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public event Action<Card> OnCardRevealed;
	public event Action<Card , Card> OnMatch;
	public event Action<Card , Card> OnMismatch;
	public event Action OnGameComplete;

	public void CardRevealed( Card card ) => OnCardRevealed?.Invoke(card);
	public void Match( Card a , Card b ) => OnMatch?.Invoke(a , b);
	public void Mismatch( Card a , Card b ) => OnMismatch?.Invoke(a , b);
	public void GameComplete() => OnGameComplete?.Invoke();
}
