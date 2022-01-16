using Dataverse.Habr.Intro.DataverseBase;
using System.Text.Json.Serialization;

namespace Dataverse.Habr.Intro.Handlers;

public class LinesCount : IHandler
{
    public const int MaxCount = 10_000;

    public string Text => $"Slow sequential count (max {MaxCount})";

    public string Handle(ConnectionProvider connectionProvider)
    {
        try
        {
            var count = 0;
            var page = 0;
            string? pagingCookie = null;

            do
            {
                var result = connectionProvider.ProcessRequest(new Request((++page, pagingCookie)));
                pagingCookie = result.GetPagingCookie();
                count += result.Value.Length;
            } while (pagingCookie != null && count < MaxCount);

            return count.ToString();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private class Request : BaseRequest<Response[]>
    {
        private readonly (int, string?) _pageAndPagingCookie;

        public Request((int, string?) pageAndPagingCookie)
            => _pageAndPagingCookie = pageAndPagingCookie;

        public override HttpMethod HttpMethod => HttpMethod.Get;

        public override string? GetBody() => null;

        public override string GetRequest()
            => "mysch_mytables?fetchXml=" + GetFetchXml(_pageAndPagingCookie);

        private static string GetFetchXml((int, string?) pageAndPagingCookie)
        {
            var fetch = new FetchType
            {
                page = pageAndPagingCookie.Item1.ToString(),
                pagingcookie = pageAndPagingCookie.Item2,
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
                            }
                        }.ToArray()
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