namespace InScale.Contracts.Settings
{
    public interface IDbContainer
    {
        string Name { get; }

        string PartitionKey { get; }
    }
}
