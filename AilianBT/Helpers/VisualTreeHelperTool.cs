using System.Collections.Generic;
using Windows.UI.Xaml;

namespace AilianBT.Helpers
{
    public class VisualTreeHelperTool
    {
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(obj);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(item);
                for (int i = 0; i < count; i++)
                {
                    var re = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(item, i);       
                    if (re is T reT)
                    {
                        return reT;
                    }
                    else
                    {
                        queue.Enqueue(re);
                    }
                }
            }
            return null;
        }

        public static T FindNamedVisualChild<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(obj);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(item);
                for (int i = 0; i < count; i++)
                {
                    var re=Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(item, i);
                    var child=re as FrameworkElement;
                    if (child != null && child.Name == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            return null;
        }

        public static VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
                return null;
            IList<VisualStateGroup> groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (var group in groups)
            {
                if (group.Name == name)
                    return group;
            }
            return null;
        }
    }
}
