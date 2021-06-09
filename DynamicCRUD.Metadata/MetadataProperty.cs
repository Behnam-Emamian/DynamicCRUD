namespace DynamicCRUD.Metadata
{
    public class MetadataProperty
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string ColumnName { get; set; }

        public bool IsNavigation { get; set; }

        public string NavigationType { get; set; } = "Single";
    }
}