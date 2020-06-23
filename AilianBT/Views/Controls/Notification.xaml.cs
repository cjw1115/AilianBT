using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AilianBT.Views.Controls
{
    public sealed partial class Notification : UserControl
    {
        public Notification()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string NotifyMessage
        {
            get { return (string)this.GetValue(NotifyMessageProperty); }
            set { SetValue(NotifyMessageProperty, value); }
        }
        public static readonly DependencyProperty NotifyMessageProperty = DependencyProperty.Register("NotifyMessage", typeof(string), typeof(Notification),null);


        public void Show()
        {
            Storyboard.Begin();
        }
    }
}
