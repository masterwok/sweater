using Android.App;
using Android.OS;
using Android.Runtime;


namespace Sweater.Android
{
//    [Activity(Label = "Dummy", MainLauncher = true, Icon = "@drawable/icon")]
    [Activity(Label = "HelloActivity"), Register("sweater_android.HelloActivity")]
    public class HelloActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);
        }
    }
}