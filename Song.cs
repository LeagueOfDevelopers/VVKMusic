using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
 

namespace ConsoleApplication1
{
    [JsonObject(MemberSerialization.OptIn)]
    class Song
    {
        [JsonProperty("aid")]
        public int Id {get; set;}
        [JsonProperty("owner_id")]
        public int owner_id {get; set;}

        [JsonProperty("artist")]
        public string artist { get; set; }
        [JsonProperty("title")]
        public string title { get; set; }
        [JsonProperty("duration")]
        public int duration{get; set;}
        [JsonProperty("url")]
        public string url {get; set;}
        public int lyrics_id {get; set;}
        public int album_id {get; set;}
        [JsonProperty("genre")]
        public int genre_id {get; set;}
        public int date {get; set;}
    }
}
