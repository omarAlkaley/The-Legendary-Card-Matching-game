using UnityEngine;

public class SaveLoadManager
{
	private const string SAVE_KEY = "CardGameSave";

	public void Save( SaveData data )
	{
		string json = JsonUtility.ToJson(data);
		PlayerPrefs.SetString(SAVE_KEY , json);
		PlayerPrefs.Save();
		//Debug.Log("Game Saved");
	}

	public SaveData Load()
	{
		if (PlayerPrefs.HasKey(SAVE_KEY))
		{
			string json = PlayerPrefs.GetString(SAVE_KEY);
			SaveData data = JsonUtility.FromJson<SaveData>(json);
			//Debug.Log("Game Loaded");
			return data;
		}
		return null;
	}

	public void DeleteSave()
	{
		PlayerPrefs.DeleteKey(SAVE_KEY);
		PlayerPrefs.Save();
	}
}