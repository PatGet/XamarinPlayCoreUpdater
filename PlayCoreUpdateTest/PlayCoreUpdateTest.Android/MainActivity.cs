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
using Com.Google.Android.Play.Core.Appupdate.Testing;

namespace PlayCoreUpdateTest.Droid
{
    [Activity(Label = "PlayCoreUpdateTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        private const int _Request_Update = 4711;
        private const int APP_UPDATE_TYPE_SUPPORTED = AppUpdateType.Immediate;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Instance = this;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

#if DEBUG
            var appUpdateManager = new FakeAppUpdateManager(this);
            /* The below line of code will trigger the fake app update manager which it will display the alert dialog 
            Let say if we comment this line of code to simulate update is not available then the play core update not available flag
            will be captured on the appupdatesuccess listener.
            If comment this line it will simulate if the app update is not available. Then you can add logic when update is not available using immeidate update*/
            appUpdateManager.SetUpdateAvailable(2); // your higher app version code that can be used to test fakeappupdate manager
#else       // The below line of code will execute in release configuration
            IAppUpdateManager appUpdateManager = AppUpdateManagerFactory.Create(this);
#endif
            var appUpdateInfoTask = appUpdateManager.AppUpdateInfo;
            appUpdateInfoTask.AddOnSuccessListener(new AppUpdateSuccessListener(appUpdateManager, this, _Request_Update, Intent));
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        /* If you call the update manager within the onresume method the update loop occurs since it will check for update every time when the
        application is back to the foreground. */
        protected override void OnResume()
        {
            base.OnResume();
            //#if DEBUG
            //            var appUpdateManager = new FakeAppUpdateManager(this);
            //            /* The below line of code will trigger the fake app update manager which it will display the alert dialog 
            //            Let say if we comment this line of code to simulate update is not available then the play core update not available flag
            //            will be captured on the appupdatesuccess listener.
            //            If comment this line it will simulate if the app update is not available. Then you can add logic when update is not available using immeidate update*/
            //            appUpdateManager.SetUpdateAvailable(2);
            //#else       // The below line of code will execute in release configuration
            //            IAppUpdateManager appUpdateManager = AppUpdateManagerFactory.Create(this);
            //#endif
            //            var appUpdateInfoTask = appUpdateManager.AppUpdateInfo;
            //            appUpdateInfoTask.AddOnSuccessListener(new AppUpdateSuccessListener(appUpdateManager, this, _Request_Update, Intent));
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (_Request_Update.Equals(requestCode))
            {
                switch (resultCode) // The switch block will be triggered only with flexible update since it returns the install result codes
                {
                    case Result.Ok:
                        // In app update success
                        if (APP_UPDATE_TYPE_SUPPORTED == AppUpdateType.Immediate)
                        {
                            Toast.MakeText(this, "App updated", ToastLength.Short).Show();
                        }
                        break;
                    case Result.Canceled:
                        Toast.MakeText(this, "In app update cancelled", ToastLength.Short).Show();
                        break;
                    case (Result)ActivityResult.ResultInAppUpdateFailed:
                        Toast.MakeText(this, "In app update failed", ToastLength.Short).Show();
                        break;
                }
            }
            else // Here we add our custom code since immediate update will not return a callback result code
            {
                AlertDialog.Builder dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetMessage("Hello Xamarin. Additional instructions");
                alert.SetCancelable(false);
                alert.SetButton((int)DialogButtonType.Positive, "Ok", (o, args) =>
                {
                    alert.Dismiss();
                });
                alert.Show();
            }
        }
    }

    public class AppUpdateSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        private readonly IAppUpdateManager _appUpdateManager;
        private readonly Activity _mainActivity;
        private readonly int _update_request;
        private Intent _intent;

        public AppUpdateSuccessListener(IAppUpdateManager appUpdateManager, Activity mainActivity, int update_request, Intent intent)
        {
            _appUpdateManager = appUpdateManager;
            _mainActivity = mainActivity;
            _update_request = update_request;
            _intent = intent;
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

#if DEBUG
                var fakeAppUpdate = _appUpdateManager as FakeAppUpdateManager;
                if (fakeAppUpdate.IsImmediateFlowVisible)
                {
                    fakeAppUpdate.UserAcceptsUpdate();
                    fakeAppUpdate.DownloadStarts();
                    fakeAppUpdate.DownloadCompletes();
                    LaunchRestartDialog(_appUpdateManager);
                }
#endif
            }

            if (availability.Equals(UpdateAvailability.UpdateNotAvailable) || availability.Equals(UpdateAvailability.Unknown))
            {
                Log.Debug("UPDATE NOT AVAILABLE", $"{info.AvailableVersionCode()}");
                // You can start your activityonresult method when update is not available when using immediate update
                _mainActivity.StartActivityForResult(_intent, 400); // You can use any random result code
            }
        }

        // The restart dialog was only used to test the fakeappupdatemanager
        private void LaunchRestartDialog(IAppUpdateManager appUpdateManager)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(_mainActivity);
            AlertDialog alert = dialog.Create();
            alert.SetMessage("Application successfully updated! You need to restart the app in order to use this new features");
            alert.SetCancelable(false);
            alert.SetButton((int)DialogButtonType.Positive, "Restart", (o, args) =>
            {
                appUpdateManager.CompleteUpdate();
                // You can start your activityonresult method when update is not available when using immediate update when testing with fakeappupdate manager
                //_mainActivity.StartActivityForResult(_intent, 400);
            });
            alert.Show();
        }
    }
}