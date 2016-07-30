using System;
using Newtonsoft.Json;
using System.Windows.Media;

namespace Common
{
    [Serializable()]
    [JsonObject(MemberSerialization.OptIn)]
    public class Song
    {
        public Song(int id)
        {
            this.ID = id;
            this.Percentage = 0;
            this.Image = @"Resources/Pictures/ok_lightgrey.png";
            this.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFD1D3DA");
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
        public Uri Uri { get; set; }
        public Uri DownloadedUri { get; set; }
        public int LyricsID { get; set; }
        [JsonProperty("genre")]
        public int GenreID { get; set; }
        public int Date { get; set; }
        public bool Downloaded { get; set; }
        public int Percentage { get; set; }
        public string Image { get; set; }
        [NonSerialized]
        public Brush _borderBrush;
        public Brush BorderBrush
        {
            get { return _borderBrush; }
            set { _borderBrush = value; }
        }
        public override string ToString()
        {
            return String.Format("{0} - {1}", this.Artist, this.Title);
        }
    }
}