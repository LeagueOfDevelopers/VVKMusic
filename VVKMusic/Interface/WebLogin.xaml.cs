using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Interface
{
    /// <summary>
    /// Interaction logic for WebLogin.xaml
    /// </summary>
    public class CustomEventArgs : EventArgs
    {
        public CustomEventArgs(string name, string accesToken, int userID)
        {

            _AccessToken = accesToken;
            _UserID = userID;
            _Name = name;
        }
        private string _AccessToken;
        private int _UserID;
        private string _Name;
        public string Name
        {
            get { return _Name; }
        }
        public string AccessToken
        {
            get { return _AccessToken; }
        }
        public int UserID
        {
            get { return _UserID; }
        }
    }
    public partial class WebLogin : Window
    {
        public string AccessToken = "";
        public int UserID = 0;
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;
        public WebLogin()
        {
            InitializeComponent();
            Loaded += WebLogin_Loaded;
            RaiseCustomEvent += WebLogin_RaiseCustomEvent;
        }
        private void WebLogin_Loaded(object sender, RoutedEventArgs e)
        {
            int AppID = 5114224;
            string Scope = "audio";
            (wbsWebLogin.Child as System.Windows.Forms.WebBrowser).Navigate(String.Format("http://api.vkontakte.ru/oauth/authorize?client_id={0}&scope={1}&display=popup&response_type=token&revoke=1", AppID, Scope));
        }
        private void wbsWebLogin_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            AccessToken = "";
            UserID = 0;
            if (e.Url.ToString().IndexOf("access_token") != -1)
            {
                Regex myReg = new Regex(@"(?<name>[\w\d\x5f]+)=(?<value>[^\x26\s]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in myReg.Matches(e.Url.ToString()))
                {
                    if (m.Groups["name"].Value == "access_token")
                    {
                        AccessToken = m.Groups["value"].Value;
                    }
                    else if (m.Groups["name"].Value == "user_id")
                    {
                        UserID = Convert.ToInt32(m.Groups["value"].Value);
                    }
                }
                wbsWebLogin.Visibility = Visibility.Hidden;
                txtUserName.Visibility = Visibility.Visible;
                btnSubmit.Visibility = Visibility.Visible;
            }
            
        }
        private void WebLogin_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            this.Close();
        }
        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.Text = "";
        }
        private void txtUserName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUserName.Text == "")
                txtUserName.Text = "Enter your name here";
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomEvent(this, new CustomEventArgs(txtUserName.Text, AccessToken, UserID));
        }
    } 
}
