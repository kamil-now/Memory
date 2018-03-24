using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Memory
{
    public class MemoryImage
    {
        public string Source { get; }
        public bool IsVisible { get; set; }
        public Image Image { get; set; }
        public MemoryImage(string src)
        {
            Source = src;
        }
    }
}
