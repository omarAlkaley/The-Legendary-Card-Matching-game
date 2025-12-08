public class ExactMatchStrategy : IMatchStrategy
{
	public bool IsMatch( Card a , Card b )
	{
		return a.Id == b.Id;
	}
}
