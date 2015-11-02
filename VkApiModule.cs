using System;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class VkApiModule
    {
        private string Token {get; set;}
        public string User_id { get; set; }
        private Song[] Songs { get; set; }

        private enum Errors
        {
            User_denied
        }
        public string Authorization()
        {
            string AuthRequest = String.Format("https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope={1}&response_type=token&v=5.37&revoke=1"
                , 5114224, "audio");
            return AuthRequest;
        }
        public string Get_Response(Uri Url)
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
                    Get_audio();
                    return Response;
                }
            }
            return "Вы преждевременно закрыли окно";
        }
        public void Get_audio()
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
            Songs = obj["response"].Children().Skip(1).Select(c => c.ToObject<Song>()).ToArray();
        }


    }
}
