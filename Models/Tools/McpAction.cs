namespace GeneralMcpServer.Models.Tools
{
    public class McpAction
    {
        public string Type { get; set; } = "call";
        public string Method { get; set; } = "GET";
        public string Path { get; set; } = default!;
        public string Query { get; set; } = default!;
    }
}
