using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AilianBT.Effects
{
    public sealed class MyRevealBorderBrush : XamlCompositionBrushBase
    {
        private RevealBrush _revealBrush = new RevealBrush();

        public static MyRevealBorderLight RevealBorderLight { get; private set; }
        
        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {
                CompositionBrush = _revealBrush.CreateRevealBrush(GetColor(this));
                _attachLights();
            }
        }

        protected override void OnDisconnected()
        {
            if (CompositionBrush != null)
            {
                _unAttachLights();

                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        #region Color
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(MyRevealBorderBrush),
            new PropertyMetadata(null, OnColorChanged)
        );
        public static void SetColor(DependencyObject target, Color value)
        {
            target.SetValue(ColorProperty, value);
        }
        public static Color GetColor(DependencyObject target)
        {
            return (Color)target.GetValue(ColorProperty);
        }
        private static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is MyRevealBorderBrush brush)
            {
                brush._revealBrush?.SetColor((Color)e.NewValue);
            }
        }
        #endregion

        #region State
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(
            "State",
            typeof(RevealBrushState),
            typeof(MyRevealBorderBrush),
            new PropertyMetadata(null, OnStateChanged)
        );
        public static void SetState(DependencyObject target, RevealBrushState value)
        {
            target.SetValue(StateProperty, value);
        }
        public static RevealBrushState GetState(DependencyObject target)
        {
            return (RevealBrushState)target.GetValue(StateProperty);
        }
        private static void OnStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            RevealBorderLight?.GoToState(e.NewValue != null ? (RevealBrushState)e.NewValue : RevealBrushState.Normal);
        }
        #endregion

        private void _attachLights()
        {
            bool hasRevealBorderLight = false;
            var element = Window.Current.Content;
            foreach (var item in element.Lights)
            {
                if(item is MyRevealBorderLight revealLight)
                {
                    RevealBorderLight = revealLight;
                    hasRevealBorderLight = true;
                    break;
                }
            }

            if(!hasRevealBorderLight)
            {
                RevealBorderLight = new MyRevealBorderLight();
                element.Lights.Add(RevealBorderLight);
            }
            MyRevealBorderLight.AddTargetBrush(MyRevealBorderLight.GetIdStatic(), this);
        }

        private void _unAttachLights()
        {
            MyRevealBorderLight.RemoveTargetBrush(MyRevealBorderLight.GetIdStatic(), this);
        }
    }
}
