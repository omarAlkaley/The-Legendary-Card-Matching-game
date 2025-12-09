using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
	private static GameManager _instance;
	public static GameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<GameManager>();
			}
			return _instance;
		}
	}

	[Header("UI References")]
	public RectTransform boardContainer;
	public GameObject cardPrefab;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI movesText;
	public Button newGameButton;

	[Header("Configuration")]
	public GameConfig config;
	public List<CardData> cardDataList;

	[Header("Card Visuals")]
	public Sprite cardBackSprite;
	public Color cardBackColor = Color.blue;

	[Header("Audio")]
	public AudioClip flipSound;
	public AudioClip matchSound;
	public AudioClip mismatchSound;
	public AudioClip gameOverSound;

	private AudioSource audioSource;
	private BoardManager boardManager;
	private ScoreManager scoreManager;
	private SaveLoadManager saveLoadManager;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;

		audioSource = gameObject.AddComponent<AudioSource>();
		boardManager = new BoardManager(this);
		scoreManager = new ScoreManager();
		saveLoadManager = new SaveLoadManager();

		if (newGameButton != null)
		{
			newGameButton.onClick.AddListener(StartNewGame);
		}
	}

	private void Start()
	{
		// Try to load saved game, otherwise start new
		if (!LoadGame())
		{
			StartNewGame();
		}
	}

	public void StartNewGame()
	{
		scoreManager.ResetScore();
		UpdateUI();
		boardManager.CreateBoard(config);
	}

	public void SaveGame()
	{
		SaveData data = new SaveData
		{
			score = scoreManager.Score ,
			moves = scoreManager.Moves ,
			matchedCardIndices = boardManager.GetMatchedCardIndices() ,
			rows = config.rows ,
			cols = config.cols
		};
		saveLoadManager.Save(data);
	}

	public bool LoadGame()
	{
		SaveData data = saveLoadManager.Load();
		if (data != null)
		{
			config.rows = data.rows;
			config.cols = data.cols;
			scoreManager.SetScore(data.score , data.moves);
			UpdateUI();
			boardManager.CreateBoard(config , data.matchedCardIndices);
			return true;
		}
		return false;
	}

	public void PlaySound( AudioClip clip )
	{
		if (clip != null && audioSource != null)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	public void OnCardsMatched()
	{
		scoreManager.AddScore(100);
		UpdateUI();
		PlaySound(matchSound);
		SaveGame();

		if (boardManager.AllCardsMatched())
		{
			PlaySound(gameOverSound);
			Debug.Log("Game Over! Final Score: " + scoreManager.Score);
		}
	}

	public void OnCardsMismatched()
	{
		PlaySound(mismatchSound);
		SaveGame();
	}

	public void OnCardFlipped()
	{
		scoreManager.IncrementMoves();
		UpdateUI();
		PlaySound(flipSound);
	}

	private void UpdateUI()
	{
		if (scoreText != null)
			scoreText.text = $"Score: {scoreManager.Score}";
		if (movesText != null)
			movesText.text = $"Moves: {scoreManager.Moves}";
	}
}