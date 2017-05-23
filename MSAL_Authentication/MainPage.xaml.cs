using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Net.Http;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MSAL_Authentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //Set the API Endpoint
        string _GraphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";

        //Set the scope
        private string[] _Scopes = new string[] { "User.Read" };

        private AuthenticationResult authResult = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await SignInHelper();
        }

        public async Task<bool> SignInHelper()
        {
            bool flagSignedIn = false;
            try
            {
                authResult = await App.clientApplication.AcquireTokenAsync(_Scopes);
                if (authResult != null)
                {
                    DisplayParseAuthResult(authResult);
                    string response = await SendGetRequest(authResult.AccessToken);
                    DisplayAPIResponse(response);
                }

            }
            catch (MsalServiceException MSALex)
            {
                txtStatus.Text = MSALex.Message;
            }
            catch (Exception ex)
            {
                txtStatus.Text = ex.Message;
            }

            flagSignedIn = true;
            return flagSignedIn;
        }

        private void DisplayAPIResponse(string response)
        {
            txtAPIResponse.Text = response;
        }

        public async Task<string> SendGetRequest(string _AccessToken)
        {
            HttpClient httpClient = new HttpClient();
            // Add access token to the header of the GET request
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _AccessToken);
            Uri GraphAPIEndpointUri = new Uri(_GraphAPIEndpoint);
            HttpResponseMessage response = await httpClient.GetAsync(GraphAPIEndpointUri);
            // Serialize the received HTTP response.
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public void DisplayParseAuthResult(AuthenticationResult _AuthResult)
        {
            txtTokenRaw.Text += string.Format("User Name: {0} \nUserId: {1} \nAccess Token: {2} \n", 
                                             _AuthResult.User.Name, _AuthResult.User.DisplayableId, _AuthResult.AccessToken);
        }
    }
}
