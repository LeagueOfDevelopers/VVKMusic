using Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;


namespace VKAPI
{
    public class VKAPI : IVKAPI
    {
        private string Token {get; set;}
        public string User_id { get; set; }

        private enum Errors
        {
            User_denied
        }
        public string Auth()
        {
            string AuthRequest = String.Format("https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope={1}&response_type=token&v=5.37&revoke=1"
                , 5114224, "audio");
            return AuthRequest;
        }
        public string GetResponse(string Url)
        {
            if (Url != null)
            {
                string Url_string = Url.ToString();
                string[] Url_massive = Url_string.Split('=');
                string Response = Url_massive[Url_massive.Length - 1];
                if (Url_string.StartsWith("https://oauth.vk.com/blank.html"))
                {
                    if (Url_string.Contains("error")) { return Response; }
                    Token = Url_massive[1].Split('&')[0];
                    User_id = Response;
                    GetAudio();
                    return Response;
                }
            }
            return "Вы преждевременно закрыли окно";
        }
        public Song[] GetAudio()
        {
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.get?owner_id={0}&access_token={1}",User_id,Token);
            WebRequest AudioRequest = WebRequest.Create(GetAudioRequest);
            WebResponse AudioAnswer = AudioRequest.GetResponse();
            Stream dataStream = AudioAnswer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responeFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            responeFromServer = HttpUtility.HtmlDecode(responeFromServer);

            JObject obj = JObject.Parse(responeFromServer);
            Song[] Songs = obj["response"].Children().Skip(1).Select(c => c.ToObject<Song>()).ToArray();
            return Songs;
        }

        public void AddAudio(int audioId, string userID, string token)
        {
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.add?audio_id={0}&owner_id={1}&access_token={2}&redirect_uri=https://oauth.vk.com/blank.html", audioId, userID, token);
            WebRequest AudioRequest = WebRequest.Create(GetAudioRequest);
            WebResponse AudioAnswer = AudioRequest.GetResponse();
            Stream dataStream = AudioAnswer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responeFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
        }

        public void DeleteAudio(int audioId, string userID, string token)
        {
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.delete?audio_id={0}&owner_id={1}&access_token={2}", audioId, userID, token);
            WebRequest AudioRequest = WebRequest.Create(GetAudioRequest);
            WebResponse AudioAnswer = AudioRequest.GetResponse();
            Stream dataStream = AudioAnswer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responeFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
        }

        public Song[] GetAudioExternal(string userID, string token)
        {
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.get?owner_id={0}&access_token={1}", userID, token);
            WebRequest AudioRequest = WebRequest.Create(GetAudioRequest);
            WebResponse AudioAnswer = AudioRequest.GetResponse();
            Stream dataStream = AudioAnswer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responeFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            responeFromServer = HttpUtility.HtmlDecode(responeFromServer);

            JObject obj = JObject.Parse(responeFromServer);
            Song[] Songs;
            try
            {
                Songs = obj["response"].Children().Skip(1).Select(c => c.ToObject<Song>()).ToArray();
            }
            catch (NullReferenceException)
            {
                Songs = new Song[0];
            }
            return Songs;
        }

        public Song[] SearchAudio(string text, string token)
        {
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.search?&q={0}&auto_complete=1&sort=2&count=300&access_token={1}", text, token);
            WebRequest AudioRequest = WebRequest.Create(GetAudioRequest);
            WebResponse AudioAnswer = AudioRequest.GetResponse();
            Stream dataStream = AudioAnswer.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responeFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            responeFromServer = HttpUtility.HtmlDecode(responeFromServer);

            JObject obj = JObject.Parse(responeFromServer);
            Song[] Songs;
            try
            {
                Songs = obj["response"].Children().Skip(1).Select(c => c.ToObject<Song>()).ToArray();
            }
            catch (NullReferenceException)
            {
                Songs = new Song[0];
            }
            return Songs;
        }
    }
}
