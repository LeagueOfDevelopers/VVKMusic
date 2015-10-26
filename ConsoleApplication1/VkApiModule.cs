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
        private static string Token {get; set;}
        private static string User_id { get; set; }
        private static Song[] Songs { get; set; }
        public static string Authorization()
        {
            string AuthRequest = String.Format("https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope={1}&response_type=token&v=5.37&revoke=1"
                , 5114224, "audio");
            return AuthRequest;
        }
        public static string Logout()
        {
            //
            string LogoutRequest = "https://login.vk.com/?act=logout&hash=61d76ff4cef2e4859f&_origin=http://vk.com";
            return LogoutRequest;
        }
        public static void Get_Token(Uri Url)
        {
            string Url_string = Url.ToString();
            VkApiModule.Token = Url_string.Split('=')[1].Split('&')[0];
            VkApiModule.User_id = Url_string.Split('=')[3];

        }
        public static void Get_audio()
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
