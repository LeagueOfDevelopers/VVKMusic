using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Song
{
	public int ID {get; set;}
	public int OwnerID { get; set; }
	public string Artist { get; set; }
	public string Title { get; set; }
	public int Duration { get; set; }
	public string url { get; set; }
	public int LyricsID { get; set; }
	public int GenreID { get; set; }
}