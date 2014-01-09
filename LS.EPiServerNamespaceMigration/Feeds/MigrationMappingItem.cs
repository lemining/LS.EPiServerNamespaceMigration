using LINQtoCSV;
using LS.EPiServerNamespaceMigration.Contracts;

namespace LS.EPiServerNamespaceMigration.Feeds
{
    public class MigrationMappingItem : IMigrationMappingItem
    {
        
        private string _oldFullAssemblyPath;

        [CsvColumn(Name = "Old Path", FieldIndex = 1)]
        public string OldFullAssemblyPath
        {
            get
            {
                return _oldFullAssemblyPath;
            }
            set
            {
                if (value != null)
                {
                    _oldFullAssemblyPath = value;
                }
            }
        }

        
        private string _oldNamespace;

        [CsvColumn(Name = "Old Assembly", FieldIndex = 2)]
        public string OldNamespace
        {
            get
            {
                return _oldNamespace;
            }
            set
            {
                if (value != null)
                {
                    _oldNamespace = value;
                }
            }
        }

        
        private string _newFullAssemblyPath;

        [CsvColumn(Name = "New Path", FieldIndex = 3)]
        public string NewFullAssemblyPath
        {
            get
            {
                return _newFullAssemblyPath;
            }
            set
            {
                if (value != null)
                {
                    _newFullAssemblyPath = value;
                }
            }
        }

        
        private string _newNamespace;

        [CsvColumn(Name = "New Assembly", FieldIndex = 4)]
        public string NewNamespace
        {
            get
            {
                return _newNamespace;
            }
            set
            {
                if (value != null)
                {
                    _newNamespace = value;
                }
            }
        }
    }
}
