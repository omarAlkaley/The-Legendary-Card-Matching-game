public interface ICard
{
	int Id { get; }
	bool IsRevealed { get; }
	void Reveal();
	void Hide();
}