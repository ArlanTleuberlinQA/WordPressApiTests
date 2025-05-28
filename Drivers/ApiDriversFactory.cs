using System.Net.Http.Headers;
using System.Text;

namespace WordPress.Drivers.DriverFactory
{
    public static class WordPressPostLifecycle
    {
        public const string BaseUrl = "https://dev.emeli.in.ua/wp-json/wp/v2";

        public static readonly string Credentials =
            Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:Engineer_123"));

        public const int PerformanceTimeout = 3000;
        public static ThreadLocal<HttpClient> client = new ThreadLocal<HttpClient>(() =>
{
    var handler = new HttpClientHandler
    {
        MaxConnectionsPerServer = 10
    };
    var httpClient = new HttpClient(handler);
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Credentials);
    httpClient.Timeout = TimeSpan.FromMinutes(2);
    return httpClient;
});

    }
}