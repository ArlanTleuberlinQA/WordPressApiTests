using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Bogus;

namespace WordPress.Configs.UniversalMethods
{
    public static class UniversalMethods
    {
public static async Task<HttpResponseMessage> SendGetRequest(string url, HttpClient client)
        {
            return await client.GetAsync(url);
        }
        public class PostPayload
{
    public string title { get; set; }
    public string content { get; set; }
    public string status { get; set; }
}
public static PostPayload LoadPayloadFromFile(string filePath)
{
    var json = File.ReadAllText(filePath);
    return JsonSerializer.Deserialize<PostPayload>(json);
}


        public static async Task<HttpResponseMessage> SendPostRequest(string url, HttpClient client, object payload = null)
        {if(payload == null){
            payload = LoadPayloadFromFile("G:/api_learning/WordPress/Configs/PostPayload.json");
        }
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PostAsync(url, content);
        }
        public static async Task<HttpResponseMessage> SendInvalidPostRequest(string url,  HttpClient client)
        {
            var payload = new

            {};
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PostAsync(url, content);
        }

        public static async Task<HttpResponseMessage> SendPutRequest(string url, HttpClient client, object payload = null)
        {
            if (payload == null)
            {
                payload = LoadPayloadFromFile("G:/api_learning/WordPress/Configs/PutPayload.json");
            
            // {

            //     title = "Default updated Post",

            //     content = "Default Updated content for lifecycle testing"

            // };
            }
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PutAsync(url, content);
            
        }

        public static async Task<HttpResponseMessage> SendDeleteRequest(string url, HttpClient client)
        {
            return await client.DeleteAsync(url);
        }

        public static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            }
            public static bool IsStatusCodeOk(HttpResponseMessage response)
            {
                return response.StatusCode == HttpStatusCode.OK;
            }
            public static bool IsStatusCodeCreated(HttpResponseMessage response)
            {
                return response.StatusCode == HttpStatusCode.Created;
            }
            public static bool IsStatusCodeNotFound(HttpResponseMessage response)
            {
                return response.StatusCode == HttpStatusCode.NotFound;
            }
            public static bool IsStatusCodeBadRequest(HttpResponseMessage response)
            {
                return response.StatusCode == HttpStatusCode.BadRequest;
            }
            public static void LogOperationTimes(double createTime, double editTime, double deleteTime)
    {
        double totalTime = createTime + editTime + deleteTime;

        Console.WriteLine($"Create time: {createTime}ms");
        Console.WriteLine($"Edit time: {editTime}ms");
        Console.WriteLine($"Delete time: {deleteTime}ms");
        Console.WriteLine($"Total time: {totalTime}ms");
    }
}

}
