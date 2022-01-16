using Dataverse.Habr.Intro.DataverseBase;
using System.Text.Json.Serialization;

namespace Dataverse.Habr.Intro.Handlers;

public class FormattedValues : IHandler
{
    public string Text => "Formatted fields";

    public string Handle(ConnectionProvider connectionProvider)
    {
        try
        {
            var result = connectionProvider.ProcessRequest(new Request());
            var resultValue = result.Value[0];
            return $"{nameof(resultValue.PaymentTypeText)} - {resultValue.PaymentTypeText}, " +
                   $"{nameof(resultValue.StatusCodeText)} - {resultValue.StatusCodeText}";
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

        public override bool ReturnFormattedValues => true;

        public override string GetRequest() => "mysch_mytables?$select=mypaymenttype,mystatuscode&$top=1";
    }

    private class Response
    {
        [JsonPropertyName("mypaymenttype@OData.Community.Display.V1.FormattedValue")]
        public string PaymentTypeText { get; set; }

        [JsonPropertyName("mystatuscode@OData.Community.Display.V1.FormattedValue")]
        public string StatusCodeText { get; set; }
    }
}