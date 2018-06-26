using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Memory
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        TapGestureRecognizer tapRecognizer;
        bool won;
        int timeLeft;
        int imgPairsLeft;
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
        public int TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = value;
                OnPropertyChanged();
            }
        }
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            tapRecognizer = GetNewTapRecognizer();
            InitImages(GetSourceStringQueue());
            Device.BeginInvokeOnMainThread(() => DisplayAlert("Memory", "RULES:\n\t- you have limited time to find matching pairs"
                          + "\n\t- every unmatching pair will cost you 1 second of delay"
                          + "\n\t- every correct pair will get you 3 seconds extra", "PLAY")
                          .ContinueWith((x) => Device.BeginInvokeOnMainThread(() => StartNewGame())));
        }

        public void StartNewGame()
        {
            previouslySelected = null;
            won = false;
            imgPairsLeft = sourceStrings.Length;
            TimeLeft = 30;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (won)
                    return false;
                else if (TimeLeft == 0 && imgPairsLeft > 0)
                {
                    Task.Run(async () => await GameLost());
                    return false;
                }
                TimeLeft--;
                return true;
            });

        }
        private void PlayAgain()
        {
            InitImages(GetSourceStringQueue());
            StartNewGame();
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
            Images.Clear();
            var gridImages = GetRandomizedGridImageChildren();

            foreach (var item in gridImages)
            {
                string source = src.Dequeue();
                InitMemoryImage(item, source, tapRecognizer);
                src.Enqueue(source);
            }
        }
        private List<Image> GetRandomizedGridImageChildren()
        {
            var gridImageChildren = MainGrid.Children.Where(x => x is Image).Select(x => x as Image).ToList();
            gridImageChildren.ForEach(x => x.IsEnabled = true);
            gridImageChildren.Shuffle();
            return gridImageChildren;
        }
        private TapGestureRecognizer GetNewTapRecognizer()
        {
            var tmp = new TapGestureRecognizer();
            tmp.Tapped += ImageTap;
            return tmp;
        }
        private void InitMemoryImage(Image img, string src, TapGestureRecognizer tapRecognizer)
        {
            Images.Add(new MemoryImage(src, img));
            img.GestureRecognizers.Clear();
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
                        imgPairsLeft--;
                        selected.Disable();
                        previouslySelected.Disable();
                        TimeLeft += 3;
                        if (imgPairsLeft == 0)
                        {
                            await GameWon();
                        }
                    }
                    else
                    {
                        await HideWithDelay(selected, previouslySelected);
                    }

                    previouslySelected = null;
                }


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
        private async Task GameWon()
        {
            won = true;
            var result = await DisplayAlert("Memory", "YOU WON!", "PLAY AGAIN", "EXIT");
            if (result == true)
                Device.BeginInvokeOnMainThread(() => PlayAgain());
            else
                DependencyService.Get<ICloseApplication>()?.ExitApp();

        }
        private async Task GameLost()
        {
            var result = await DisplayAlert("Memory", "YOU LOST!", "PLAY AGAIN", "EXIT");
            if (result == true)
                Device.BeginInvokeOnMainThread(() => PlayAgain());
            else
                DependencyService.Get<ICloseApplication>()?.ExitApp();
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
