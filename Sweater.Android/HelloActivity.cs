using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;

namespace Sweater.Android
{
    [Activity(Label = "HelloActivity"), Register("sweater_android.HelloActivity")]
    public class HelloActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Log.Debug("HELLO", "HELLO, WORLD!");

//            SetContentView(Resource.Layout.hello);
        }
    }
}