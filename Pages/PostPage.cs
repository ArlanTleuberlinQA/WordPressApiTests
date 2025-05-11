using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace WordPress.Posts
{
public class WordPressPost

    {

        [JsonPropertyName("id")]

        public int Id { get; set; }
 
        [JsonPropertyName("date")]

        public string Date { get; set; }
 
        [JsonPropertyName("date_gmt")]

        public string DateGmt { get; set; }
 
        [JsonPropertyName("guid")]

        public Guid Guid { get; set; }
 
        [JsonPropertyName("modified")]

        public string Modified { get; set; }
 
        [JsonPropertyName("modified_gmt")]

        public string ModifiedGmt { get; set; }
 
        [JsonPropertyName("slug")]

        public string Slug { get; set; }
 
        [JsonPropertyName("status")]

        public string Status { get; set; }
 
        [JsonPropertyName("type")]

        public string Type { get; set; }
 
        [JsonPropertyName("link")]

        public string Link { get; set; }
 
        [JsonPropertyName("title")]

        public Title Title { get; set; }
 
        [JsonPropertyName("content")]

        public Content Content { get; set; }
 
        [JsonPropertyName("author")]

        public int Author { get; set; }
 
        [JsonPropertyName("comment_status")]

        public string CommentStatus { get; set; }
 
        [JsonPropertyName("ping_status")]

        public string PingStatus { get; set; }
 
        [JsonPropertyName("template")]

        public string Template { get; set; }
 
        [JsonPropertyName("meta")]

        public object Meta { get; set; }

    }
 
    public class Guid

    {

        [JsonPropertyName("rendered")]

        public string Rendered { get; set; }

    }
 
    public class Title

    {

        [JsonPropertyName("rendered")]

        public string Rendered { get; set; }

    }
 
    public class Content

    {

        [JsonPropertyName("rendered")]

        public string Rendered { get; set; }
 
        [JsonPropertyName("protected")]

        public bool Protected { get; set; }
        
    }
}