using PurgarNET.AAConnector.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PurgarNET.AAConnector.Console
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private static string _code = null;

        private static LoginWindow _wnd = null;

        private static Exception _err = null;

        public static string InitializeLogin(Uri loginUri)
        {
            _code = null;
            _err = null;
            _wnd = new LoginWindow();
            _wnd.LoginBrowser.Navigate(loginUri);
            _wnd.ShowDialog();

            if (_err != null)
                throw _err;
            else
                return _code;
        }

        private void LoginBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            LoginBrowser.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Visible;
            if (e.Uri.AbsolutePath == Parameters.REDIRECT_URI.AbsolutePath)
            {
                try
                {
                    _code = GetCodeFromUri(e.Uri);
                }
                catch (Exception err)
                {
                    _err = err;
                }
                _wnd.Close();
            }
        }

        private void LoginBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            LoginBrowser.Visibility = Visibility.Visible;
            Progress.Visibility = Visibility.Collapsed;
        }


        private string GetCodeFromUri(Uri uri)
        {
            var q = uri.Query;
            if (q.StartsWith("?"))
                q = q.Remove(0, 1);
            foreach (var param in q.Split('&'))
            {
                if (param.Contains("="))
                {
                    var arr = param.Split('=');
                    if (arr[0].Equals("code", StringComparison.InvariantCultureIgnoreCase))
                        return arr[1];
                }
            }
         
            throw new InvalidOperationException("Authorization code was not found in the response.");
        }

        
    }
}
