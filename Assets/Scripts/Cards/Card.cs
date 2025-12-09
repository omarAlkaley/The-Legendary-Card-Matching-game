using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Card : MonoBehaviour, IPointerClickHandler
{
	public int Id { get; private set; }
	public bool IsRevealed { get; private set; } = false;
	public bool IsLocked { get; private set; } = false; // matched permanently

	[Header("Visuals")]
	[SerializeField] private Image frontImage;
	[SerializeField] private Sprite backSprite;

	Animator animator;

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	// Initialize with id and sprite
	public void Initialize( int id , Sprite front )
	{
		Id = id;
		if (frontImage != null) frontImage.sprite = front;
		IsRevealed = false;
		IsLocked = false;
		// ensure closed pose
		animator.Play("Closed" , 0 , 1f);
	}

	public void OnPointerClick( PointerEventData eventData )
	{
		if (IsLocked) return;
		if (IsRevealed) return;

		// Visual flip happens immediately (A2)
		RevealVisual();
		GameManager.Instance.RegisterVisualFlip(this);
	}

	// Visual reveal (immediate)
	public void RevealVisual()
	{
		if (IsRevealed || IsLocked) return;
		IsRevealed = true;
		animator.SetTrigger("FlipOpen");
		GameManager.Instance.EventManager.PlayFlipSFX();
	}

	// Visual close (can be called later)
	public void CloseVisual()
	{
		if (!IsRevealed || IsLocked) return;
		IsRevealed = false;
		animator.SetTrigger("FlipClose");
	}

	// Permanently lock as matched and play match animation
	public void SetMatched()
	{
		if (IsLocked) return;
		IsLocked = true;
		animator.SetTrigger("Match");
		GameManager.Instance.EventManager.PlayMatchSFX();
	}

	// Helper to force closed instantly (used on load)
	public void ForceClosed()
	{
		IsRevealed = false;
		animator.Play("Closed" , 0 , 1f);
	}
}