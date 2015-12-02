using System;
using Newtonsoft.Json;

namespace Common
{

    [Serializable()][JsonObject(MemberSerialization.OptIn)]
    public class Song
    {
        public Song(int id)
        {
            this.ID = id;
        }

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
        public Uri url { get; set; }
        public int LyricsID { get; set; }
        [JsonProperty("genre")]
        public int GenreID { get; set; }
        public int Date { get; set; }
        public bool Downloaded { get; set; }
    }
}
