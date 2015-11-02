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
            VkApiModule ApiMethods = new VkApiModule();
            using (Form AuthForm = new Form())
            {
                WebBrowser Auth = new WebBrowser();
                Auth.HandleDestroyed += new EventHandler( (object sender, EventArgs e) => 
                {
                    var a = (WebBrowser)sender;
                    string resp = ApiMethods.Get_Response(a.Url);
                    MessageBox.Show(resp);
                });
                AuthForm.Controls.Add(Auth);
                Auth.Url = new Uri(ApiMethods.Authorization());
                Auth.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
                AuthForm.ShowDialog();
            }
            Console.ReadLine();
        }
    }
}
