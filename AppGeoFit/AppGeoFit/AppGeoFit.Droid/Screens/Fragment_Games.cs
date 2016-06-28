using Android.OS;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Android.Content;

namespace AppGeoFit.Droid.Screens
{
    public class Fragment_Games : Fragment
    {
        int actualSportId;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle bundle)
        {
            base.OnCreateView(inflater, container, bundle);

            View view = inflater.Inflate(Resource.Layout.Games, container, false);

            ImageButton createGameButton = view.FindViewById<ImageButton>(Resource.Id.Games_createGame);
            //Recuperamos el id del deporte actual.
            Spinner spinnerFavoriteSport_et = this.Activity.FindViewById<Spinner>(Resource.Id.Toolbar_spinnerSports);
            actualSportId = spinnerFavoriteSport_et.SelectedItem.GetHashCode();
            createGameButton.Click += (o, e) =>
            {
                var screen_CreateGame_Captain = new Intent(Context, typeof(Screen_CreateGame));
                screen_CreateGame_Captain.PutExtra("sportId", actualSportId);
                Activity.StartActivity(screen_CreateGame_Captain);
            };
            return view;
        }

    }
}