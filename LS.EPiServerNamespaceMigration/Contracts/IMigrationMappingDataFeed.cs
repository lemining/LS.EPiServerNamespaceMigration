using System.Linq;

namespace LS.EPiServerNamespaceMigration.Contracts
{
    public interface IMigrationMappingDataFeed
    {
        IQueryable<IMigrationMappingItem> GetMigrationMappingItems();
    }
}
