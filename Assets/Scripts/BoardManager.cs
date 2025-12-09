
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
public class BoardManager
{
	private GameManager gameManager;
	private List<UICard> cards = new List<UICard>();
	private List<UICard> flippedCards = new List<UICard>();

	public BoardManager( GameManager manager )
	{
		gameManager = manager;
	}

	public void CreateBoard( GameConfig config , List<int> matchedIndices = null )
	{
		ClearBoard();

		int totalCards = config.rows * config.cols;
		if (totalCards % 2 != 0) totalCards--; // Ensure even number

		List<CardData> selectedCards = SelectRandomCards(totalCards / 2);
		List<CardData> shuffledPairs = ShuffleCards(selectedCards);

		LayoutCards(config , shuffledPairs , matchedIndices);
	}

	private void ClearBoard()
	{
		foreach (UICard card in cards)
		{
			if (card != null && card.gameObject != null)
				Object.Destroy(card.gameObject);
		}
		cards.Clear();
		flippedCards.Clear();
	}

	private List<CardData> SelectRandomCards( int count )
	{
		List<CardData> available = new List<CardData>(gameManager.cardDataList);
		List<CardData> selected = new List<CardData>();

		count = Mathf.Min(count , available.Count);

		for (int i = 0; i < count; i++)
		{
			int index = Random.Range(0 , available.Count);
			selected.Add(available[index]);
			available.RemoveAt(index);
		}

		return selected;
	}

	private List<CardData> ShuffleCards( List<CardData> cardData )
	{
		List<CardData> pairs = new List<CardData>();
		foreach (var data in cardData)
		{
			pairs.Add(data);
			pairs.Add(data);
		}

		return pairs.OrderBy(x => Random.value).ToList();
	}

	private void LayoutCards( GameConfig config , List<CardData> cardData , List<int> matchedIndices )
	{
		RectTransform container = gameManager.boardContainer;
		if (container == null)
		{
			//Debug.LogError("Board container is null!");
			return;
		}

		// Setup GridLayoutGroup component
		GridLayoutGroup gridLayout = container.GetComponent<GridLayoutGroup>();
		if (gridLayout == null)
		{
			gridLayout = container.gameObject.AddComponent<GridLayoutGroup>();
		}

		// Padding for container
		float padding = 20f;

		// Get actual runtime size of container
		float containerWidth = container.rect.width;
		float containerHeight = container.rect.height;

		float usableWidth = containerWidth - ( padding * 2 );
		float usableHeight = containerHeight - ( padding * 2 );

		// Total spacing
		float totalSpacingX = ( config.cols - 1 ) * config.cardSpacing;
		float totalSpacingY = ( config.rows - 1 ) * config.cardSpacing;

		// Calculate max card size
		float cellWidth = ( usableWidth - totalSpacingX ) / config.cols;
		float cellHeight = ( usableHeight - totalSpacingY ) / config.rows;

		// Square card size, clamped to min/max
		float minSize = 230f;   // Minimum size of card
		float maxSize = 250f;  // Maximum size of card
		float cardSize = Mathf.Clamp(Mathf.Min(cellWidth , cellHeight) , minSize , maxSize);

		// Configure GridLayoutGroup
		gridLayout.cellSize = new Vector2(cardSize , cardSize);
		gridLayout.spacing = new Vector2(config.cardSpacing , config.cardSpacing);
		gridLayout.padding = new RectOffset((int) padding , (int) padding , (int) padding , (int) padding);
		gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		gridLayout.constraintCount = config.cols;
		gridLayout.childAlignment = TextAnchor.MiddleCenter;
		gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
		gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;

		// Force layout rebuild before adding cards
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(container);

		// Create cards
		int index = 0;
		for (int row = 0; row < config.rows; row++)
		{
			for (int col = 0; col < config.cols; col++)
			{
				if (index >= cardData.Count) break;

				UICard card = CreateCard(cardData[index] , index);

				if (matchedIndices != null && matchedIndices.Contains(index))
				{
					card.SetMatched();
				}

				cards.Add(card);
				index++;
			}
		}

		LayoutRebuilder.ForceRebuildLayoutImmediate(container);
	}

	private UICard CreateCard( CardData data , int index )
	{
		GameObject cardObj = Object.Instantiate(gameManager.cardPrefab , gameManager.boardContainer);
		UICard card = cardObj.GetComponent<UICard>();

		if (card == null)
		{
			card = cardObj.AddComponent<UICard>();
		}

		// GridLayoutGroup handles positioning, we just need to initialize
		card.Initialize(data , index , this);
		return card;
	}

	public void OnCardClicked( UICard card )
	{
		if (card.IsMatched || card.IsFlipped) return;

		card.Flip(true);
		gameManager.OnCardFlipped();
		flippedCards.Add(card);

		if (flippedCards.Count == 2)
		{
			gameManager.StartCoroutine(CheckMatch());
		}
	}

	private IEnumerator CheckMatch()
	{
		UICard card1 = flippedCards[0];
		UICard card2 = flippedCards[1];

		flippedCards.Clear();

		yield return new WaitForSeconds(0.8f);

		if (card1.Data.id == card2.Data.id)
		{
			card1.SetMatched();
			card2.SetMatched();
			gameManager.OnCardsMatched();
		}
		else
		{
			card1.Flip(false);
			card2.Flip(false);
			gameManager.OnCardsMismatched();
		}
	}

	public bool AllCardsMatched()
	{
		return cards.All(c => c.IsMatched);
	}

	public List<int> GetMatchedCardIndices()
	{
		return cards.Where(c => c.IsMatched).Select(c => c.CardIndex).ToList();
	}
}