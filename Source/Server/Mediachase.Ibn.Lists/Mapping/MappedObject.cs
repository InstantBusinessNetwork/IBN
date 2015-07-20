using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.Lists.Mapping
{
    public class MetaObjectData
    {

        private Dictionary<String, object> _mapMetaFieldData;
        private Dictionary<String, object> _mapColumnData;
        private int? _primaryKey = null;

        public MetaObjectData()
        {
            _mapMetaFieldData = new Dictionary<string, object>();
            _mapColumnData = new Dictionary<string, object>();
        }

        public int? PrimaryKey
        {
            get
            {
                return _primaryKey;
            }

            set
            {
                _primaryKey = value;
            }
        }

        public Dictionary<String, object> MapColumnData
        {
            get
            {
                return _mapColumnData;
            }
        }

        public Dictionary<String, object> MapMetaFieldData
        {
            get
            {
                return _mapMetaFieldData;
            }
        }
    };

    public class MappedObject
    {
        private String _className;

        private readonly MappingElement _mapElColl;
        private List<MetaObjectData> _metaObjects;

        public MappedObject(String className, MappingElement mapElColl)
        {
            _className = className;
            _mapElColl = mapElColl;
            _metaObjects = new List<MetaObjectData>();
        }

        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        /// <value>The name of the class.</value>
        public String ClassName
        {
            get
            {
                return _className;
            }
        }

        /// <summary>
        /// Gets the map el coll.
        /// </summary>
        /// <value>The map el coll.</value>
        public MappingElement MapElColl
        {
            get { return _mapElColl; }
        }

        /// <summary>
        /// Gets the meta objets.
        /// </summary>
        /// <value>The meta objets.</value>
        public List<MetaObjectData> MetaObjets
        {
            get
            {
                return _metaObjects;
            }
        }

    };

}
