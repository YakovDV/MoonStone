public interface IStorage
{
    float MaxCapacity { get; }
    float CurrentResourcesMass {  get; }
    int CurrentResourcesValue { get; }

    bool TryAdd(ResourceConfig config);
    void Clear();
}