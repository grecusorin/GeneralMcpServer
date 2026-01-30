using System.ComponentModel;
using System.Net.Http.Headers;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace GeneralMcpServer.ToolHandlers;

[McpServerToolType]
public class InmateTools
{
    private static Uri _baseApiUri = new("https://localhost:7021");

    [McpServerTool(Name = "inmate_simple_search")]
    [Description(@"Search for inmates using basic identity attributes. All provided parameters are combined to filter the results. Only inmates matching all specified criteria are returned.")]
    public static async Task<string> SimpleSearch(
        [Description("First name of the inmate used as a search filter. If provided, only inmates with this first name are included.")]
        string firstName,
        [Description("Last name of the inmate used as a search filter. If provided, only inmates with this last name are included.")]
        string lastName,
        [Description("Date of birth of the inmate used as a search filter. Only inmates born on this date are included.")]
        DateOnly? birthDate,
        [Description("Optional filter for the inmate's father's last name. If null, this filter is not applied.")]
        string? fatherName = null,
        [Description("Optional filter for the inmate's mother's last name. If null, this filter is not applied.")]
        string? motherName = null,
        [Description("Optional filter for the inmate's unique identifier. If provided, only inmates with this identifier are included.")]
        string? uniqueIdentifier = null,
        CancellationToken cancellationToken = default)
    {
        var relativePath = "odata/SimpleSearch";
        var filters = new List<string>
        {
            $"contains(tolower(FirstName),'{firstName.ToLowerInvariant()}')",
            $"contains(tolower(LastName),'{lastName.ToLowerInvariant()}')"
        };
        if(birthDate.HasValue)
            filters.Add($"BirthDate eq {birthDate.Value:yyyy-MM-dd}");
        if (!string.IsNullOrEmpty(fatherName))
            filters.Add($"contains(tolower(FatherName),'{fatherName.ToLowerInvariant()}')");
        if (!string.IsNullOrEmpty(motherName))
            filters.Add($"contains(tolower(MotherName),'{motherName.ToLowerInvariant()}')");
        if (!string.IsNullOrEmpty(uniqueIdentifier))
            filters.Add($"UniqueIdentityNumber eq '{uniqueIdentifier.ToLowerInvariant()}'");

        var uriBuilder = new UriBuilder($"{_baseApiUri.AbsoluteUri}{relativePath}")
        {
            Query = $"$filter={string.Join(" and ", filters)}"
        };

        var request = uriBuilder.ToString();
        Console.WriteLine($"Request URI: {request}");

        using var client = new HttpClient();
        var response = await client.GetAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        return json;
    }

    [McpServerTool(Name = "inmate_get_details")]
    [Description("Retrieve full profile information for a specific inmate identified by a unique inmate identifier.")]
    public static async Task<string> GetDetails(
        [Description("Unique identifier of the inmate whose full details are requested. This parameter is required and must reference an existing inmate.")]
        long inmateId,
        CancellationToken cancellationToken = default)
    {
        return string.Empty;
    }


    [McpServerTool(Name = "inmate_get_convictions")]
    [Description(@"Retrieve conviction records associated with a specific inmate. 
        The tool can return all convictions for the inmate or a single conviction if a conviction identifier is provided.")]
    public static async Task<IEnumerable<ContentBlock>> GetConvictions(
        [Description("Unique identifier of the inmate whose conviction records are requested. This parameter is required and must reference an existing inmate.")]
        long inmateId,

        [Description(@"Optional filter to retrieve a specific conviction. 
        If provided, only the conviction with this identifier is returned. 
        If null, all convictions associated with the inmate are returned.")]
        long? convictionId = null,
        CancellationToken cancellationToken = default
    )
    {
        return [];
    }


    [McpServerTool(Name = "inmate_get_visits")]
    [Description(@"Retrieve a list of inmate visits filtered by inmate identifier and optional date range. 
    The tool returns only visits that belong to the specified inmate.
    Date filters are applied inclusively. If no end date is provided, visits are returned up to the current time.")]
    public static async Task<IEnumerable<ContentBlock>> GetVisits(
    [Description(@"Unique identifier of the inmate whose visits are requested. This parameter is required and must reference an existing inmate.")]
    long inmateId,

    [Description(@"Start of the visit date filter. Only visits that occurred on or after this date and time are included. 
    If null, no lower date limit is applied. The value is interpreted as UTC.")]
    DateTime? startDate,

    [Description(@"End of the visit date filter. Only visits that occurred on or before this date and time are included. 
    If null, the current date and time is used as the upper limit. The value is interpreted as UTC.")]
    DateTime? endDate = null,
    CancellationToken cancellationToken = default)
    {
        startDate ??= DateTime.MinValue;
        endDate ??= DateTime.Now;

        return [];
    }
}
