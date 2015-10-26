using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApplication1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (Form AuthForm = new Form())
            {
                WebBrowser Auth = new WebBrowser();
                Auth.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Getting_token);
                AuthForm.Controls.Add(Auth);
                Auth.Url = new Uri(VkApiModule.Authorization());
                //Auth.Url = new Uri(VkApiModule.Logout());
                Auth.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
                AuthForm.ShowDialog();
            }
            VkApiModule.Get_audio();
            Console.ReadLine();
        }
        static public void Getting_token(object sender, EventArgs e)
        {
            var a = (WebBrowser)sender;
            VkApiModule.Get_Token(a.Url);
            Console.WriteLine("{0}", a.Url);
        }
    }
}
