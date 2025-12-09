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
	//public Button changeConfigButton;
	[SerializeField] TMP_Dropdown columnsCount;
	[SerializeField] TMP_Dropdown rowsCount;
	int columnsIndex= 2;
	int rowsIndex = 2;

	[Header("Configuration")]
	public GameConfig config;
	public List<CardData> cardDataList;

	[Header("Card Visuals")]
	public Sprite cardBackSprite;
	public Color cardBackColor = Color.blue;
	
	[Header("Effects")]
	public TextMeshProUGUI comboText;

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
		rowsCount.onValueChanged.AddListener(onRowsCountChanged);
		columnsCount.onValueChanged.AddListener(onColumnsCountChanged);
		// Check if config has changed from saved game
		SaveData savedData = saveLoadManager.Load();
		bool configChanged = false;

		if (savedData != null)
		{
			configChanged = ( savedData.rows != config.rows || savedData.cols != config.cols );

			if (configChanged)
			{
				//Debug.Log($"Config changed! Saved: {savedData.rows}x{savedData.cols}, Current: {config.rows}x{config.cols}");
				//Debug.Log("Starting new game with current config...");
				saveLoadManager.DeleteSave(); // Only delete if config doesn't match
				StartNewGame();
			}
			else
			{
				// Config matches, load the saved game
				LoadGame();
			}
		}
		else
		{
			// No saved game, start new
			StartNewGame();
		}
	}

	public void StartNewGame()
	{
		config.rows = rowsIndex;
		config.cols = columnsIndex;

		scoreManager.ResetScore();
		UpdateUI();
		boardManager.CreateBoard(config);
	}

	void onRowsCountChanged( int index )
	{
		rowsIndex = index + 2;
		ApplyConfig();
	}

	void onColumnsCountChanged( int index )
	{
		columnsIndex = index + 2;
		ApplyConfig();
	}

	private void ApplyConfig()
	{
		config.rows = rowsIndex;
		config.cols = columnsIndex;

		SaveGame();

		boardManager.CreateBoard(config);
	}

	public void SaveGame()
	{
		SaveData data = new SaveData
		{
			score = scoreManager.Score ,
			moves = scoreManager.Moves ,
			matchedCardIndices = boardManager.GetMatchedCardIndices() ,
			rows = rowsIndex,
			cols = columnsIndex
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
		int previousStreak = scoreManager.GetCurrentStreak();
		scoreManager.AddStreakMatch(100);
		UpdateUI();
		PlaySound(matchSound);
		
		int newStreak = scoreManager.GetCurrentStreak();
		if (newStreak >= 2)
		{
			ShowComboText(newStreak);
		}

		SaveGame();

		if (boardManager.AllCardsMatched())
		{
			PlaySound(gameOverSound);
		}
	}

	public void OnCardsMismatched()
	{
		scoreManager.ResetStreak();
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
	public void OnConfigChanged()
	{
		saveLoadManager.DeleteSave(); // Clear old save that doesn't match
		StartNewGame();
	}

	public void ShowComboText( int streak )
	{
		if (comboText == null) return;

		string comboMsg = "";

		switch (streak)
		{
			case 2:
				comboMsg = "Combo x2! +50";
				break;
			case 3:
				comboMsg = "Combo x3! +100";
				break;
			default:
				comboMsg = $"Combo x{streak}! +150";
				break;
		}

		comboText.text = comboMsg;

		// Reset scale & alpha
		comboText.transform.localScale = Vector3.zero;
		comboText.GetComponent<CanvasGroup>().alpha = 1f;

		// Scale up then down
		LeanTween.scale(comboText.gameObject , Vector3.one * 1.3f , 0.2f)
			.setEaseOutBack()
			.setOnComplete(() =>
			{
				LeanTween.scale(comboText.gameObject , Vector3.one , 0.2f).setEaseInOutQuad();
			});

		// Fade out after delay
		CanvasGroup canvasGroup = comboText.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 1f;

		LeanTween.value(canvasGroup.gameObject , 1f , 0f , 0.5f)
			.setDelay(0.8f)
			.setOnUpdate(( float value ) =>
			{
				canvasGroup.alpha = value;
			});
		LeanTween.alphaText(comboText.rectTransform , 0f , 0.5f).setDelay(0.8f);
	}
}