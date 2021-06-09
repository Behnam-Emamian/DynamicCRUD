using System.Collections.Generic;

namespace DynamicCRUD.Metadata
{
    public class MetadataConfig
    {
        public string Version { get; set; }
        public List<MetadataEntity> Entities { get; set; } = new List<MetadataEntity>();
    }
}