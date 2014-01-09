namespace LS.EPiServerNamespaceMigration.Contracts
{
    public interface IMigrationMappingItem
    {
        string OldFullAssemblyPath { set; get; }
        string OldNamespace { set; get; }

        string NewFullAssemblyPath { set; get; }
        string NewNamespace { set; get; }
    }
}
