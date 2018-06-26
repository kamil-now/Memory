using Xamarin.Forms;

[assembly: Dependency(typeof(Memory.UWP.CloseApplication))]
namespace Memory.UWP
{
    public class CloseApplication : ICloseApplication
    {
        public void ExitApp()
        {
            Windows.UI.Xaml.Application.Current.Exit();
        }
    }
}
