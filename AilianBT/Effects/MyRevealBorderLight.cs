using System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;

namespace AilianBT.Effects
{
    public sealed class MyRevealBorderLight : XamlLight
    {
        #region IsTarget
        public static readonly DependencyProperty IsTargetProperty = DependencyProperty.RegisterAttached(
            "IsTarget",
            typeof(bool),
            typeof(MyRevealBorderLight), new PropertyMetadata(null, OnIsTargetChanged));
        public static void SetIsTarget(DependencyObject target, bool value)
        {
            target.SetValue(IsTargetProperty, value);
        }
        public static bool GetIsTarget(DependencyObject target)
        {
            return (bool)target.GetValue(IsTargetProperty);
        }
        private static void OnIsTargetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (obj is UIElement)
                {
                    XamlLight.AddTargetElement(GetIdStatic(), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.AddTargetBrush(GetIdStatic(), obj as Brush);
                }
            }
            else
            {
                if (obj is UIElement)
                {
                    XamlLight.RemoveTargetElement(GetIdStatic(), obj as UIElement);
                }
                else if (obj is Brush)
                {
                    XamlLight.RemoveTargetBrush(GetIdStatic(), obj as Brush);
                }
            }
        }
        #endregion

        #region Color
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(MyRevealBorderLight),
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
            var light = obj as MyRevealBorderLight;
            if (light.CompositionLight is PointLight pointLight)
            {
                pointLight.Color = (Color)e.NewValue;
            }
        }
        #endregion

        private SpotLight _light;

        protected override void OnConnected(UIElement newElement)
        {
            if (CompositionLight == null)
            {
                _light = _createLight();
                CompositionLight = _light;

                _createAnimation(Window.Current.Content, _light);
                _hookupWindowPointerHandlers();
            }
        }

        protected override void OnDisconnected(UIElement oldElement)
        {
            _hookupWindowPointerHandlers();

            if (CompositionLight != null)
            {
                CompositionLight.Dispose();
                CompositionLight = null;
            }
        }

        protected override string GetId()
        {
            return GetIdStatic();
        }

        public static string GetIdStatic()
        {
            return typeof(MyRevealBorderLight).FullName;
        }

        private CompositionPropertySet _offsetProps;
        private CompositionPropertySet _colorsProxy;

        private const string OFFSET_ANIMATION_EXPRESSION = "pointer.Position + Vector3(0,0,props.SpotlightHeight)";
        private const string OFFSET_ANIMATION_EXPRESSION_REFERENCE_POINTER = "pointer";
        private const string OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS = "props";
        private const string OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_NAME = "SpotlightHeight";
        private const float OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_VALUE = 128f;
        private const float OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_VALUE_WIDE = 192f;

        private SpotLight _createLight()
        {
            var spotLight = Window.Current.Compositor.CreateSpotLight();

            spotLight.InnerConeIntensity = 2;
            spotLight.OuterConeIntensity = -3.5f;
            spotLight.ConstantAttenuation = 2.7f;
            spotLight.LinearAttenuation = 0.1f;
            spotLight.QuadraticAttenuation = 0.1f;
            spotLight.InnerConeAngleInDegrees = 7f;
            spotLight.OuterConeAngleInDegrees = 27f;
            spotLight.InnerConeColor = Color.FromArgb(255, 255, 255, 255);
            spotLight.OuterConeColor = Color.FromArgb(255, 255, 255, 255);

            _offsetProps = Window.Current.Compositor.CreatePropertySet();
            _offsetProps.InsertScalar(OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_NAME, OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_VALUE);
            _offsetProps.InsertScalar("OuterAngleScale", 1f);

            _colorsProxy = _createSpotLightColorsProxy(spotLight);

            return spotLight;
        }

        private CompositionPropertySet _createSpotLightColorsProxy(SpotLight compositionSpotLight)
        {
            var lightProperties = compositionSpotLight.Properties;
            var compositor = compositionSpotLight.Compositor;

            lightProperties.InsertColor("InnerConeColor", Color.FromArgb(255, 255, 255, 255));
            lightProperties.InsertColor("OuterConeColor", Color.FromArgb(255, 255, 255, 255));
            lightProperties.InsertScalar("LightIntensity", 1f);

            var innerConeColorExpression = compositor.CreateExpressionAnimation("ColorLerp(ColorRgb(0,0,0,0), target.InnerConeColor, target.LightIntensity)");
            innerConeColorExpression.SetReferenceParameter("target", lightProperties);

            var outerConeColorExpression = compositor.CreateExpressionAnimation("ColorLerp(ColorRgb(0,0,0,0), target.OuterConeColor, target.LightIntensity)");
            outerConeColorExpression.SetReferenceParameter("target", lightProperties);

            compositionSpotLight.StartAnimation("InnerConeColor", innerConeColorExpression);
            compositionSpotLight.StartAnimation("OuterConeColor", outerConeColorExpression);

            return lightProperties;
        }

        private ExpressionAnimation _offsetAnimation;
        private void _createAnimation(UIElement attachedElement, CompositionObject compositionObject)
        {
            _offsetAnimation = Window.Current.Compositor.CreateExpressionAnimation(OFFSET_ANIMATION_EXPRESSION);
            _offsetAnimation.SetReferenceParameter(OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS, _offsetProps);

            var pointerProps = ElementCompositionPreview.GetPointerPositionPropertySet(attachedElement);
            _offsetAnimation.SetReferenceParameter(OFFSET_ANIMATION_EXPRESSION_REFERENCE_POINTER, pointerProps);

            compositionObject.StartAnimation("Offset", _offsetAnimation);
        }

        #region State
        public void GoToState(RevealBrushState state)
        {
            switch (state)
            {
                case RevealBrushState.Normal:
                    _pressed(false);
                    break;
                case RevealBrushState.PointerOver:
                    break;
                case RevealBrushState.Pressed:
                    _pressed(true);
                    break;
                default:
                    break;
            }
        }

        private void _pressed(bool isPressed)
        {
            Action animateSpotLight = () =>
            {
                var animation = _colorsProxy.Compositor.CreateScalarKeyFrameAnimation();
                animation.Duration = TimeSpan.FromMilliseconds(250);
                animation.InsertKeyFrame(1, isPressed 
                    ? OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_VALUE_WIDE 
                    : OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_VALUE);
                _offsetProps.StartAnimation(OFFSET_ANIMATION_EXPRESSION_REFERENCE_PROPS_SPOTLIGHTHEIGHT_NAME, animation);
            };
            animateSpotLight();
        }
        #endregion

        #region Pointer event
        private void _hookupWindowPointerHandlers()
        {
            Window.Current.CoreWindow.PointerEntered += _coreWindowPointerEntered;
            Window.Current.CoreWindow.PointerExited += _coreWindowPointerExited;
            //Window.Current.CoreWindow.PointerCaptureLost += _coreWindowPointerExited;
        }

        private void _unHookupWindowPointerHandlers()
        {
            Window.Current.CoreWindow.PointerEntered -= _coreWindowPointerEntered;
            Window.Current.CoreWindow.PointerExited -= _coreWindowPointerExited;
        }

        private void _coreWindowPointerEntered(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            _switchLight(true);
        }

        private void _coreWindowPointerExited(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            _switchLight(false);
        }

        private void _switchLight(bool turnOn)
        {
            Action animateSpotLight = () =>
            {
                var animation = _colorsProxy.Compositor.CreateScalarKeyFrameAnimation();
                animation.Duration = TimeSpan.FromMilliseconds(250);
                animation.InsertKeyFrame(1, turnOn ? 1f : 0f);
                _colorsProxy.StartAnimation("LightIntensity", animation);
            };

            if (turnOn)
            {
                //_light.IsEnabled = true;
                animateSpotLight();
            }
            else
            {
                CompositionScopedBatch scopedBatch = Window.Current.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                animateSpotLight();
                scopedBatch.End();
                scopedBatch.Completed += (o, e) =>
                {
                    //_light.IsEnabled = false;
                };
            }
        }
        #endregion
    }
}
