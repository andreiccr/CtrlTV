using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CtrlTv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtrlTV
{
    [Activity(Label = "ControlActivity")]
    public class ControlActivity : Activity
    {
        Button volumeUpBtn, volumeDownBtn, channelUpBtn, channelDownBtn;
        TextView statusText;

        Lgtv lgtv;
        int connectionStatus = 0;
        string deviceUri, deviceClientKey;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_control);

            statusText = FindViewById<TextView>(Resource.Id.textView1);

            volumeUpBtn = FindViewById<Button>(Resource.Id.volumeUp);
            volumeDownBtn = FindViewById<Button>(Resource.Id.volumeDown);
            channelUpBtn = FindViewById<Button>(Resource.Id.channelUp);
            channelDownBtn = FindViewById<Button>(Resource.Id.channelDown);

            volumeUpBtn.Click += VolumeUpBtn_Click;
            volumeDownBtn.Click += VolumeDownBtn_Click;
            channelUpBtn.Click += ChannelUpBtn_Click;
            channelDownBtn.Click += ChannelDownBtn_Click;

            deviceUri = Intent.GetStringExtra("DEVICE_URI");
            deviceClientKey = Intent.GetStringExtra("DEVICE_CLIENT_KEY");

            if (deviceClientKey == string.Empty)
                lgtv = new Lgtv();
            else
                lgtv = new Lgtv(deviceClientKey);


            await lgtv.Connect(deviceUri, async delegate (int result, string msg)
            {
                if (result == Lgtv.RESULT_OK)
                {
                    //await lgtv.ShowFloat("Connected!", Lgtv.NoNext());

                    //Get all data from TV
                    await lgtv.GetChannelList(async delegate
                    {
                        await lgtv.GetChannel(async delegate
                        {
                            await lgtv.GetVolume(delegate
                            {
                                //statusText.Text = "Connected to " + deviceUri + "(CH: " + lgtv.ChannelNumber + "  V: " + lgtv.Volume + ")";
                                connectionStatus = 1;
                            });
                        });
                    });
                    
                }
                else
                {
                    statusText.Text = "Connection failed!";
                    Intent finishIntent = new Intent(this, typeof(MainActivity));
                    finishIntent.PutExtra("MESSAGE", "Failed to connect");
                    SetResult(Result.Canceled, finishIntent);
                    Finish();
                }

            });

        }

        private async void ChannelDownBtn_Click(object sender, EventArgs e)
        {
            if (connectionStatus == 1)
            {
                connectionStatus = 2;

                await lgtv.SetChannel(lgtv.ChannelIdFromNumber(lgtv.ChannelNumber - 1), async delegate
                {
                    await lgtv.GetChannel(delegate
                    {
                        connectionStatus = 1;
                    });

                });
            }
        }

        private async void ChannelUpBtn_Click(object sender, EventArgs e)
        {
            if (connectionStatus == 1)
            {
                connectionStatus = 2;

                await lgtv.SetChannel(lgtv.ChannelIdFromNumber(lgtv.ChannelNumber + 1), async delegate
                {
                    await lgtv.GetChannel(delegate
                    {
                        connectionStatus = 1;
                    });

                });
            }
        }

        private async void VolumeDownBtn_Click(object sender, EventArgs e)
        {
            if (connectionStatus == 1)
            {
                connectionStatus = 2;

                await lgtv.SetVolume(lgtv.Volume - 1, async delegate
                {
                    await lgtv.GetVolume(delegate
                    {
                        connectionStatus = 1;
                    });

                });
            }
        }

        private async void VolumeUpBtn_Click(object sender, EventArgs e)
        {
            if(connectionStatus == 1)
            {
                connectionStatus = 2;

                await lgtv.SetVolume(lgtv.Volume + 1, async delegate
                {
                    await lgtv.GetVolume(delegate
                    {
                        connectionStatus = 1;
                    });
                    
                });
            }
        }
    }
}