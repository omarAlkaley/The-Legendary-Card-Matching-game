using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CardFactory : MonoBehaviour
{
	[SerializeField] private GameObject cardPrefab;
	[SerializeField] private Transform boardParent;

	public List<Card> CreateCardPairs( int pairCount )
	{
		List<Card> cards = new List<Card>();

		for (int i = 0; i < pairCount; i++)
		{
			cards.Add(CreateCard(i));
			cards.Add(CreateCard(i));
		}

		// Shuffle
		cards = cards.OrderBy(c => Random.value).ToList();

		// Arrange on board
		for (int i = 0; i < cards.Count; i++)
			cards[i].transform.SetSiblingIndex(i);

		return cards;
	}

	private Card CreateCard( int id )
	{
		GameObject cardObj = Instantiate(cardPrefab , boardParent);
		var card = cardObj.GetComponent<Card>();
		card.Initialize(id);
		return card;
	}
}
