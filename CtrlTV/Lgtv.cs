/// Lgtv.cs
/// 
/// LG TV WebOS remote control library
///

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CtrlTv
{
    class Lgtv
    {
        public const int RESULT_ERROR = 1;
        public const int RESULT_OK = 0;

        //Handshake first connection
        const string hello = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";
        const string hello_w_key = "{\"type\":\"register\",\"id\":\"register_0\",\"payload\":{\"forcePairing\":false,\"pairingType\":\"PROMPT\",\"client-key\":\"CLIENTKEYGOESHERE\",\"manifest\":{\"manifestVersion\":1,\"appVersion\":\"1.1\",\"signed\":{\"created\":\"20140509\",\"appId\":\"com.lge.test\",\"vendorId\":\"com.lge\",\"localizedAppNames\":{\"\":\"LG Remote App\",\"ko-KR\":\"리모컨 앱\",\"zxx-XX\":\"ЛГ Rэмotэ AПП\"},\"localizedVendorNames\":{\"\":\"LG Electronics\"},\"permissions\":[\"TEST_SECURE\",\"CONTROL_INPUT_TEXT\",\"CONTROL_MOUSE_AND_KEYBOARD\",\"READ_INSTALLED_APPS\",\"READ_LGE_SDX\",\"READ_NOTIFICATIONS\",\"SEARCH\",\"WRITE_SETTINGS\",\"WRITE_NOTIFICATION_ALERT\",\"CONTROL_POWER\",\"READ_CURRENT_CHANNEL\",\"READ_RUNNING_APPS\",\"READ_UPDATE_INFO\",\"UPDATE_FROM_REMOTE_APP\",\"READ_LGE_TV_INPUT_EVENTS\",\"READ_TV_CURRENT_TIME\"],\"serial\":\"2f930e2d2cfe083771f68e4fe7bb07\"},\"permissions\":[\"LAUNCH\",\"LAUNCH_WEBAPP\",\"APP_TO_APP\",\"CLOSE\",\"TEST_OPEN\",\"TEST_PROTECTED\",\"CONTROL_AUDIO\",\"CONTROL_DISPLAY\",\"CONTROL_INPUT_JOYSTICK\",\"CONTROL_INPUT_MEDIA_RECORDING\",\"CONTROL_INPUT_MEDIA_PLAYBACK\",\"CONTROL_INPUT_TV\",\"CONTROL_POWER\",\"READ_APP_STATUS\",\"READ_CURRENT_CHANNEL\",\"READ_INPUT_DEVICE_LIST\",\"READ_NETWORK_STATE\",\"READ_RUNNING_APPS\",\"READ_TV_CHANNEL_LIST\",\"WRITE_NOTIFICATION_TOAST\",\"READ_POWER_STATE\",\"READ_COUNTRY_INFO\"],\"signatures\":[{\"signatureVersion\":1,\"signature\":\"eyJhbGdvcml0aG0iOiJSU0EtU0hBMjU2Iiwia2V5SWQiOiJ0ZXN0LXNpZ25pbmctY2VydCIsInNpZ25hdHVyZVZlcnNpb24iOjF9.hrVRgjCwXVvE2OOSpDZ58hR+59aFNwYDyjQgKk3auukd7pcegmE2CzPCa0bJ0ZsRAcKkCTJrWo5iDzNhMBWRyaMOv5zWSrthlf7G128qvIlpMT0YNY+n/FaOHE73uLrS/g7swl3/qH/BGFG2Hu4RlL48eb3lLKqTt2xKHdCs6Cd4RMfJPYnzgvI4BNrFUKsjkcu+WD4OO2A27Pq1n50cMchmcaXadJhGrOqH5YmHdOCj5NSHzJYrsW0HPlpuAx/ECMeIZYDh6RMqaFM2DXzdKX9NmmyqzJ3o/0lkk/N97gfVRLW5hA29yeAwaCViZNCP8iC9aO0q9fQojoa7NQnAtw==\"}]}}}";

        int command_count = 0;
        ClientWebSocket client;
        bool registration_in_progress;

        /// <summary>
        /// Client key of the connected TV
        /// </summary>
        public string ClientKey { get; private set; }

        /// <summary>
        /// TV volume as a value between 0 and 100.
        /// </summary>
        public int Volume { get; private set; }

        /// <summary>
        /// TV mute status. Value is true for muted, false for not muted.
        /// </summary>
        public bool Muted { get; private set; }

        /// <summary>
        /// List of all the channels on the connected TV.
        /// </summary>
        public List<TVChannel> ChannelList { get; private set; }

        /// <summary>
        /// Current channel number on the TV input
        /// </summary>
        public int ChannelNumber { get; private set; }

        public delegate void Fn(int err, string resp);

        public static Fn NoNext()
        {
            return delegate { };
        }

        /// <summary>
        /// Take control of a unpaired LG TV with webOS. 
        /// </summary>
        public Lgtv() 
        {
            ClientKey = "";
        }

        /// <summary>
        /// Take control of a paired LG TV with webOS.
        /// </summary>
        /// <param name="clientKey">The TV client key</param>
        public Lgtv(string clientKey)
        {
            ClientKey = clientKey;
        }

        private string get_handshake()
        {
            registration_in_progress = false;

            if (ClientKey != "")
            {
                return hello_w_key.Replace("CLIENTKEYGOESHERE", ClientKey);
            }
            else
            {
                Console.WriteLine("First usage, let's pair with TV.");
                return hello;
            }
        }

        private async Task open_connection(string host, Fn fn)
        {
            Console.WriteLine("Connecting to TV... ("+host+")");
            Uri hostUri = new Uri(host);

            if(client != null)
            {
                client.Dispose();
            }

            client = new ClientWebSocket();

            await client.ConnectAsync(hostUri, CancellationToken.None);

            Console.WriteLine("Building handshake...");

            string hs = get_handshake();
            
            byte[] sendBytes = Encoding.UTF8.GetBytes(hs);
            var sendBuffer = new ArraySegment<byte>(sendBytes);

            if (client.State == WebSocketState.Open)
            {
                //Thread to receive messages
                Console.WriteLine("Starting message receiver thread...");

                _ = Task.Factory.StartNew(
                    async () =>
                    {
                        var rcvBytes = new byte[1024];
                        var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                        while (true)
                        {
                            WebSocketReceiveResult rcvResult = await client.ReceiveAsync(rcvBuffer, CancellationToken.None);
                            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                            string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                            Console.WriteLine("Received: {0}", rcvMsg);

                            if (rcvMsg.Contains("registered"))
                            {
                                JObject json = JObject.Parse(rcvMsg);
                                ClientKey = json["payload"]["client-key"].ToString();

                                Console.WriteLine("Paired with device: " + ClientKey);

                                break;
                            }

                        }

                        Console.WriteLine("Paired. Calling next function...");
                        fn(RESULT_OK, "Paired with device");

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }

            Console.WriteLine("Sending handshake: " + hs);
            while(client.State == WebSocketState.Open)
            {
                if (registration_in_progress == false)
                {
                    await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    registration_in_progress = true;
                    if(ClientKey != "")
                        break;
                }

            }

        }

        private async Task close_connection()
        {
            Console.WriteLine("Closing connection...");

            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            
            if(client.State == WebSocketState.Closed)
            {
                client.Dispose();
                Console.WriteLine("Disconnected.");
            }

        }

        private async Task ws_send(string msg)
        {

            if(client == null || client.State != WebSocketState.Open)
            {
                Console.WriteLine("ws_send(): No connection.");
                return;
            }

            byte[] sendBytes = Encoding.UTF8.GetBytes(msg);
            var sendBuffer = new ArraySegment<byte>(sendBytes);

            await client.SendAsync(sendBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }

        
        /// <summary>
        /// Disconnects from the TV.
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            await close_connection();
        }

        /// <summary>
        /// Creates a Web Socket connection with the TV. 
        /// </summary>
        /// <param name="host">The IP address of the TV. Must specify ws:// protocol and port 3000. Eg.: ws://192.168.2.10:3000</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task Connect(string host, Fn fn)
        {
            if (client != null && client.State == WebSocketState.Open)
            {
                //Already connected
                Console.WriteLine("Connect(): already connected");
                return;
            }

            await open_connection(host, fn);

        }

        /// <summary>
        /// Displays a float popup message on the TV screen.
        /// </summary>
        /// <param name="text">Message to display on the TV screen</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task ShowFloat(string text, Fn fn)
        {
            await SendCommand("", "request", "ssap://system.notifications/createToast", "{\"message\": \""+ text + "\"}", fn);
        }

        /// <summary>
        /// Changes the TV volume.
        /// </summary>
        /// <param name="value">Value between 0 and 100 representing the TV volume</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task SetVolume(int value, Fn fn)
        {
            if((value < 0) || (value > 100))
            {
                fn(RESULT_ERROR, "Volume value out of range (0-100)");
                return;
            }
            
            await SendCommand("", "request", "ssap://audio/setVolume", @"{""volume"":" + value + @"}", fn);
        }

        /// <summary>
        /// Send a Web Socket request recognized by the TV.
        /// </summary>
        /// <param name="prefix">Usually empty</param>
        /// <param name="msgtype">The type of the request made. Has a value of 'register' when registering a device or 'request' to perform any other action.</param>
        /// <param name="uri">The URI containing the service and action requested</param>
        /// <param name="payload">Data in the form of a JSON string containing parameters required in the request. Sometimes this value is empty.</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task SendCommand(string prefix, string msgtype, string uri, string payload, Fn fn) {
            command_count++;
            string msg = @"{""id"":"""+ prefix + command_count + @""",""type"":""" + msgtype + @""",""uri"":""" + uri + @"""";
            if (payload != null && payload.Length > 0)
            {
                msg += @",""payload"":" + payload + "}";
            }
            else
            {
                msg += "}";
            }

            Console.WriteLine("Sending command:" + msg);

            try
            {
                await ws_send(msg);
                fn(RESULT_OK, "");
            }
            catch (Exception e)
            {

                Console.WriteLine("Error: couldn't send message: " + e.Message);
                fn(RESULT_ERROR, "Couldn't send message");
            }
        }

        /// <summary>
        /// Turns off the TV.
        /// </summary>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task TurnOff(Fn fn)
        {
            await SendCommand("", "request", "ssap://system/turnOff", null, fn);
        }

        /// <summary>
        /// Get TVs current volume number and whether the TV is muted or not. Note: the values are not returned. They can be accessed from the Volume and Muted properties.
        /// </summary>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task GetVolume(Fn fn)
        {
            await SendCommand("status_", "request", "ssap://audio/getVolume", null, NoNext());

            _ = Task.Factory.StartNew(
                    async () =>
                    {
                        var rcvBytes = new byte[1024];
                        var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                        while (true)
                        {
                            WebSocketReceiveResult rcvResult = await client.ReceiveAsync(rcvBuffer, CancellationToken.None);
                            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                            string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                            Console.WriteLine("Received: {0}", rcvMsg);

                            if (rcvMsg.Contains("volume"))
                            {
                                Console.WriteLine("Handling volume response...");
                                // {"type":"response","id":"status_1","payload":{"muted":false,"scenario":"mastervolume_tv_speaker","active":false,"action":"requested","volume":7,"returnValue":true,"subscribed":true}}
                                try
                                {
                                    JObject json = JObject.Parse(rcvMsg);
                                    Muted = json["payload"]["muted"].ToString() == "true" ? true : false;
                                    Volume = int.Parse(json["payload"]["volume"].ToString());
                                    Console.WriteLine("Handled volume response.");
                                    break;
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine("Failed to handle volume response: " + e.Message);
                                    break;
                                }
                                
                                
                               
                            }

                        }
                        Console.WriteLine("Calling next function from GetVolume()");
                        fn(RESULT_OK, "");

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            
        }

        /// <summary>
        /// Puts the TV on mute or unmutes it.
        /// </summary>
        /// <param name="value">True for mute, false for unmute</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>

        public async Task SetMute(bool value, Fn fn)
        {
            await SendCommand("", "request", "ssap://audio/setMute", @"{ ""mute"": " + (value?"true":"false") + "}", fn);
        }

        /// <summary>
        /// Same as GetVolume(). Get TVs current volume number and whether the TV is muted or not. Note: the values are not returned. They can be accessed from the Volume and Muted properties.
        /// </summary>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task GetMute(Fn fn)
        {
            await GetVolume(fn);
        }

        /// <summary>
        /// Changes the channel on the TV
        /// </summary>
        /// <param name="channelId">The id of the channel eg.: 0_13_7_0_0_1307_0</param>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task SetChannel(string channelId, Fn fn)
        {
            await SendCommand("", "request", "ssap://tv/openChannel", @"{ ""channelId"": """ + channelId + @"""}", fn);
        }

        /// <summary>
        /// Gets the current channel number on the TV. Note: the value is not returned. It can be accessed from the ChannelNumber property.
        /// </summary>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task GetChannel(Fn fn)
        {
            await SendCommand("channels_", "request", "ssap://tv/getCurrentChannel", null, NoNext());

            _ = Task.Factory.StartNew(
                    async () =>
                    {
                        var rcvBytes = new byte[1024];
                        var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                        while (true)
                        {
                            WebSocketReceiveResult rcvResult = await client.ReceiveAsync(rcvBuffer, CancellationToken.None);
                            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                            string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                            Console.WriteLine("Received: {0}", rcvMsg);

                            if (rcvMsg.Contains("channels_"))
                            {
                                Console.WriteLine("Handling current channel response...");
                                try
                                {
                                    JObject json = JObject.Parse(rcvMsg);
                                    var channelNumber = json["payload"]["channelNumber"];

                                    ChannelNumber = int.Parse(channelNumber.ToString());

                                    Console.WriteLine("Handled current channel response.");
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Failed to handle current channel response: " + e.Message);
                                    break;
                                }
                            }

                        }
                        Console.WriteLine("Calling next function from GetChannel()");
                        fn(RESULT_OK, "");

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Get the id of a channel from its assigned number.
        /// </summary>
        /// <param name="number">The channel number</param>
        /// <returns>String containing id of the channel. Empty string if the channel is not found</returns>
        public string ChannelIdFromNumber(int number)
        {
            if(ChannelList == null || ChannelList.Count == 0)
            {
                throw new Exception("Channel list is empty");
            }

            foreach(var channel in ChannelList)
            {
                if(channel.Number == number.ToString())
                {
                    return channel.Id;
                }
            }

            //Channel number is not in the list
            return string.Empty;
        }

        /// <summary>
        /// Retrieves the TVs channel list that contains the id, name and channel number. Note: the list is not returned. It can be accessed from the ChannelList property.  
        /// </summary>
        /// <param name="fn">Next function</param>
        /// <returns></returns>
        public async Task GetChannelList(Fn fn)
        {
            await SendCommand("channels_", "request", "ssap://tv/getChannelList", null, NoNext());

            _ = Task.Factory.StartNew(
                    async () =>
                    {
                        var rcvBytes = new byte[1024 * 1024 * 2]; // 2MB max response
                        var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                        while (true)
                        {
                            WebSocketReceiveResult rcvResult = await client.ReceiveAsync(rcvBuffer, CancellationToken.None);
                            byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                            string rcvMsg = Encoding.UTF8.GetString(msgBytes);

                            Console.WriteLine("Received: {0}", rcvMsg);

                            if (rcvMsg.Contains("cableDigitalCount"))
                            {
                                Console.WriteLine("Handling channel list response...");
                                try
                                {
                                    JObject json = JObject.Parse(rcvMsg);
                                    var channelListArray = json["payload"]["channelList"];
                                    List<TVChannel> channels = new List<TVChannel>();

                                    for(int i=channelListArray.Count() - 1; i>=0; i--)
                                    {
                                        channels.Add(new TVChannel(
                                            channelListArray[i]["channelId"].ToString(),
                                            channelListArray[i]["channelName"].ToString(),
                                            channelListArray[i]["channelNumber"].ToString()));
                                    }

                                    ChannelList = channels;

                                    Console.WriteLine("Handled channel list response.");
                                    break;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Failed to handle channel list response: " + e.Message);
                                    break;
                                }
                            }

                        }
                        Console.WriteLine("Calling next function from GetChannelList()");
                        fn(RESULT_OK, "");

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);


        }
    }

    /// <summary>
    /// A TV channel with an id, name and number.
    /// </summary>
    public class TVChannel
    {
        /// <summary>
        /// Channel id. Eg.: 0_13_7_0_0_1307_0
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The name of the channel, usually as registered in the TV.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of the channel as it appears in the TV. Usually an integer.
        /// </summary>
        public string Number { get; private set; }

        /// <summary>
        /// A TV channel with an id, name and number.
        /// </summary>
        /// <param name="id">Id of the TV channel</param>
        /// <param name="name">Name of the TV channel</param>
        /// <param name="number">Number of the TV channel</param>
        public TVChannel(string id, string name, string number)
        {
            Id = id;
            Name = name;
            Number = number;
        }
    }
}