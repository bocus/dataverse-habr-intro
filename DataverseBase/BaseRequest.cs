using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dataverse.Habr.Intro.DataverseBase;

public abstract class BaseRequest<TOut>
{
    private static readonly XmlSerializerNamespaces EmptyNamespaces = GetEmptyXmlSerializerNamespaces();
    private static readonly XmlWriterSettings XmlWriterSettings = new() { OmitXmlDeclaration = true };
    private static readonly XmlSerializer XmlSerializer = new(typeof(FetchType));

    public virtual bool ReturnFormattedValues => false;

    public abstract HttpMethod HttpMethod { get; }

    public abstract string GetRequest();

    public abstract string? GetBody();

    protected static string Serialize(FetchType fetchType)
    {
        var stringBuilder = new StringBuilder();
        using var writer = XmlWriter.Create(stringBuilder, XmlWriterSettings);
        XmlSerializer.Serialize(writer, fetchType, EmptyNamespaces);
        return stringBuilder.ToString();
    }

    private static XmlSerializerNamespaces GetEmptyXmlSerializerNamespaces()
    {
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);
        return namespaces;
    }
}