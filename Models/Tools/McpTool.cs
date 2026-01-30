using GeneralMcpServer.Models.Schema;

namespace GeneralMcpServer.Models.Tools
{
    public class McpTool
    {
        public string Name { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public JsonSchema InputSchema { get; set; } = default!;
    }
}
