public interface IMineable
{
    int Tier { get; }
    public bool TryMine(int damage);
}