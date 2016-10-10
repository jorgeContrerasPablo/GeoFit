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
using AppGeoFit.DataAccesLayer.Models;
using AppGeoFit.BusinessLayer.Managers.FeedBackManager;
using AppGeoFit.Droid.Adapters;
using AppGeoFit.DataAccesLayer.Data.FeedBackRestService.Exceptions;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;

namespace AppGeoFit.Droid.Screens
{
    [Activity(Icon = "@drawable/icon", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    class Screen_Comments : Screen
    {
        Player actualPlayer;
        int commentPlayerId;
        int commentPlaceId;
        int commentGameId;
        IFeedBackManager feedBackManager;
        CommentArrayAdapter adapterLComment;
        List<FeedBack> commentList = new List<FeedBack>();
        ListView commentListView;
        int rows = 11;
        int page = 0;
        Drawable errorD;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Comments);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            AppSession appSession = new AppSession(this.ApplicationContext);
            actualPlayer = appSession.getPlayer();

            feedBackManager = Xamarin.Forms.DependencyService.Get<IFeedBackManager>().InitiateServices(false);

            commentPlayerId = Intent.GetIntExtra("playerId", 0);
            commentPlaceId = Intent.GetIntExtra("placeId", 0);
            commentGameId = Intent.GetIntExtra("gameId", 0);

            commentListView = FindViewById<ListView>(Resource.Id.Comments_commentsList);

            updateCommentList();

            //Se crea el icono exclamation_error
            errorD = ContextCompat.GetDrawable(this, Resource.Drawable.exclamation_error);
            errorD.SetBounds(0, 0, errorD.IntrinsicWidth, errorD.IntrinsicHeight);

            int totalCommentsCount = 0;
            commentListView.Scroll += (o, e) =>
            {
                if (!(commentListView.Adapter == null || commentListView.Adapter.Count == 0)
                    && commentListView.LastVisiblePosition >= commentListView.Count - 1)
                {
                    try
                    {
                        if (commentPlayerId != 0)
                            totalCommentsCount = feedBackManager.TotalPlayerCommentsCount(commentPlayerId);
                        if (commentPlaceId != 0)
                            totalCommentsCount = feedBackManager.TotalPlaceCommentsCount(commentPlaceId);
                        if (commentGameId != 0)
                            totalCommentsCount = feedBackManager.TotalGameCommentsCount(commentGameId);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                    }
                    if (totalCommentsCount > commentListView.Count)
                    {
                        page = (int)Math.Ceiling((double)totalCommentsCount / commentListView.LastVisiblePosition) - 1;
                        updateCommentList();
                    }
                }
            };
        }
        //Mostramos las peticiones pendientes si es que las hay.
        void updateCommentList()
        {
            try
            {
                if (commentPlayerId != 0)
                    commentList = feedBackManager.GetPlayerCommentsPagination(page, rows, commentPlayerId);
                if (commentPlaceId != 0)
                    commentList = feedBackManager.GetPlaceCommentsPagination(page, rows, commentPlaceId);
                if (commentGameId != 0)
                    commentList = feedBackManager.GetGameCommentsPagination(page, rows, commentGameId);
                adapterLComment = new CommentArrayAdapter(
                this, commentList);
                commentListView.Adapter = adapterLComment;
                RegisterForContextMenu(commentListView);
            }
            catch (FeedBackNotFoundException ex) { }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override void OnCreateContextMenu(IContextMenu menu, View vValue, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, vValue, menuInfo);
            
            var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
            FeedBack selectedFeedBack = new FeedBack();
            try
            {
                selectedFeedBack = feedBackManager.GetFeedBack(commentListView.Adapter.GetItem(info.Position).GetHashCode());
            }
            catch (FeedBackNotFoundException ex)
            {
                Toast.MakeText(ApplicationContext,ex.Message, ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
            }

            MenuInflater inflater = new MenuInflater(this);
            if(selectedFeedBack.CreatorID == actualPlayer.PlayerId)
            {
                inflater.Inflate(Resource.Menu.MenuComment, menu);
            }            
        }
        public override bool OnContextItemSelected(IMenuItem item)
        { 
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            FeedBack feedBackSelected = adapterLComment.GetItem(info.Position);
            switch (item.ItemId)
            {
                case Resource.Id.MenuCommentEdit:
                    AlertDialog dialogEditComment;
                    dialogEditComment = CreateAlertDialog(Resource.Layout.dialog_EditComment, this);
                    dialogEditComment.Show();
                    EditText comment = dialogEditComment.FindViewById<EditText>(Resource.Id.D_EditComment_Comment);
                    comment.Text = feedBackSelected.Description;
                    Button acceptButton = dialogEditComment.FindViewById<Button>(Resource.Id.D_EditComment_AcceptB);
                    Button cancelButton = dialogEditComment.FindViewById<Button>(Resource.Id.D_EditComment_CancelB);
                    bool commentN = false;
                    acceptButton.Click += (o, e) =>
                    {
                        commentN = IsRequired(comment, "Insert a comment please", errorD);
                        if (!commentN)
                        {
                            feedBackSelected.Description = comment.Text;
                            try
                            {
                                feedBackManager.UpdateFeedBack(feedBackSelected);
                                updateCommentList();
                            }
                            catch(Exception ex)
                            {
                                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                            }
                            dialogEditComment.Cancel();
                        }
                    };
                    cancelButton.Click += (o, e) =>
                    {
                        dialogEditComment.Cancel();
                        try
                        {
                            updateCommentList();
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                        }
                    };
                    return true;
                case Resource.Id.MenuCommentDelete:
                    AlertDialog baDelete;
                    Button baDeletePositiveButton;
                    Button baDeleteNegativeButton;
                    baDelete = BotonAlert("Alert", "Do you want to delete this comment?", "OK", "Cancel", this);
                    baDelete.Show();
                    baDeletePositiveButton = baDelete.GetButton((int)DialogButtonType.Positive);
                    baDeleteNegativeButton = baDelete.GetButton((int)DialogButtonType.Negative);
                    baDeletePositiveButton.Click += (oPB, ePB) =>
                    {
                        feedBackSelected.Description = null;
                        try
                        {
                            updateCommentList();
                        }
                        catch (Exception ex)
                        {
                            Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Short).Show();
                        }
                        baDelete.Cancel();
                    };
                    baDeleteNegativeButton.Click += (oNB, eNB) =>
                    {
                        baDelete.Cancel();
                    };
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }

        }
    }
}