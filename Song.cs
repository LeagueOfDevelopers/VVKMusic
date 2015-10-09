using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Song
{
	public int id {get; set;}
	public int owner_id { get; set; }
	public string artist { get; set; }
	public string title { get; set; }
	public int duration { get; set; }
	public string url { get; set; }
	public int lyrics_id { get; set; }
	public int genre_id { get; set; }
}