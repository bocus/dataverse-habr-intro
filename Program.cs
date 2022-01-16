using Dataverse.Habr.Intro;
using Dataverse.Habr.Intro.Handlers;

const string tenantId = "835d8b21-b31c-45e8-b0a6-cea337ec6d37";
const string clientId = "8b441c8c-2cc1-405c-bf35-6cbdb1204c6d";
const string clientSecret = "4.T5a~RD7t2WpJDklI6vk003F9~FW4m3odpf";
const string scope = "https://your-project-development.api.crm.dynamics.com/";

var authenticationHeader = AuthProvider.GetAuthHeader(tenantId, clientId, clientSecret, scope);
Console.WriteLine($"authenticationHeader: {authenticationHeader}");

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri($"{scope}api/data/v9.2/");
httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

var connectionProvider = new ConnectionProvider(httpClient);

var handlers = new IHandler[] {
    new GetOneField(),
    new GetByFetchXml(),
    new LinesCount(),
    new Paging(),
    new FormattedValues()
};

foreach(var handler in handlers)
{
    Console.WriteLine();
    Console.WriteLine($"{handler.Text}: {handler.Handle(connectionProvider)}");
}