using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Networking.PushNotifications;
using Windows.Storage.Streams;

namespace WnsPushNotificationClient_NETCore31
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RegisterForPushNotificationsButton_Click(object sender, RoutedEventArgs e)
        {
            // sample public application server key (encoded as a url base64-encoded string)
            // NOTE: for production, you should generate your own VAPID keys and store them in an appropriate location
            var publicKeyAsUrlBase64String = "BABLpHwSPlWwj5IrEeU7B8XgQf8glbJDHyZlSgscQ1Sl5uOxq4dDS4kcCHomnrJp6fjRFvUjjfO4cWGVX3y_n8Q";
            var appServerKey = this.ConvertUrlBase64EncodedStringToBuffer(publicKeyAsUrlBase64String);

            // channel id
            // NOTE: this channel name can be any name; alternate push channels support multiple parallel channels (identified by channelId)
            var channelId = "channelName";

            // register with WNS for alternate channel push notifications
            var defaultChannelManager = PushNotificationChannelManager.GetDefault();
            var channel = await defaultChannelManager.CreateRawPushNotificationChannelWithAlternateKeyForApplicationAsync(appServerKey, channelId);

            // display the channel uri and expiration
            this.ChannelUriTextBox.Text = channel.Uri;

            MessageBox.Show("WNS Push Notification Channel has been registered.\r\n\r\nChannel Uri: " + channel.Uri + "\r\n\r\nExpires: " + channel.ExpirationTime.ToString());
        }

        private IBuffer ConvertUrlBase64EncodedStringToBuffer(string urlBase64String)
        {
            /* convert the "url base 64" encoded string into a regular "base 64" encoded string */

            // step 1: clone the original urlBase64-encoded string; we'll edit it on the fly
            var base64String = urlBase64String;

            // step 2: strip any trailing padding (trailing periods), in case they exist
            // NOTE: we could probably just replace periods with equal signs, but by measuring the actual length we can assure that padding is correct
            while (base64String.Length > 0 && base64String.Last() == '.')
            {
                base64String.Remove(base64String.Length - 1);
            }

            // step 3: convert any '-' character to '+'; convert any '_' character to '/'
            base64String = base64String.Replace('-', '+');
            base64String = base64String.Replace('_', '/');

            // step 4: add any required padding
            var trailingPaddingLength = (4 - (base64String.Length % 4) % 4);
            base64String += new string('=', trailingPaddingLength);

            /* convert our base64 string to a buffer (after first converting it to a byte array) */
            var base64StringAsByteArray = Convert.FromBase64String(base64String);
            var base64StringAsBuffer = base64StringAsByteArray.AsBuffer();

            /* return the result */
            return base64StringAsBuffer;
        }
    }
}
