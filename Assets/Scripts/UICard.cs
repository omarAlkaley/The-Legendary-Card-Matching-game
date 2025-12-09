using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerClickHandler
{
	private CardData data;
	private BoardManager boardManager;
	private ICardState currentState;
	private int cardIndex;

	private RectTransform rectTransform;
	private Image frontImage;
	private Image backImage;
	private Image cardImage;
	private GameObject front;
	private GameObject back;
	private CanvasGroup canvasGroup;

	public CardData Data => data;
	public bool IsFlipped { get; private set; }
	public bool IsMatched { get; private set; }
	public int CardIndex => cardIndex;

	public void Initialize( CardData cardData , int index , BoardManager manager )
	{
		data = cardData;
		boardManager = manager;
		cardIndex = index;

		SetupVisuals();
		currentState = new CardFaceDownState();
		currentState.Enter(this);
	}

	private void SetupVisuals()
	{
		rectTransform = GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			rectTransform = gameObject.AddComponent<RectTransform>();
		}

		canvasGroup = gameObject.AddComponent<CanvasGroup>();

		// Create back (blue card back) - THIS is now the clickable element
		back = new GameObject("Back");
		back.transform.SetParent(transform);
		RectTransform backRect = back.AddComponent<RectTransform>();
		backRect.anchorMin = Vector2.zero;
		backRect.anchorMax = Vector2.one;
		backRect.offsetMin = Vector2.zero;
		backRect.offsetMax = Vector2.zero;

		backImage = back.AddComponent<Image>();
		backImage.sprite = GameManager.Instance.cardBackSprite;
		backImage.color = GameManager.Instance.cardBackColor;
		backImage.raycastTarget = true; // ENABLE raycast on back - this catches clicks

		// Add click handler to back image
		Button backButton = back.AddComponent<Button>();
		backButton.transition = Selectable.Transition.None;
		backButton.onClick.AddListener(() => OnPointerClick(null));

		// Create front (card icon)
		front = new GameObject("Front");
		front.transform.SetParent(transform);
		RectTransform frontRect = front.AddComponent<RectTransform>();
		frontRect.anchorMin = Vector2.zero;
		frontRect.anchorMax = Vector2.one;
		frontRect.offsetMin = Vector2.zero;
		frontRect.offsetMax = Vector2.zero;

		frontImage = front.AddComponent<Image>();
		frontImage.sprite = data.icon;
		frontImage.preserveAspect = true;
		frontImage.raycastTarget = true; // ENABLE raycast on front too

		// Add click handler to front image
		Button frontButton = front.AddComponent<Button>();
		frontButton.transition = Selectable.Transition.None;
		frontButton.onClick.AddListener(() => OnPointerClick(null));

		// Add padding to front image
		frontRect.offsetMin = new Vector2(10 , 10);
		frontRect.offsetMax = new Vector2(-10 , -10);

		front.SetActive(false);
		back.SetActive(true);

		Debug.Log($"Card {cardIndex} setup - Back raycast: {backImage.raycastTarget}, Front raycast: {frontImage.raycastTarget}");
	}

	public void OnPointerClick( PointerEventData eventData )
	{
		Debug.Log($"Card {cardIndex} clicked!");
		currentState.OnClick(this);
	}

	public void Flip( bool faceUp )
	{
		IsFlipped = faceUp;

		if (faceUp)
		{
			currentState = new CardFaceUpState();
			AnimateFlip(true);
		}
		else
		{
			currentState = new CardFaceDownState();
			AnimateFlip(false);
		}

		currentState.Enter(this);
	}

	private void AnimateFlip( bool showFront )
	{
		// Scale X to 0 (flip to side view)
		LeanTween.scaleX(gameObject , 0f , 0.15f)
			.setEase(LeanTweenType.easeInQuad)
			.setOnComplete(() =>
			{
				// Switch sprites at the thinnest point
				front.SetActive(showFront);
				back.SetActive(!showFront);

				// Scale X back to 1 (flip back to front view)
				LeanTween.scaleX(gameObject , 1f , 0.15f)
					.setEase(LeanTweenType.easeOutQuad);
			});
	}

	public void SetMatched()
	{
		IsMatched = true;
		currentState = new CardMatchedState();
		currentState.Enter(this);

		// Pulse animation for matched cards
		LeanTween.scale(gameObject , Vector3.one * 1.1f , 0.2f)
			.setEase(LeanTweenType.easeOutQuad)
			.setOnComplete(() =>
			{
				LeanTween.scale(gameObject , Vector3.one , 0.2f)
					.setOnComplete(() =>
					{
						// Fade out matched cards
						LeanTween.alpha(rectTransform , 0.5f , 0.3f);
					});
			});
	}

	public void NotifyBoardManager()
	{
		boardManager?.OnCardClicked(this);
	}
}