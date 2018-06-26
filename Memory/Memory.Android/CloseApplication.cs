using Android.OS;
using Xamarin.Forms;

[assembly: Dependency(typeof(Memory.Droid.CloseApplication))]
namespace Memory.Droid
{

    public class CloseApplication : ICloseApplication
    {
        public void ExitApp()
        {
            Process.KillProcess(Process.MyPid());
        }
    }
}