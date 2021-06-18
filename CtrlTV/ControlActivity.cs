using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using CtrlTv;
using Google.Android.Material.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CtrlTV
{
    [Activity(Label = "ControlActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class ControlActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        Button volumeUpBtn, volumeDownBtn, channelUpBtn, channelDownBtn, channelGoBtn;
        EditText channelNumberInput;
        TextView statusText;

        Lgtv lgtv;
        int connectionStatus = 0;
        string deviceUri, deviceClientKey;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_control);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar2);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            statusText = FindViewById<TextView>(Resource.Id.textView1);

            volumeUpBtn = FindViewById<Button>(Resource.Id.volumeUp);
            volumeDownBtn = FindViewById<Button>(Resource.Id.volumeDown);
            channelUpBtn = FindViewById<Button>(Resource.Id.channelUp);
            channelDownBtn = FindViewById<Button>(Resource.Id.channelDown);

            channelNumberInput = FindViewById<EditText>(Resource.Id.channelNumberInput);
            channelGoBtn = FindViewById<Button>(Resource.Id.channelGo);

            channelGoBtn.Click += ChannelGoBtn_Click;

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

        private async void ChannelGoBtn_Click(object sender, EventArgs e)
        {
            if(channelNumberInput.Text.Length > 0)
            {
                try
                {
                    int num = int.Parse(channelNumberInput.Text);
                    if (connectionStatus == 1)
                    {
                        connectionStatus = 2;

                        await lgtv.SetChannel(lgtv.ChannelIdFromNumber(num), async delegate
                        {
                            await lgtv.GetChannel(delegate
                            {
                                connectionStatus = 1;
                            });

                        });
                    }
                }
                catch(Exception ex)
                {
                    Log.Debug("APP", "Error changing channel: " + ex.Message);
                }
                finally
                {
                    channelNumberInput.Text = "";
                }
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_gallery)
            {

            }
            else if (id == Resource.Id.nav_slideshow)
            {

            }
            else if (id == Resource.Id.nav_manage)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
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