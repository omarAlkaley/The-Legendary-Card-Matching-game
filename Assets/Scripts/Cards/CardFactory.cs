using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class CardFactory : MonoBehaviour
{
	[Header("Prefab & Grid")]
	public GameObject cardPrefab;         // Card prefab with Card component
	public RectTransform container;       // UI panel for grid
	public GridLayoutGroup grid;          // GridLayoutGroup attached to container

	[Header("Sprites")]
	public Sprite[] frontSprites;         // unique sprites for pairs
	public Sprite backSprite;             // optional back sprite

	List<Card> created = new List<Card>();

	public List<Card> CreateCards( int rows , int cols , int[] ids )
	{
		Clear();
		ResizeCells(rows , cols);

		int total = rows * cols;
		for (int i = 0; i < total; i++)
		{
			GameObject go = Instantiate(cardPrefab , grid.transform);
			Card card = go.GetComponent<Card>();
			Sprite front = frontSprites.Length > 0 ? frontSprites[ids[i] % frontSprites.Length] : null;
			// set back sprite via card's inspector if needed
			card.Initialize(ids[i] , front);
			created.Add(card);
		}

		return new List<Card>(created);
	}

	// compute cell size to fit container
	public void ResizeCells( int rows , int cols )
	{
		var padding = grid.padding;
		float totalW = container.rect.width - padding.left - padding.right - grid.spacing.x * ( cols - 1 );
		float totalH = container.rect.height - padding.top - padding.bottom - grid.spacing.y * ( rows - 1 );
		float cellW = totalW / cols;
		float cellH = totalH / rows;
		float size = Mathf.Floor(Mathf.Min(cellW , cellH));
		grid.cellSize = new Vector2(size , size);
	}

	public void Clear()
	{
		foreach (Transform t in grid.transform) Destroy(t.gameObject);
		created.Clear();
	}
}
