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
using AppGeoFit.BusinessLayer.Managers.PlayerManager;
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.DataAccesLayer.Data.GameRestService.Exceptions;
using AppGeoFit.Droid.Adapters;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class Screen_ActualGames : Screen
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ActualGames);
            Xamarin.Forms.Forms.Init(this, bundle);
            

            IPlayerManager playerManager = Xamarin.Forms.DependencyService.Get<IPlayerManager>().InitiateServices(false);

            int sportId = Intent.GetIntExtra("sportId", 0);

            AppSession appSession = new AppSession(ApplicationContext);
            Player player = appSession.getPlayer();

            ListView gameListView = FindViewById<ListView>(Resource.Id.ActualGames_GameList);
            List<Game> playerGamesList = new List<Game>();
            int rows = 11;
            int page = 0;
            try
            {
                playerGamesList = playerManager.GetActualGames(page, rows, player.PlayerId, sportId);
            }
            catch (GameNotFoundException ex){}
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
            }
            GameArrayAdapter adapter = new GameArrayAdapter(this, playerGamesList);
            gameListView.Adapter = adapter;
            int totalGamesCount = 0;
            gameListView.Scroll += (o, e) =>
            {
                if (!(gameListView.Adapter == null || gameListView.Adapter.Count == 0)
                    && gameListView.LastVisiblePosition >= gameListView.Count - 1)
                {
                    try
                    {
                        totalGamesCount = playerManager.TotalGamesCount(player.PlayerId, sportId);
                    }
                    catch (Exception ex) {
                        Toast.MakeText(ApplicationContext,
                            ex.Message, ToastLength.Short).Show();
                    }                    
                    if (totalGamesCount > gameListView.Count)
                    {
                        page = (int)Math.Ceiling((double)totalGamesCount / gameListView.LastVisiblePosition) - 1;
                        try
                        {
                            playerGamesList.AddRange(playerManager.GetActualGames(page, rows, player.PlayerId, sportId));
                        }
                        catch (GameNotFoundException) { }
                        catch(Exception ex)
                        {
                            Toast.MakeText(ApplicationContext,
                           ex.Message, ToastLength.Short).Show();
                        }
                        adapter = new GameArrayAdapter(this, playerGamesList);
                        gameListView.Adapter = adapter;
                    }
                }

            };

            gameListView.ItemClick += (o, e) =>
            {
                int gameSelectedOnClickId = gameListView.GetItemAtPosition(e.Position).GetHashCode();
                var screen_GameDetails = new Intent(this, typeof(Screen_GameDetails));
                screen_GameDetails.PutExtra("gameId", gameSelectedOnClickId);
                StartActivity(screen_GameDetails);
            };
        }
    }
}