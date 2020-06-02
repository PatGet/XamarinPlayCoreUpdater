using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Google.Android.Play.Core.Appupdate;
using Android.Content;
using Android.Util;
using Com.Google.Android.Play.Core.Install.Model;
using Com.Google.Android.Play.Core.Tasks;

namespace PlayCoreUpdateTest.Droid
{
    [Activity(Label = "PlayCoreUpdateTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();

            Activity _activity = this;
            int _Request_Update = 4711;
            IAppUpdateManager appUpdateManager = AppUpdateManagerFactory.Create(this);
            var appUpdateInfoTask = appUpdateManager.AppUpdateInfo;
            appUpdateInfoTask.AddOnSuccessListener(new AppUpdateSuccessListener(appUpdateManager, _activity, _Request_Update));
        }
    }

    public class AppUpdateSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private readonly IAppUpdateManager _appUpdateManager;
        private readonly Activity _mainActivity;
        private readonly int _update_request;

        public AppUpdateSuccessListener(IAppUpdateManager appUpdateManager, Activity mainActivity, int update_request)
        {
            _appUpdateManager = appUpdateManager;
            _mainActivity = mainActivity;
            _update_request = update_request;
        }

        public void OnSuccess(Java.Lang.Object p0)
        {
            if (!(p0 is AppUpdateInfo info))
            {
                return;
            }

            Log.Debug("AVAILABLE VERSION CODE", $"{info.AvailableVersionCode()}");



            var availability = info.UpdateAvailability();
            if ((availability.Equals(UpdateAvailability.UpdateAvailable) || availability.Equals(UpdateAvailability.DeveloperTriggeredUpdateInProgress)) && info.IsUpdateTypeAllowed(AppUpdateType.Immediate))
            {

                // Start an update
                _appUpdateManager.StartUpdateFlowForResult(info, AppUpdateType.Immediate, _mainActivity, _update_request);
            }

            //#if DEBUG
            //            var fakeAppUpdate = _appUpdateManager as FakeAppUpdateManager;
            //            if (fakeAppUpdate.IsImmediateFlowVisible)
            //            {
            //                fakeAppUpdate.UserAcceptsUpdate();
            //                fakeAppUpdate.DownloadStarts();
            //                fakeAppUpdate.DownloadCompletes();
            //                LaunchRestartDialog(_appUpdateManager);
            //            }
            //#endif
        }

        private void LaunchRestartDialog(IAppUpdateManager appUpdateManager)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(_mainActivity);
            AlertDialog alert = dialog.Create();
            alert.SetMessage("Application successfully updated! You need to restart the app in order to use this new features");
            alert.SetCancelable(false);
            alert.SetButton((int)DialogButtonType.Positive, "Restart", (o, args) =>
            {
                appUpdateManager.CompleteUpdate();
            });
            alert.Show();
        }
    }
}