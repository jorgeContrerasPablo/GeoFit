using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_ColorPicker : Screen
    {
        readonly List<ColorItem> colorItems = new List<ColorItem>();
        ListView listView;
        int teamId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ColorList);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            string teamName = Intent.GetStringExtra("teamName") ?? "";
            teamId = Intent.GetIntExtra("teamId",0);

            listView = FindViewById<ListView>(Resource.Id.ColorList_list);

            colorItems.Add(new ColorItem { ColorName = "Blue", Code = "#096ab1" });
            colorItems.Add(new ColorItem { ColorName = "DarkBlue", Code = "#0c0c2a" });
            colorItems.Add(new ColorItem { ColorName = "LightBlue", Code = "#38f5ff" });
            colorItems.Add(new ColorItem { ColorName = "Green", Code = "#3a923c" });
            colorItems.Add(new ColorItem { ColorName = "DarkGreen", Code = "#1d491e" });
            colorItems.Add(new ColorItem { ColorName = "LightGreen", Code = "#62f464" });
            colorItems.Add(new ColorItem { ColorName = "Red", Code = "#f60d0d" });
            colorItems.Add(new ColorItem { ColorName = "DarkRed", Code = "#c30000" });
            colorItems.Add(new ColorItem { ColorName = "LightRed", Code = "#fd5c5c" });
            colorItems.Add(new ColorItem { ColorName = "Yellow", Code = "#f9ff21" });
            colorItems.Add(new ColorItem { ColorName = "DarkYellow", Code = "#bfc100" });
            colorItems.Add(new ColorItem { ColorName = "LightYellow", Code = "#fdffad" });
            colorItems.Add(new ColorItem { ColorName = "Orange", Code = "#e33a19" });
            colorItems.Add(new ColorItem { ColorName = "DarkOrange", Code = "#9e2811" });
            colorItems.Add(new ColorItem { ColorName = "LightOrange", Code = "#ff9155" });
            colorItems.Add(new ColorItem { ColorName = "White", Code = "#ffffff" });
            colorItems.Add(new ColorItem { ColorName = "Black", Code = "#000000" });
            colorItems.Add(new ColorItem { ColorName = "Purple", Code = "#614761" });
            colorItems.Add(new ColorItem { ColorName = "DarkPurple", Code = "#291e29" });
            colorItems.Add(new ColorItem { ColorName = "LightPurple", Code = "#a590a5" });
            colorItems.Add(new ColorItem { ColorName = "Brown", Code = "#944c1c" });
            colorItems.Add(new ColorItem { ColorName = "DarkBrown", Code = "#643120" });
            colorItems.Add(new ColorItem { ColorName = "LightBrown", Code = "#a2693c" });
            colorItems.Add(new ColorItem { ColorName = "Pink", Code = "#f06cb9" });

            listView.Adapter = new ColorAdapter(this, colorItems);

            listView.ItemClick += (o, e) => 
                    {
                        //Comprobamos, si venimos de editTeam o de createTeam
                        //y actuamos en consecuencia
                        Type type = typeof(Screen_EditTeam);
                        if (teamId == 0)
                            type = typeof(Screen_CreateTeam);
                        var mainActivity = new Intent(this, type);
                        if (teamId == 0)
                            mainActivity.PutExtra("teamName", teamName);                        
                        else
                            mainActivity.PutExtra("teamId", teamId);
                        mainActivity.PutExtra("ColorCode", colorItems[e.Position].Code);
                        mainActivity.SetFlags(ActivityFlags.ClearTop);
                        StartActivity(mainActivity);
                    };
        }

        public override void OnBackPressed()
        {
            Type type = typeof(Screen_EditTeam);
            if (teamId == 0)
                type = typeof(Screen_CreateTeam);
            var mainActivity = new Intent(this, type);
            mainActivity.SetFlags(ActivityFlags.ClearTop);
            StartActivity(mainActivity);           
        }
}

    public class ColorAdapter : BaseAdapter<ColorItem>
    {
        readonly List<ColorItem> items;
        readonly Activity context;
        public ColorAdapter(Activity context, List<ColorItem> items)
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override ColorItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];

            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.ColorElement, null);
            view.FindViewById<TextView>(Resource.Id.ColorElement_nameColor).Text = item.ColorName;
            view.FindViewById<ImageView>(Resource.Id.ColorElement_imageColor).SetBackgroundColor(Android.Graphics.Color.ParseColor(item.Code));

            return view;
        }
    }


    public class ColorItem
    {
        public string ColorName { get; set; }
        public string Code { get; set; }
    }
}