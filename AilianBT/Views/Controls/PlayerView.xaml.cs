﻿using AilianBT.ViewModels;
using System;
using System.Threading;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace AilianBT.Views.Controls
{
    public sealed partial class PlayerView : UserControl
    {
        private PlayerViewModel _playerVM;
        private SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

        public PlayerView()
        {
            this.InitializeComponent();
            this.DataContextChanged += (o, e) => _playerVM = this.DataContext as PlayerViewModel;
            _initPlayerTitleAnimationResources();
            _initPlaybackProgress();
        }

        private bool _isAnimating = false;
        private bool _isScrollable = false;
        private Visual _scrollableVisual = null;
        private Timer _panelSizeChangedTimer = null;

        private void _initPlayerTitleAnimationResources()
        {
            _scrollableVisual = ElementCompositionPreview.GetElementVisual(panelScrollPlayerTitle);
        }

        private void _createAnimation()
        {
            var easingFunction = Window.Current.Compositor.CreateLinearEasingFunction();
            var offsetAnimation = Window.Current.Compositor.CreateVector3KeyFrameAnimation();
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(tbPlayerTitle.Text.Length * 300);
            offsetAnimation.StopBehavior = AnimationStopBehavior.SetToInitialValue;
            var width = tbPlayerTitle.ActualWidth + tbPlayerTitle.Margin.Left + tbPlayerTitle.Margin.Right;
            offsetAnimation.InsertKeyFrame(1f, new System.Numerics.Vector3((float)-width, 0, 0), easingFunction);
            _scrollableVisual.StartAnimation("Offset", offsetAnimation);
        }

        private void _panelScrollPlayerTitlePointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _tryAnimate();
        }

        private void _tryAnimate()
        {
            if (!_isScrollable)
                return;
            if (_isAnimating)
                return;
            var scopedBatch = Window.Current.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _isAnimating = true;
            _createAnimation();
            scopedBatch.End();
            scopedBatch.Completed += (o, args) =>
            {
                _isAnimating = false;
                scopedBatch.Dispose();
            };
        }

        private void _initScrollableRegion()
        {
            if (_isAnimating)
            {
                _scrollableVisual.StopAnimation("Offset");
                _isAnimating = false;
            }

            if (tbPlayerTitle.ActualWidth >= gridPlayerTitle.ActualWidth)
            {
                tbPlayerTitleSecond.Visibility = Visibility.Visible;
                tbPlayerTitle.Margin = new Thickness(0, 0, 16, 0);
                _isScrollable = true;
            }
            else
            {
                tbPlayerTitleSecond.Visibility = Visibility.Collapsed;
                _isScrollable = false;
            }
        }

        private void _tbPlayerTitleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _initScrollableRegion();
        }

        private void _gridPlayerTitleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _initScrollableRegion();

            if (_panelSizeChangedTimer == null)
            {
                _panelSizeChangedTimer = new Timer((o) =>
                {
                    SynchronizationContext.Post((state) =>
                    {
                        _tryAnimate();
                    }, null);
                    _panelSizeChangedTimer.Dispose();
                    _panelSizeChangedTimer = null;
                }, null, TimeSpan.FromMilliseconds(400), TimeSpan.Zero);
            }
            _panelSizeChangedTimer.Change(TimeSpan.FromMilliseconds(400), TimeSpan.Zero);
        }

        #region Interact with Player Progress
        private void _initPlaybackProgress()
        {
            sliderProgress.AddHandler(PointerPressedEvent, new PointerEventHandler(_sliderProgressPointerPressed), true);
            sliderProgress.AddHandler(PointerReleasedEvent, new PointerEventHandler(_sliderProgressPointerReleased), true);
        }

        private void _sliderProgressPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _playerVM.Seeking = true;
        }

        private void _sliderProgressPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var length = (sliderProgress.Maximum - sliderProgress.Minimum);
            if (length > 0)
            {
                _playerVM.Seek(sliderProgress.Value / length);
            }
            _playerVM.Seeking = false;
        }
        #endregion
    }
}
