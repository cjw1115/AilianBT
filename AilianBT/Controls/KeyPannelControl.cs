using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace AilianBT.Controls
{
    public sealed class KeyPannelControl : Panel
    {
        public KeyPannelControl()
        {
            
        }
        private List<Point> list = new List<Point>();
        protected override Size ArrangeOverride(Size finalSize)
        {
            var count = this.Children.Count;
            for (int i = 0; i < count; i++)
            {
                var child = Children[i];
                child.Arrange(new Rect(list[i], child.DesiredSize));
            }

            return finalSize;
        }

        
        protected override Size MeasureOverride(Size availableSize)
        {
            this.list?.Clear();
            int row = 1;
            double rowheight = 0;

            var x = 0d;
            var y = 0d;
            var count = this.Children.Count;
            for (int i = 0; i < count; i++)
            {
                var child = Children[i];
                child.Measure(availableSize);
                
                if (child.DesiredSize.Width + x < availableSize.Width)
                {
                    list.Add(new Point { X = x, Y = y });
                    x += child.DesiredSize.Width;
                }
                else
                {
                    
                    row++;
                    x = 0;
                    y += child.DesiredSize.Height;

                    list.Add(new Point { X = x, Y = y });
                    
                    x += child.DesiredSize.Width + 8;
                }
                

                rowheight = child.DesiredSize.Height;
            }

            var totalHeight = row * rowheight;
            return new Size(availableSize.Width, totalHeight);
        }
    }
}
