using GeneralMcpServer.Models.Schema;
using GeneralMcpServer.Models.Tools;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace GeneralMcpServer.ToolHandlers;

    public static class InmateToolHandlers
    {
        public static List<McpTool> GetMcpTools()
        {
            return new List<McpTool>
            {
                new McpTool {
                    Name = "inmate_simple_search",
                    Title = "Inmate Simple Search",
                    Description = "Search for inmates by first name, last name, parents name or unique identifier number.",
                    InputSchema = new JsonSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, JsonSchemaProperty>
                        {
                            ["q"] = new JsonSchemaProperty { Type = "string" },
                            ["top"] = new JsonSchemaProperty { Type = "integer" }
                        },
                        Required = new [] { "q" }
                    }
                },

                new McpTool {
                    Name = "inmate_get_details",
                    Title = "Get inmate details",
                    Description = "Retrieve full information about the inmate.",
                    InputSchema = new JsonSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, JsonSchemaProperty>
                        {
                            ["id"] = new JsonSchemaProperty { Type = "integer" }
                        },
                        Required = new [] { "id" }
                    }
                },

                new McpTool {
                    Name = "inmate_get_convictions",
                    Title = "Get convictions",
                    Description = "Retrieve conviction records for the inmate.",
                    InputSchema = new JsonSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, JsonSchemaProperty>
                        {
                            ["id"] = new JsonSchemaProperty { Type = "integer" }
                        },
                        Required = new [] { "id" }
                    }
                },

                new McpTool {
                    Name = "inmate_get_visits",
                    Title = "Get visits",
                    Description = "Retrieve visit history for the inmate.",
                    InputSchema = new JsonSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, JsonSchemaProperty>
                        {
                            ["id"] = new JsonSchemaProperty { Type = "integer" }
                        },
                        Required = new [] { "id" }
                    }
                }
            };
        }

    [McpServerTool(Name = "inmate_simple_search")]
    [Description("Search for inmates by first name, last name, parents name or unique identifier number.")]
    public static ToolCallResult InmateSimpleSearchHandler(JsonElement toolArgs)
    {
        string queryText = toolArgs.GetProperty("q").GetString()!;
        int top = toolArgs.TryGetProperty("top", out var topElement)
            ? topElement.GetInt32()
            : 10;
        return new ToolCallResult
        {
            Content = new[]
            {
                new ToolCallContent
                {
                    Type = "actions",
                    Actions = new[]
                    {
                        new McpAction
                        {
                            Type = "call",
                            Method = "GET",
                            Path = "/odata/SimpleSearch",
                            Query = $"q={Uri.EscapeDataString(queryText)}&top={top}"
                        }
                    }
                }
            }
        };
    }

    [McpServerTool(Name = "inmate_get_details")]
    [Description("Retrieve full information about the inmate.")]
    public static ToolCallResult InmateGetDetailsHandler(JsonElement toolArgs)
    {
        int id = toolArgs.GetProperty("id").GetInt32();
        return new ToolCallResult
        {
            Content = new[]
            {
                new ToolCallContent
                {
                    Type = "actions",
                    Actions = new[]
                    {
                        new McpAction
                        {
                            Type = "call",
                            Method = "GET",
                            Path = "/odata/CSDetentionPerson",
                            Query = $"$filter=Id eq {id}"
                        }
                    }
                }
            }
        };
    }

    [McpServerTool(Name = "inmate_get_convictions")]
    [Description("Retrieve conviction records for the inmate.")]
    public static ToolCallResult InmateGetConvictionsHandler(JsonElement toolArgs)
    {
        int id = toolArgs.GetProperty("id").GetInt32();
        return new ToolCallResult
        {
            Content = new[]
            {
                new ToolCallContent
                {
                    Type = "actions",
                    Actions = new[]
                    {
                        new McpAction
                        {
                            Type = "call",
                            Method = "GET",
                            Path = "/odata/CSConviction",
                            Query = $"detentionPersonId={id}"
                        }
                    }
                }
            }
        };
    }

    [McpServerTool(Name = "inmate_get_visits")]
    [Description("Retrieve visit history for the inmate.")]
    public static ToolCallResult InmateGetVisitsHandler(JsonElement toolArgs)
    {
        int id = toolArgs.GetProperty("id").GetInt32();
        return new ToolCallResult
        {
            Content = new[]
            {
                new ToolCallContent
                {
                    Type = "actions",
                    Actions = new[]
                    {
                        new McpAction
                        {
                            Type = "call",
                            Method = "GET",
                            Path = "/odata/CSVisit",
                            Query = $"detentionPersonId={id}"
                        }
                    }
                }
            }
        };
    }

    public static ToolCallResult ExecuteTool(string toolName, JsonElement toolArgs)
    {
        switch (toolName)
        {
            case "inmate_simple_search":
                return InmateSimpleSearchHandler(toolArgs);

            case "inmate_get_details":
                return InmateGetDetailsHandler(toolArgs);

            case "inmate_get_convictions":
                return InmateGetConvictionsHandler(toolArgs);

            case "inmate_get_visits":
                return InmateGetVisitsHandler(toolArgs);

            default:
                throw new InvalidOperationException($"Unknown MCP tool '{toolName}'.");
        }
    }
}
