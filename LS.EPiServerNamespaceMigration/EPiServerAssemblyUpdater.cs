using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using LS.EPiServerNamespaceMigration.Contracts;

namespace LS.EPiServerNamespaceMigration
{
    public class EPiServerAssemblyUpdater
    {
        EPiServerEntities _db = new EPiServerEntities();

        // Some of the tables will be fully kept in memory to stop rescanning database and putting massive load on it.
        // NOTE: Make sure that your machine has enough ram to keep table in the memory.
        #region Stores

        private IEnumerable<tblBigTable> _bigTable;

        public IEnumerable<tblBigTable> BigTable
        {
            set
            {
                _bigTable = value;
            }

            get
            {
                if (_bigTable == null)
                {
                    _bigTable = _db.tblBigTables.Select(w=>w).ToArray();
                }

                return _bigTable;
            }
        }

        private IEnumerable<tblBigTableReference> _bigTableRefrReferences;

        public IEnumerable<tblBigTableReference> BigTableRefrReferences
        {
            set
            {
                _bigTableRefrReferences = value;
            }

            get
            {
                if (_bigTableRefrReferences == null)
                {
                    _bigTableRefrReferences = _db.tblBigTableReferences.Select(w => w).ToArray();
                }

                return _bigTableRefrReferences;
            }
        }

        private IEnumerable<tblBigTableStoreInfo> _bigTableStoreInfo;

        public IEnumerable<tblBigTableStoreInfo> BigTableStoreInfo
        {
            set
            {
                _bigTableStoreInfo = value;
            }

            get
            {
                if (_bigTableStoreInfo == null)
                {
                    _bigTableStoreInfo = _db.tblBigTableStoreInfoes.Select(w => w).ToArray();
                }

                return _bigTableStoreInfo;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EPiServerAssemblyUpdater"/> class.
        /// </summary>
        public EPiServerAssemblyUpdater()
        {
            _db = new EPiServerEntities();
            _db.SetCommandTimeout(600);
            _db.Configuration.LazyLoadingEnabled = false;
        }

        public void StartTransaction()
        {
            // Not implemented
        }

        /// <summary>
        /// Tests the connection.
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            return _db.Database.Exists();
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void CommitTransaction()
        {
            _db.SaveChanges();
        }

        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        public void CancelTransaction()
        {
            _db = new EPiServerEntities();
        }

        /// <summary>
        /// Updates all tables.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateAllTables(IMigrationMappingItem item)
        {
            LoggerHelper.Log("Updating tblBigTable");
            UpdateBigTable(item);

            LoggerHelper.Log("Updating tblBigTableReference");
            UpdateBigTableReference(item);

            LoggerHelper.Log("Updating tblPageTypeDefinition");
            UpdatePageDefinitionTypes(item);

            LoggerHelper.Log("Updating tblPlugin");
            UpdatePlugin(item);

            LoggerHelper.Log("Updating tblBigTableStoreInfos");
            // Last one because it uses update not usual EF DbSet
            UpdateBigTableStoreInfos(item);

            //LoggerHelper.Log("Updating views");
            //UpdateViews(item);
        }

        /// <summary>
        /// Updates the big table.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateBigTable(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems =
                    BigTable.Where(
                        w =>
                            (w.StoreName != null && w.StoreName.Contains(item.OldFullAssemblyPath)) ||
                            (w.ItemType != null && w.ItemType.Contains(item.OldFullAssemblyPath)));
                //LoggerHelper.Log(string.Format("Updating {0} items in tblBigTable.", epiItems.Count()));
                foreach (var epiItem in epiItems)
                {
                    if (epiItem != null)
                    {
                        // Update item
                        epiItem.StoreName = ReplaceAssemblyNames(item, epiItem.StoreName);
                        epiItem.ItemType = ReplaceAssemblyNames(item, epiItem.ItemType);

                        LoggerHelper.Log(string.Format("Item: {0},{1}", epiItem.StoreName, epiItem.ItemType));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the big table reference.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateBigTableReference(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems =
                    BigTableRefrReferences.Where(
                        w =>
                            (w.CollectionType != null && w.CollectionType.Contains(item.OldFullAssemblyPath)) ||
                            (w.ElementType != null && w.ElementType.Contains(item.OldFullAssemblyPath)));
                //LoggerHelper.Log(string.Format("Updating {0} items in tblBigTable.", epiItems.Count()));
                foreach (var epiItem in epiItems)
                {
                    if (epiItem != null)
                    {
                        // Update item
                        epiItem.CollectionType = ReplaceAssemblyNames(item, epiItem.CollectionType);
                        epiItem.ElementType = ReplaceAssemblyNames(item, epiItem.ElementType);

                        LoggerHelper.Log(string.Format("Item: {0},{1}", epiItem.CollectionType, epiItem.ElementType));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the big table store infos.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdateBigTableStoreInfos(IMigrationMappingItem item)
        { 
            if (item != null)
            {
                var epiItems =
                    BigTableStoreInfo.Where(
                        w =>
                            (w.StoreName != null && w.StoreName.Contains(item.OldFullAssemblyPath)) ||
                            (w.PropertyType != null && w.PropertyType.Contains(item.OldFullAssemblyPath)));
                foreach (var epiItem in epiItems)
                {

                    _db.Database.ExecuteSqlCommand(
                        "update [dbo].[tblBigTableStoreInfo] set StoreName = {0} WHERE StoreName = {1}",
                        ReplaceAssemblyNames(item, epiItem.StoreName), epiItem.StoreName
                        );

                    LoggerHelper.Log(string.Format("Item: {0}", epiItem.StoreName));
                }
            }
        }

        /// <summary>
        /// Updates the page definition types.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdatePageDefinitionTypes(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems =
                    _db.tblPageDefinitionTypes.Where(
                        w =>
                            (w.TypeName != null && w.TypeName.Contains(item.OldFullAssemblyPath)) ||
                            (w.AssemblyName != null && w.AssemblyName.Contains(item.OldFullAssemblyPath)));
                foreach (var epiItem in epiItems)
                {
                    if (epiItem != null)
                    {
                        // Update item
                        epiItem.TypeName = ReplaceAssemblyNames(item, epiItem.TypeName);
                        epiItem.AssemblyName = ReplaceAssemblyNames(item, epiItem.AssemblyName);

                        LoggerHelper.Log(string.Format("Item: {0},{1}", epiItem.TypeName, epiItem.AssemblyName));
                    }
                }
            }
        }

        /// <summary>
        /// Updates the plugin.
        /// </summary>
        /// <param name="item">The item.</param>
        public void UpdatePlugin(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems =
                    _db.tblPlugIns.Where(
                        w =>
                            (w.TypeName != null && w.TypeName.Contains(item.OldFullAssemblyPath)) ||
                            (w.AssemblyName != null && w.AssemblyName.Contains(item.OldFullAssemblyPath)));
                foreach (var epiItem in epiItems)
                {
                    if (epiItem != null)
                    {
                        // Update item
                        epiItem.TypeName = ReplaceAssemblyNames(item, epiItem.TypeName);
                        epiItem.AssemblyName = ReplaceAssemblyNames(item, epiItem.AssemblyName);

                        LoggerHelper.Log(string.Format("Item: {0},{1}", epiItem.TypeName, epiItem.AssemblyName));
                    }
                }
            }
        }

        #region not used
        public void UpdateViews(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems = _db.Database.SqlQuery<string>(string.Format("select Name from sys.views where Name like '%{0}%'", item.OldFullAssemblyPath));
                foreach (var epiItem in epiItems)
                {
                    string updatedView;
                    if (epiItem != null)
                    {
                        // Update item
                        updatedView = ReplaceAssemblyNames(item, epiItem);
                        updatedView = ReplaceAssemblyNames(item, updatedView);

                        _db.Database.ExecuteSqlCommand(
                        string.Format("RENAME VIEW '{0}' TO '{1}'",
                        epiItem, updatedView)
                        );

                        LoggerHelper.Log(string.Format("Item: {0} to {1}", epiItem, updatedView));
                    }
                }
            }
        }

        public void UpdateStoredProcedures(IMigrationMappingItem item)
        {
            if (item != null)
            {
                var epiItems = _db.Database.SqlQuery<string>(string.Format("select SPECIFIC_NAME from information_schema.routines where routine_type = 'PROCEDURE' where SPECIFIC_NAME like '%{0}%'", item.OldFullAssemblyPath));
                foreach (var epiItem in epiItems)
                {
                    string updatedSPName;
                    if (epiItem != null)
                    {
                        // Update item
                        updatedSPName = ReplaceAssemblyNames(item, epiItem);
                        updatedSPName = ReplaceAssemblyNames(item, updatedSPName);

                        _db.Database.ExecuteSqlCommand(
                        "EXEC sp_rename {0} {1}",
                        epiItem, updatedSPName
                        );

                        LoggerHelper.Log(string.Format("Item: {0} to {1}", epiItem, updatedSPName));
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Replaces the assembly names.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="toBeAdjusted">To be adjusted.</param>
        /// <returns></returns>
        private string ReplaceAssemblyNames(IMigrationMappingItem item, string toBeAdjusted)
        {
            toBeAdjusted = toBeAdjusted.Replace(item.OldFullAssemblyPath, item.NewFullAssemblyPath);
            toBeAdjusted = toBeAdjusted.Replace(item.OldNamespace, item.NewNamespace);

            return toBeAdjusted;
        }
    }

    /// <summary>
    /// This is an extension found on StackOverflow that allows for overriding CommandTimeout (since our tables have over million rows it could timeout while pulling them)
    /// </summary>
    public static class EF
    {
        public static DbContext SetCommandTimeout(this DbContext db, TimeSpan? timeout)
        {
            ((IObjectContextAdapter)db).ObjectContext.CommandTimeout = timeout.HasValue ? (int?)timeout.Value.TotalSeconds : null;

            return db;
        }

        public static DbContext SetCommandTimeout(this DbContext db, int seconds)
        {
            return db.SetCommandTimeout(TimeSpan.FromSeconds(seconds));
        }
    }
}
