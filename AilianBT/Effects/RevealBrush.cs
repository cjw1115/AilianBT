using Microsoft.Graphics.Canvas.Effects;
using System.Collections.Generic;
using Windows.Graphics.Effects;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace AilianBT.Effects
{
    public class RevealBrush
    {
        private Matrix5x4 sc_revealInvertedBorderColorMatrix = new Matrix5x4
        {
            M11 = -2.0f, M12 = 0.0f,M13 = 0.0f,M14 = 2 * .2125f,
            M21 = 0.0f,M22 = -2.0f,M23 = 0.0f,M24 = 2 * .7154f,
            M31 = 0.0f,M32 = 0.0f,M33 = -2.0f,M34 = 2 * .0721f,
            M41 = 0.0f,
            M42 = 0.0f,
            M43 = 0.0f,
            M44 = 0.0f,
            M51 = 0.0f,
            M52 = 0.0f,
            M53 = 0.0f,
            M54 = 0.0f
        };

        private const float sc_diffuseAmountBorder = 0.2f;
        private const float sc_specularAmountBorder = 0f;
        private const float sc_specularShineBorder = 0f;

        private IGraphicsEffect _createRevealBorderEffect()
        {
            var sceneLightingEffect = new Windows.UI.Composition.Effects.SceneLightingEffect();
            sceneLightingEffect.AmbientAmount = 0f;
            sceneLightingEffect.DiffuseAmount = sc_diffuseAmountBorder;
            sceneLightingEffect.SpecularAmount = sc_specularAmountBorder;
            sceneLightingEffect.SpecularShine = sc_specularShineBorder;

            var colorMatrixEffect = new ColorMatrixEffect();
            colorMatrixEffect.ColorMatrix = sc_revealInvertedBorderColorMatrix;
            colorMatrixEffect.AlphaMode = Microsoft.Graphics.Canvas.CanvasAlphaMode.Straight;
            colorMatrixEffect.Source = sceneLightingEffect;

            var baseColorEffect = new ColorSourceEffect();
            baseColorEffect.Name = "BaseColorEffect";
            baseColorEffect.Color = Color.FromArgb(0, 0, 0, 0);

            var compositeEffect = new CompositeEffect();
            compositeEffect.Mode = Microsoft.Graphics.Canvas.CanvasComposite.SourceOver;
            //	Union of source and destination bitmaps. Equation: O = S + (1 - SA) * D.
            compositeEffect.Sources.Add(baseColorEffect); //Destination
            compositeEffect.Sources.Add(colorMatrixEffect);  //Source
            return compositeEffect;
        }

        private CompositionEffectFactory _createRevealBrushCompositionEffectFactory()
        {
            var props = new List<string>();
            props.Add("BaseColorEffect.Color");
            // Create the Comp effect Brush
            return Window.Current.Compositor.CreateEffectFactory(_createRevealBorderEffect(), props);
        }

        public void SetColor(Color color)
        {
            _revealBrush?.Properties.InsertColor("BaseColorEffect.Color", color);
        }

        private CompositionEffectBrush _revealBrush;

        public CompositionBrush CreateRevealBrush(Color color)
        {
            var effectFactory = _createRevealBrushCompositionEffectFactory();
            _revealBrush = effectFactory.CreateBrush();
            SetColor(color);
            return _revealBrush;
        }
    }
}
