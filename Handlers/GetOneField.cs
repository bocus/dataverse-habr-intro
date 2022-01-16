using Dataverse.Habr.Intro.DataverseBase;
using System.Text.Json.Serialization;

namespace Dataverse.Habr.Intro.Handlers;

public class GetOneField : IHandler
{
    public string Text => "Three rows field value";

    public string Handle(ConnectionProvider connectionProvider)
    {
        try
        {
            var result = connectionProvider.ProcessRequest(new Request());
            return string.Join(", ", result.Value.Select(i => i.Field));
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private class Request : BaseRequest<Response[]>
    {
        public override HttpMethod HttpMethod => HttpMethod.Get;

        public override string? GetBody() => null;

        public override string GetRequest() => "mysch_mytables?$select=mysch_myid&$filter=mysch_mytype ne null&$top=3";
    }

    private class Response
    {
        [JsonPropertyName("mysch_myid")]
        public string Field { get; set; }
    }
}