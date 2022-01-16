using Dataverse.Habr.Intro.DataverseBase;
using System.Text.Json.Serialization;

namespace Dataverse.Habr.Intro.Handlers;

public class GetByFetchXml : IHandler
{
    public string Text => "The same with fetch xml";

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

        public override string GetRequest() => "mysch_mytables?fetchXml=" + GetFetchXml();

        private static string GetFetchXml()
        {
            var fetch = new FetchType
            {
                top = "3",
                Items = new object[]
                {
                new FetchEntityType
                {
                    name = "mysch_mytable",
                    Items = new object[]
                    {
                        new FetchAttributeType
                        {
                            name = "mysch_myid"
                        },
                        new filter
                        {
                            Items = new[]
                            {
                                new condition
                                {
                                    attribute = "mysch_mytype",
                                    @operator = @operator.ne,
                                    value = "null"
                                }
                            }.ToArray<object>()
                        }
                    }
                }
                }
            };

            var fetchXml = Serialize(fetch);

            return fetchXml;
        }
    }


    private class Response
    {
        [JsonPropertyName("mysch_myid")]
        public string Field { get; set; }
    }
}