using System;
using System.Linq;
using LS.EPiServerNamespaceMigration.Feeds;

namespace LS.EPiServerNamespaceMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting updating assemblies in EPiServer");


            var feed = new CsvMigrationMappingDataFeed("Data/test.csv");
            var updater = new EPiServerAssemblyUpdater();
            var items = feed.GetMigrationMappingItems();

            try
            {
                updater.TestConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine("Database connection failed.");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }

            if (items.Any())
            {
                try
                {
                    var itemsToBeChanged = items.Where(w => !w.OldFullAssemblyPath.Equals(w.NewFullAssemblyPath));

                    LoggerHelper.Log(string.Format("--- Found {0} items to be updated.", itemsToBeChanged.Count()));

                    for (int i = 0; i < itemsToBeChanged.Count(); i++)
                    {
                        // Namespaces have moved therefore we need to update data
                        updater.UpdateAllTables(itemsToBeChanged.ElementAt(i));
                        LoggerHelper.Log(string.Format("--- Updating {0} with {1}", itemsToBeChanged.ElementAt(i).OldFullAssemblyPath, itemsToBeChanged.ElementAt(i).NewFullAssemblyPath));
                    }


                    LoggerHelper.Log("--- Successfully updated all records. Saving to database...");
                    updater.CommitTransaction();

                    LoggerHelper.Log("--- Records saved");
                }
                catch (Exception e)
                {
                    LoggerHelper.Log("Failed to update all records. Reverting changes.");
                    LoggerHelper.Log(e.Message);
                    LoggerHelper.Log(e.InnerException.ToString());
                    updater.CancelTransaction();
                }
            }

            LoggerHelper.Log("Press any key to close.");
            Console.ReadKey();
        }
    }
}
