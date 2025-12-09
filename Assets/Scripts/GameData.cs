
[System.Serializable]
public class GameData
{
	public int rows;
	public int cols;
	public int[] cardIds;  // length = rows*cols
	public bool[] matched; // same order
	public int score;
}