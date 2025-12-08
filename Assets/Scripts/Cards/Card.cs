using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
	public int Id { get; private set; }
	public bool IsRevealed { get; private set; }

	public void Initialize( int id )
	{
		Id = id;
		HideInstant();
	}

	public void OnPointerClick( PointerEventData eventData )
	{
		if (!IsRevealed)
			GameManager.Instance.SelectCard(this);
	}

	public void Reveal()
	{
		IsRevealed = true;
		// Trigger flip animation
		GetComponent<Animator>().SetTrigger("FlipOpen");
	}

	public void Hide()
	{
		IsRevealed = false;
		// Trigger flip back animation
		GetComponent<Animator>().SetTrigger("FlipClose");
	}

	public void HideInstant()
	{
		IsRevealed = false;
		// instantly set closed state
	}
}
