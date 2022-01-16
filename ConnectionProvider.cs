using Dataverse.Habr.Intro.DataverseBase;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Dataverse.Habr.Intro;

public class ConnectionProvider
{
    private readonly HttpClient _httpClient;

    public ConnectionProvider(HttpClient httpClient) => _httpClient = httpClient;

    public OdataResponse<T> ProcessRequest<T>(BaseRequest<T> baseRequest) where T : class
    {
        var requestUri = baseRequest.GetRequest();
        var content = baseRequest.GetBody();

        if (baseRequest.ReturnFormattedValues)
        {
            _httpClient.DefaultRequestHeaders.Add(
                "Prefer",
                "odata.include-annotations=OData.Community.Display.V1.FormattedValue");
        }

        var request = new HttpRequestMessage(baseRequest.HttpMethod, requestUri);

        if (content != null)
        {
            request.Content = new StringContent(content);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
        }

        var response = _httpClient.Send(request);

        var data = response.Content.ReadAsStream();

#if DEBUG
        var debugStringData = ReadResponseAsString(response);
        data.Position = 0;
#endif

        if (response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<OdataResponse<T>>(data)!;
        }

        var responseContent = ReadResponseAsString(response);

        var errorMessage = string.IsNullOrWhiteSpace(responseContent)
            ? $"Failed with a status of '{response.ReasonPhrase}'"
            : $"Failed with content: {responseContent.Replace("\"", string.Empty)}";

        throw new Exception(errorMessage);
    }

    private static string ReadResponseAsString(HttpResponseMessage message)
        => new StreamReader(message.Content.ReadAsStream()).ReadToEnd();
}