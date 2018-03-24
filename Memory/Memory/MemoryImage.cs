using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Memory
{
    public class MemoryImage
    {
        public string Source { get; }
        public Image Image { get; set; }

        public MemoryImage(string src, Image referencedImage)
        {
            Source = src;
            Image = referencedImage;
        }

        public void Enable() => Image.IsEnabled = true;
        public void Disable() => Image.IsEnabled = false;
        public void Uncover()
        {
            Image.Source = Source;
            Disable();
        }
        public void Cover(string src)
        {
            Image.Source = src;
            Enable();
        }
        public bool IsTheSameAs(MemoryImage img)
        {
            return this.Source == img?.Source && this.Image != img?.Image;
        }

    }
}
