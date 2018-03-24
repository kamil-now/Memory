using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Memory
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        string defaultImageSrc = "icon.png";
        List<MemoryImage> Images = new List<MemoryImage>();
        string[] sourceStrings = new string[]
        {
            "csharp.png",
            "cplusplus.png",
            "swift.png",
            "java.png",
            "kotlin.png",
            "python.png",
            "ruby.png",
            "rust.png",
        };
        MemoryImage previouslySelected;
        private bool waitingToHidePictures;
        public MainPage()
        {
            InitializeComponent();

            var imageSourceStrings = GetSourceStack();
            InitImages(imageSourceStrings);
        }

        private async void ImageTap(object sender, EventArgs context)
        {
            if (!waitingToHidePictures)
            {
                var img = (sender as Image);
                var selected = Images.First(x => x?.Image == img);
                img.Source = selected.Source;
                if (!selected.IsLocked)
                {
                    if (previouslySelected == null)
                    {
                        previouslySelected = selected;
                    }
                    else
                    {
                        if (selected.Image == previouslySelected.Image)
                        {
                            selected.Image.Source = defaultImageSrc;
                        }
                        else if (selected.Source == previouslySelected.Source)
                        {
                            selected.IsLocked = true;
                            previouslySelected.IsLocked = true;
                        }
                        else
                        {
                            waitingToHidePictures = true;
                            await Task.Delay(2000);
                            previouslySelected.Image.Source = defaultImageSrc;
                            selected.Image.Source = defaultImageSrc;
                            waitingToHidePictures = false;
                        }
                        previouslySelected = null;
                    }
                }
            }
        }


        private Queue<string> GetSourceStack()
        {
            var tmp = new Queue<string>();
            foreach (var item in sourceStrings)
            {
                tmp.Enqueue(item);
            }
            return tmp;
        }
        private void InitImages(Queue<string> src)
        {
            var images = GetRandomizedImageList();
            var tapRecognizer = GetNewTapRecognizer();
            foreach (var item in images)
            {
                string source = src.Dequeue();
                InitImage(item, source, tapRecognizer);
                src.Enqueue(source);
            }
        }
        private void InitImage(Image img, string src, TapGestureRecognizer tapRecognizer)
        {
            Images.Add(new MemoryImage(src, img));

            img.GestureRecognizers.Add(tapRecognizer);
            img.Source = defaultImageSrc;
        }
        private List<Image> GetRandomizedImageList()
        {
            var img = MainGrid.Children.Where(x => x is Image).Select(x => x as Image).ToList();
            img.Shuffle();
            return img;
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
