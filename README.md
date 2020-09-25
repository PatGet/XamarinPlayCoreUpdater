## Play.Core Library Bindings for Xamarin.Android
[![NuGet](https://img.shields.io/badge/Nuget-1.8.0-blue.svg)](https://www.nuget.org/packages/PlayCore/)

This repo contains a NuGet package that allows you to Support **in-app updates** and **in-app reviews** in your Xamarin Forms Android Apps.

## Screenshots

### In-App Review
![](Screenshots/93619419-8802e580-f9a6-11ea-9c80-920f8a3fb196.png?raw=true)

### In-App Updates

Update Prompt           |  Update in Progress
:-------------------------:|:-------------------------:
![](Screenshots/Screenshot_20200924-222055_Google%20Play%20Store.jpg?raw=true)  |  ![](Screenshots/Screenshot_20200924-222114_Google%20Play%20Store.jpg?raw=true)



## Download
- Install the package into your Android project (https://www.nuget.org/packages/PlayCore/)
 
## Implementation
- To support In-App Updates, you need to modify `MainActivity.cs` to initialise the IAppUpdateManager and define the OnSuccessListener. 
- To support In-App Reviews, you need to use Dependency Injection to call the Review workflow from your Shared Project. Add an interface in the Shared Project and implement that interface in your Android project.
- Although optional, using James Montemagno's Current Activity Plugin (https://github.com/jamesmontemagno/CurrentActivityPlugin) is recommended to retrieve the Context for the IReview object. Alternatively, the Sample uses a static object to pass the Context to the IAppReview implementation or you can use a singleton.

To make it easier for you, check the Sample project to see examples of implementations for both features.

## Google Guidelines
- See https://developer.android.com/guide/playcore/in-app-updates for more info on In-App Updates guidelines. 
- See https://developer.android.com/guide/playcore/in-app-review for info on In-App Reviews guidelines. 

## Possible Pitfalls
- You need to have the app submitted to at least an internal test track or internal app sharing to fully test the features. 

- If you use **Google App signing** (https://support.google.com/googleplay/android-developer/answer/7384423?hl=en), you can only test the update process with an app from the store and an update from the store. You can just disable automatic app updates, wait for the new version to appear in the Play Store and than manually start your App.

- If you test in-app reviews with an app submitted to Internal App Sharing, the **Submit** button for the review will be grayed out. This is also clearly stated in the Google docs. 


