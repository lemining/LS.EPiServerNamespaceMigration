using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;
using LS.EPiServerNamespaceMigration.Contracts;

namespace LS.EPiServerNamespaceMigration.Feeds
{
    public class CsvMigrationMappingDataFeed : IMigrationMappingDataFeed
    {
        private CsvFileDescription _fileDescription;
        private CsvContext _context;
        private IQueryable<IMigrationMappingItem> _dataFeeds;

        private string _filePath;

        public CsvMigrationMappingDataFeed(string csvFilePath)
        {
            _fileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            _filePath = csvFilePath;
            _context = new CsvContext();
        }

        //FeedEntities _db = new FeedEntities();

        public IQueryable<IMigrationMappingItem> GetMigrationMappingItems()
        {
            if (_dataFeeds == null)
            {
                IEnumerable<MigrationMappingItem> feedItems =
                _context.Read<MigrationMappingItem>(_filePath, _fileDescription);

                _dataFeeds = feedItems.AsQueryable();
            }

            return _dataFeeds;
        }
    }
}
