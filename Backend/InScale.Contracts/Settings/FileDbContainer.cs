namespace InScale.Contracts.Settings
{
    public class FileDbContainer : IDbContainer
    {
        public string Name { get; }

        public string PartitionKey { get; }

        public FileDbContainer(string name, string partitionKey)
        {
            Name = name;
            PartitionKey = partitionKey;
        }
    }
}
