using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Song
    {
        [JsonProperty("aid")]
        public int ID { get; set; }
        [JsonProperty("owner_id")]
        public int OwnerID { get; set; }
        [JsonProperty("artist")]
        public string Artist { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("duration")]
        public int Duration { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        public int LyricsID { get; set; }
        [JsonProperty("genre")]
        public int GenreID { get; set; }
        public int Date { get; set; }
    }
}
