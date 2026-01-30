using System.Collections.Concurrent;
using GeneralMcpServer.Messages;
using GeneralMcpServer.ToolHandlers;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace GeneralMcpServer;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> subscriptions = new();

        var mcpServerOptions = new McpServerOptions()
        {
              ServerInfo = new Implementation() { Name="General MCP Server", Version="0.1.0"}
        };

        builder.Services.AddMcpServer(options => options = mcpServerOptions)
             .WithHttpTransport(options =>
                {
                    // Add a RunSessionHandler to remove all subscriptions for the session when it ends
                    options.RunSessionHandler = async (httpContext, mcpServer, cancellationToken) =>
                    {
                        if (mcpServer.SessionId == null)
                        {
                            // There is no sessionId if the serverOptions.Stateless is true
                            await mcpServer.RunAsync(cancellationToken);
                            return;
                        }
                        try
                        {
                            subscriptions[mcpServer.SessionId] = new ConcurrentDictionary<string, byte>();
                            // Start an instance of SubscriptionMessageSender for this session
                            using var subscriptionSender = new SubscriptionMessageSender(mcpServer, subscriptions[mcpServer.SessionId]);
                            await subscriptionSender.StartAsync(cancellationToken);
                            // Start an instance of LoggingUpdateMessageSender for this session
                            using var loggingSender = new LoggingUpdateMessageSender(mcpServer);
                            await loggingSender.StartAsync(cancellationToken);
                            await mcpServer.RunAsync(cancellationToken);
                        }
                        finally
                        {
                            // This code runs when the session ends
                            subscriptions.TryRemove(mcpServer.SessionId, out _);
                        }
                    };
                })
            .WithTools<InmateTools>();

        var app = builder.Build();
        app.MapMcp();

        await app.RunAsync();
    }
}