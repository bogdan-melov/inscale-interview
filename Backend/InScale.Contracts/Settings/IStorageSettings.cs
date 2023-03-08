namespace InScale.Contracts.Settings
{
    public interface IStorageSettings
    {
        string ConnectionString { get; }

        string ContainerName { get; }
    }
}
