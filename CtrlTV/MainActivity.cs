using System;
using System.Collections.Generic;
using Android.App;
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
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Google.Android.Material.Snackbar;
using Rssdp;

namespace CtrlTV
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        DeviceListAdapter mAdapter;
        DeviceList mDeviceList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            mDeviceList = new DeviceList();

            // Plug in the linear layout manager:
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            // Plug in my adapter:
            mAdapter = new DeviceListAdapter(mDeviceList);
            mRecyclerView.SetAdapter(mAdapter);

            //Add divider between items
            mRecyclerView.AddItemDecoration(new DividerItemDecoration(mRecyclerView.Context, DividerItemDecoration.Vertical));

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

        private async void FabOnClick(object sender, EventArgs eventArgs)
        {
            //SSDP - Device Discovery
            SsdpDeviceLocator nsd = new SsdpDeviceLocator();
            var deviceList = await nsd.SearchAsync();

            string deviceName;

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
                    mDeviceList.Address.Add(d.DescriptionLocation.ToString());
                    mDeviceList.Usn.Add(d.Usn.ToString());

                    mAdapter.NotifyDataSetChanged();
                }

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
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class DeviceListAdapter : RecyclerView.Adapter
    {
        public DeviceList mDeviceList;

        public DeviceListAdapter(DeviceList deviceList)
        {
            mDeviceList = deviceList;
        }

        public override int ItemCount
        {
            get { return mDeviceList.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            DeviceListHolder vh = holder as DeviceListHolder;

            vh.Caption.Text = mDeviceList.Name[position];
            vh.Address.Text = mDeviceList.Address[position];
            vh.Usn.Text = mDeviceList.Usn[position];
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DeviceListView, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            DeviceListHolder vh = new DeviceListHolder(itemView);
            return vh;
        }

        //Inner Class
        public class DeviceListHolder : RecyclerView.ViewHolder
        {
            public TextView Caption { get; private set; }
            public TextView Address { get; private set; }
            public TextView Usn { get; private set; }

            public DeviceListHolder(View itemView) : base(itemView)
            {
                Caption = itemView.FindViewById<TextView>(Resource.Id.deviceName);
                Address = itemView.FindViewById<TextView>(Resource.Id.deviceLocation);
                Usn = itemView.FindViewById<TextView>(Resource.Id.deviceUsn);
            }
        }
    }

    public class DeviceList
    {
        public List<string> Name;
        public List<string> Usn;
        public List<string> Address;

        public int Count
        {
            get { return Name.Count; }
        }

        public DeviceList()
        {
            Name = new List<string>();
            Usn = new List<string>();
            Address = new List<string>();

            /*Name.Add("DUMMY 1");
            Usn.Add("0000:0000");
            Address.Add("http://0.0.0.0/");

            Name.Add("DUMMY 2");
            Usn.Add("0000:0000");
            Address.Add("http://0.0.0.0/");*/
        }
    }
}

