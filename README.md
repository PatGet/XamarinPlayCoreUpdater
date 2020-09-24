[![NuGet](https://img.shields.io/badge/Nuget-1.0.0-blue.svg)](https://www.nuget.org/packages/PlayCore/)
# XamarinPlayCoreUpdater
This repo contains a NuGet package that allows you to Support **in-app updates** and **in-app reviews** in your Xamarin Forms Android Apps.
Supported and tested is the Immediate update variant for in-app updates. 

You need to have the app submitted to at least an internal test track or internal app sharing to fully test the features. 



See https://developer.android.com/guide/playcore/in-app-updates for more info on In-App Updates guidelines. 

See https://developer.android.com/guide/playcore/in-app-review for info on In-App Reviews guidelines. 
 
Please see the PlayCoreUpdateTest Sample for how to use the PlayCore Updater and Reviews. 

Possible Pitfall:
If you use "Google App signing https://support.google.com/googleplay/android-developer/answer/7384423?hl=en" you can only test the update process with an app from the store and an update from the store.
You can just disable automatic app updates, wait for the new version to appear in the Play Store and than manually start your App.

Possible Pitfall 2:
If you test in-app reviews with an app submitted to Internal App Sharing, the **Submit** button for the review will be grayed out. This is also clearly stated in the Google docs. 
