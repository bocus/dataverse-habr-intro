using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Dataverse.Habr.Intro.DataverseBase;

public class OdataResponse<T>
{
    private static readonly Regex CookieRegex = new ("pagingcookie=\"(.+?)\"", RegexOptions.Compiled);

    [JsonPropertyName("@odata.context")]
    public string OdataContext { get; set; }

    [JsonPropertyName("@Microsoft.Dynamics.CRM.fetchxmlpagingcookie")]
    public string? FetchXmlPagingCookie { get; set; }

    [JsonPropertyName("@odata.count")]
    public int Count { get; set; }

    [JsonPropertyName("value")]
    public T Value { get; set; }

    public string? GetPagingCookie()
    {
        if (FetchXmlPagingCookie == null)
        {
            return null;
        }

        var match = CookieRegex.Match(FetchXmlPagingCookie);

        if (match.Groups.Count != 2)
        {
            return null;
        }

        var pagingCookieTwiceEncoded = match.Groups[1].Value;
        var pagingCookie = HttpUtility.UrlDecode(HttpUtility.UrlDecode(pagingCookieTwiceEncoded));

        return HttpUtility.UrlEncode(HttpUtility.HtmlEncode(pagingCookie));
    }
}