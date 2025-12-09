using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
	public int score;
	public int moves;
	public List<int> matchedCardIndices = new List<int>();
	public int rows;
	public int cols;
}
