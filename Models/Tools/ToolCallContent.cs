namespace GeneralMcpServer.Models.Tools
{
    public class ToolCallContent
    {
        public string Type { get; set; } = "actions";
        public McpAction[] Actions { get; set; } = [];
    }
}
