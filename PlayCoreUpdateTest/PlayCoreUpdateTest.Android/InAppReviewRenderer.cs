using System;
using Com.Google.Android.Play.Core.Tasks;
using Com.Google.Android.Play.Core.Review;
using Com.Google.Android.Play.Core.Review.Testing;

[assembly: Xamarin.Forms.Dependency(typeof(PlayCoreUpdateTest.Droid.InAppReviewRenderer))]
namespace PlayCoreUpdateTest.Droid
{
    public class InAppReviewRenderer : IInAppReview
    {
        public void LaunchReview()
        {
#if DEBUG
            var manager = new FakeReviewManager(MainActivity.Instance);
#else         
            var manager = ReviewManagerFactory.Create(MainActivity.Instance);
#endif            
            var request = manager.RequestReviewFlow();
            request.AddOnCompleteListener(new OnCompleteListener(manager));
        }
    }

    public class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        FakeReviewManager _fakeReviewManager;
        IReviewManager _reviewManager;
        bool _usesFakeManager;
        void IOnCompleteListener.OnComplete(Com.Google.Android.Play.Core.Tasks.Task p0)
        {
            if (p0.IsSuccessful)
            {
                var review = p0.GetResult(Java.Lang.Class.FromType(typeof(ReviewInfo)));
                if (_usesFakeManager)
                {
                    var x = _fakeReviewManager.LaunchReviewFlow(MainActivity.Instance, (ReviewInfo)review);
                    x.AddOnCompleteListener(new OnCompleteListener(_fakeReviewManager));
                }
                else
                {
                    var x = _reviewManager.LaunchReviewFlow(MainActivity.Instance, (ReviewInfo)review);
                    x.AddOnCompleteListener(new OnCompleteListener(_reviewManager));
                }
            }
        }
        public OnCompleteListener(FakeReviewManager fakeReviewManager)
        {
            _fakeReviewManager = fakeReviewManager;
            _usesFakeManager = true;
        }
        public OnCompleteListener(IReviewManager reviewManager)
        {
            _reviewManager = reviewManager;
        }
    }
}
