using Dataverse.Habr.Intro.DataverseBase;
using System.Text.Json.Serialization;

namespace Dataverse.Habr.Intro.Handlers;

public class Paging : IHandler
{
    public string Text => "Count of rows on the second page";

    public string Handle(ConnectionProvider connectionProvider)
    {
        try
        {
            var result = connectionProvider.ProcessRequest(new Request());
            return result.Value.Length.ToString();
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
            var innerJoin = new FetchLinkEntityType
            {
                name = "mysch_relatedtable_mysch_mytable",
                from = "mysch_myid",
                to = "mysch_mytableid",
                Items = new object[]
                {
                new FetchLinkEntityType
                {
                    name = "mysch_relatedtable",
                    from = "mysch_relatedtableid",
                    to = "mysch_relatedtableid",
                    Items = new object[]
                    {
                        new FetchAttributeType
                        {
                            name = "mysch_filesize",
                            aggregate = AggregateType.min,
                            aggregateSpecified = true,
                            alias = "mysch_filesize_Min"
                        }
                    }
                }
                }
            };

            var items = new object[]
            {
            new FetchAttributeType
            {
                name = "mysch_mytype",
                groupby = FetchBoolType.@true,
                groupbySpecified = true,
                alias = "mysch_mytype"
            },
            new FetchAttributeType
            {
                name = "mysch_myid",
                aggregate = AggregateType.count,
                aggregateSpecified = true,
                alias = "mysch_myid_Count"
            },
            new FetchAttributeType
            {
                name = "createdon",
                aggregate = AggregateType.max,
                aggregateSpecified = true,
                alias = "createdon_Max"
            },
            new filter
            {
                Items = new object[]
                {
                    new condition
                    {
                        attribute = "createdon",
                        @operator = @operator.ge,
                        value = "2021-03-06"
                    },
                    new condition
                    {
                        attribute = "createdon",
                        @operator = @operator.lt,
                        value = "2021-03-07"
                    }
                }
            },
            innerJoin,
            new FetchOrderType
            {
                alias = "createdon_Max",
                descending = true
            }
            };

            var fetch = new FetchType
            {
                page = "2",
                count = "3",
                aggregate = true,
                aggregateSpecified = true,
                Items = new object[]
                {
                new FetchEntityType
                {
                    name = "mysch_mytable",
                    Items = items
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