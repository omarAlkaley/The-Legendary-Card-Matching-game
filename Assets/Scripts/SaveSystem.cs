using System.IO;
using UnityEngine;

public static class SaveSystem
{
	static string Path => System.IO.Path.Combine(Application.persistentDataPath , "memory_save.json");

	public static void Save( GameData data )
	{
		try
		{
			string json = JsonUtility.ToJson(data);
			File.WriteAllText(Path , json);
			Debug.Log("Saved game to: " + Path);
		}
		catch (System.Exception e) { Debug.LogError(e); }
	}

	public static GameData Load()
	{
		if (!File.Exists(Path)) return null;
		try
		{
			string json = File.ReadAllText(Path);
			return JsonUtility.FromJson<GameData>(json);
		}
		catch (System.Exception e) { Debug.LogError(e); return null; }
	}

	public static void DeleteSave()
	{
		if (File.Exists(Path)) File.Delete(Path);
	}
}