using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpCircleProgressBar : Control
    {

        static WpCircleProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpCircleProgressBar), new FrameworkPropertyMetadata(typeof(WpCircleProgressBar)));
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(WpCircleProgressBar), new PropertyMetadata(10.0));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(WpCircleProgressBar), new PropertyMetadata(90.0));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            private set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(WpCircleProgressBar), new PropertyMetadata(100.0));

        public double WidthSpin
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(double), typeof(WpCircleProgressBar), new PropertyMetadata(100.0));

        public double HeightSpin
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty =
             DependencyProperty.Register(nameof(Progress), typeof(double), typeof(WpCircleProgressBar), new PropertyMetadata(0.0, OnProgressChanged));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty IsTextVisibleProperty =
             DependencyProperty.Register(nameof(IsTextVisible), typeof(bool), typeof(WpCircleProgressBar), new PropertyMetadata(true));

        public bool IsTextVisible
        {
            get => (bool)GetValue(IsTextVisibleProperty);
            set => SetValue(IsTextVisibleProperty, value);
        }

        public static readonly DependencyProperty IsSpinningProperty =
            DependencyProperty.Register(nameof(IsSpinning), typeof(bool), typeof(WpCircleProgressBar), new PropertyMetadata(true));

        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (WpCircleProgressBar)d;
            double progress = (double)e.NewValue;

            // Giới hạn progress từ 0 đến 100, rồi tính góc
            double clampedProgress = Math.Min(Math.Max(progress, 0), 100);
            control.Progress = clampedProgress;
            double newAngle = clampedProgress * 3.6;

            control.Angle = newAngle;
        }
    }
}
