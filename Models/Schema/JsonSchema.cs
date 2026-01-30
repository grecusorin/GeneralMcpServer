namespace GeneralMcpServer.Models.Schema
{
    public class JsonSchema
    {
        public string Type { get; set; } = "object";
        public Dictionary<string, JsonSchemaProperty> Properties { get; set; } = [];
        public string[] Required { get; set; } = [];
    }
}
