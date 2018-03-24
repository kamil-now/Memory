using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Memory
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        string defaultImageSrc = "icon.png";
        string csharp = "csharp.png";
        List<MemoryImage> Images = new List<MemoryImage>();
        //{
        //    { new MemoryImage("csharp.png") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //    { new MemoryImage("") },
        //};
        List<Image> images = new List<Image>();

        public MainPage()
        {
            InitializeComponent();
            InitImages();
        }
        private void ImageTap(object sender, EventArgs context)
        {
            var img = (sender as Image);
            var tmp = Images.FirstOrDefault(x => x?.Image == img);
            img.Source = tmp?.Source ?? defaultImageSrc;
        }
        private void InitImages()
        {
            images = MainGrid.Children.Where(x => x is Image).Select(x => x as Image).ToList();
            images.Shuffle();
            var tapRecognizer = GetNewTapRecognizer();
            foreach (var item in images)
            {
                if (Images.Count == 0)
                    Images.Add(new MemoryImage(csharp) { Image = item });
                item.GestureRecognizers.Add(tapRecognizer);
                item.Source = defaultImageSrc;
            }
        }

        private TapGestureRecognizer GetNewTapRecognizer()
        {
            var tmp = new TapGestureRecognizer();
            tmp.Tapped += ImageTap;
            return tmp;

        }
    }
    public static class Extensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rand = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
