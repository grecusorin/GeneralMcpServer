# McpServer (development mock)

This is a minimal MCP mock server used for local development and testing.

Purpose
- Provide a lightweight MCP-compatible endpoint for the Api project to call during development.
- Expose `/generate` (main endpoint) and `/tools` (metadata) that match the `McpResponse`/`McpAction` JSON shape used by `Api/Services/Mcp/McpModels.cs`.

Run
- From the solution root (or inside `McpServer`):
  - `dotnet run --project McpServer/McpServer.csproj`
- In Visual Studio: set `McpServer` as a startup project (profile `McpServer`) or use multiple startup projects to run `Api` + `McpServer` together.

Configuration
- The mock listens on `http://localhost:8080`. Api dev settings already point to this URL in `Api/appsettings.Development.json`:
  ```json
  "Mcp": { "BaseUrl": "http://localhost:8080", "GeneratePath": "/generate" }
  ```

Endpoints
- POST `/generate` — request body: `{ "conversationId": "...", "message": "...", "context": "..." }`.
  - Response example: `{ "text": "...", "actions": [ { "type":"call","method":"GET","path":"/odata/SimpleSearch","query":"q=..." } ] }`
- GET `/tools` — returns a short JSON list of available tool descriptions for development.

Notes
- This server is a development mock only. It intentionally returns controlled actions and sample text. The Api enforces a whitelist in `AiChatService` so the mock cannot trigger arbitrary internal paths.
- The DTO contract used by the Api is in `Api/Services/Mcp/McpModels.cs`. Keep changes compatible if you modify the mock responses.

Extending
- To simulate real OData responses, update `Program.cs` to return realistic JSON payloads for the `/odata/*` actions or enhance `/generate` heuristics.
- For integration tests, consider starting both apps in-memory and asserting flows end-to-end.
