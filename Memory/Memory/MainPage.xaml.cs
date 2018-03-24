using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Memory
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        int defaultDelay = 1000;
        string defaultImageSrc = "def.png";
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
            ResetGame();
        }

        public void ResetGame()
        {
            var imageSourceStrings = GetSourceStringQueue();
            InitImages(imageSourceStrings);
        }
        private Queue<string> GetSourceStringQueue()
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
        private List<Image> GetRandomizedImageList()
        {
            var imageList = MainGrid.Children.Where(x => x is Image).Select(x => x as Image).ToList();
            imageList.Shuffle();
            return imageList;
        }
        private TapGestureRecognizer GetNewTapRecognizer()
        {
            var tmp = new TapGestureRecognizer();
            tmp.Tapped += ImageTap;
            return tmp;
        }
        private void InitImage(Image img, string src, TapGestureRecognizer tapRecognizer)
        {
            Images.Add(new MemoryImage(src, img));
            img.GestureRecognizers.Add(tapRecognizer);
            img.Source = defaultImageSrc;
        }

        private async void ImageTap(object sender, EventArgs context)
        {
            if (!waitingToHidePictures)
            {
                var selected = GetAssociatedObject(sender);

                selected.Uncover();

                if (previouslySelected == null)
                {
                    previouslySelected = selected;
                }
                else
                {
                    if (selected.IsTheSameAs(previouslySelected))
                    {
                        selected.Disable();
                        previouslySelected.Disable();
                    }
                    else
                    {
                        await HideWithDelay(selected, previouslySelected);
                    }
                    previouslySelected = null;
                }

                selected.Enable();
            }
        }
        private async Task HideWithDelay(MemoryImage first, MemoryImage second)
        {
            waitingToHidePictures = true;

            await Task.Delay(defaultDelay);

            first.Cover(defaultImageSrc);
            second.Cover(defaultImageSrc);

            waitingToHidePictures = false;
        }
        private MemoryImage GetAssociatedObject(object sender)
        {
            var img = (sender as Image);
            return Images.First(x => x?.Image == img);
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
