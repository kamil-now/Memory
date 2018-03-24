using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Memory.UWP
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new Memory.App());
        }
    }
}