using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.RecyclerView.Widget;
using CtrlTv;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Rssdp;

namespace CtrlTV
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        DeviceListAdapter mAdapter;
        DeviceList mDeviceList;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Set up toolbar
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //Set up FAB
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            //Set up recycler view
            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mDeviceList = new DeviceList();

            // Plug in the linear layout manager
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // Plug in recycler view adapter:
            mAdapter = new DeviceListAdapter(mDeviceList);
            mAdapter.DeviceClick += MAdapter_DeviceClick;
            mRecyclerView.SetAdapter(mAdapter);

            //Add divider between items
            mRecyclerView.AddItemDecoration(new DividerItemDecoration(mRecyclerView.Context, DividerItemDecoration.Vertical));

            //Scan for devices
            await UpdateDeviceList();

        }

        private void MAdapter_DeviceClick(object sender, int position)
        {

            string deviceIp = mDeviceList.Address[position];
            string uri = "ws://" + deviceIp + ":3000";

            var myIntent = new Intent(this, typeof(ControlActivity));
            myIntent.PutExtra("DEVICE_URI", uri);
            myIntent.PutExtra("DEVICE_CLIENT_KEY", "37802f5921bf55e93068e22d1c73778e");
            StartActivityForResult(myIntent, 0);
        }



        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        async Task UpdateDeviceList()
        {
            //SSDP - Device Discovery
            SsdpDeviceLocator nsd = new SsdpDeviceLocator();
            var deviceList = await nsd.SearchAsync();

            string deviceName;
            string pattern = @"\b\d+.\d+.\d+.\d+\b"; //IP address patern

            foreach (var d in deviceList)
            {
                try
                {
                    var fullDevice = await d.GetDeviceInfo();
                    deviceName = fullDevice.FriendlyName;
                }
                catch (Exception e)
                {
                    deviceName = "";
                    Log.Debug("LOGMSG", "Getting Device Info failed:" + e.Message);
                }

                if (deviceName != "" && !mDeviceList.Name.Contains(deviceName))
                {
                    mDeviceList.Name.Add(deviceName);
                    mDeviceList.Uri.Add(d.DescriptionLocation.ToString());
                    mDeviceList.Usn.Add(d.Usn.ToString());

                    Match m = Regex.Match(d.DescriptionLocation.ToString(), pattern, RegexOptions.IgnoreCase);
                    mDeviceList.Address.Add(m.Groups[0].ToString());

                    mAdapter.NotifyDataSetChanged();
                }

            }
        }

        private async void FabOnClick(object sender, EventArgs eventArgs)
        {
            await UpdateDeviceList();
        }

        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class DeviceListAdapter : RecyclerView.Adapter
    {
        public DeviceList mDeviceList;
        public event EventHandler<int> DeviceClick;

        public DeviceListAdapter(DeviceList deviceList)
        {
            mDeviceList = deviceList;
        }

        public override int ItemCount
        {
            get { return mDeviceList.Count; }
        }

        void OnClick(int position)
        {
            if (DeviceClick != null)
                DeviceClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            DeviceListHolder vh = holder as DeviceListHolder;

            vh.Caption.Text = mDeviceList.Name[position];
            vh.Address.Text = mDeviceList.Address[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeviceListView, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            DeviceListHolder vh = new DeviceListHolder(itemView, OnClick);
            return vh;
        }

        //Inner Class
        public class DeviceListHolder : RecyclerView.ViewHolder
        {
            public TextView Caption { get; private set; }
            public TextView Address { get; private set; }

            public DeviceListHolder(View itemView, Action<int> listener) : base(itemView)
            {
                Caption = itemView.FindViewById<TextView>(Resource.Id.deviceName);
                Address = itemView.FindViewById<TextView>(Resource.Id.deviceLocation);

                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }

    public class DeviceList
    {
        public List<string> Name;
        public List<string> Usn;
        public List<string> Uri;
        public List<string> Address;

        public int Count
        {
            get { return Name.Count; }
        }

        public DeviceList()
        {
            Name = new List<string>();
            Usn = new List<string>();
            Uri = new List<string>();
            Address = new List<string>();
        }
    }
}

