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
        public bool IsLocked { get; set; }
        public MemoryImage(string src, Image referencedImage)
        {
            Source = src;
            Image = referencedImage;
        }
    }
}
