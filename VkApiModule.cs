using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class VkApiModule
    {
        private static string Token {get; set;}
        private static string User_id { get; set; }
        public static string Authorization()
        {
            string AuthRequest = String.Format("https://oauth.vk.com/authorize?client_id={0}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope={1}&response_type=token&v=5.37"
                , 5114224, "audio");
            return AuthRequest;
        }

        public static string Logout()
        {
            //string LogoutRequest = "https://api.vk.com/oauth/logout?client_id=5114224";
            string LogoutRequest = "https://login.vk.com/?act=logout&hash=61d76ff4cef2e4859f&_origin=http://vk.com";
            return LogoutRequest;
        }

        public static void Get_Token(Uri Url)
        {
            string Url_string = Url.ToString();
            VkApiModule.Token = Url_string.Split('=')[1].Split('&')[0];
            VkApiModule.User_id = Url_string.Split('=')[3];
            Console.WriteLine("{0} - {1}", VkApiModule.Token, VkApiModule.User_id);
        }
        public static void Get_audio(Uri Url)
        {

            //https://api.vk.com/method/users.get?user_id=66748&v=5.37&access_token=533bacf01e11f55b536a565b57531ac114461ae8736d6506a3
            string GetAudioRequest = String.Format("https://api.vk.com/method/audio.get?owner_id={0}&access_token={1}",User_id,Token);

        }
    }
}
