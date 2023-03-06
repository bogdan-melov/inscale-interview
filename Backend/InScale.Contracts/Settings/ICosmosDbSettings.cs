namespace InScale.Contracts.Settings
{
    public interface ICosmosDbSettings
    {
        string ConnectionString { get; }

        string DatabaseName { get; }

        FileDbContainer FileContainer { get; }
    }
}
