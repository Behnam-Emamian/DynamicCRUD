using System;
using System.Collections.Generic;

namespace DynamicCRUD.Metadata
{
    public class MetadataEntity
    {
        public string Name { get; set; }

        public string TableName { get; set; }

        public string SchemaName { get; set; }

        public Type EntityType { get; set; }

        public List<MetadataProperty> Properties { get; set; }
    }
}