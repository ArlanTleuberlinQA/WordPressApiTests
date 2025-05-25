using System.Text;

namespace WordPress.Drivers.DriverFactory
{
     public static class WordPressPostLifecycle
{
    public const string BaseUrl = "https://dev.emeli.in.ua/wp-json/wp/v2";

    public static readonly string Credentials =
        Convert.ToBase64String(Encoding.ASCII.GetBytes("admin:Engineer_123"));

    public const int PerformanceTimeout = 3000;
    public static HttpClient client;
}

}