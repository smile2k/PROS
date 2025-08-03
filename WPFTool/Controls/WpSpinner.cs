using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFTool.Controls
{
    public class WpSpinner : Control
    {
        static WpSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WpSpinner), new FrameworkPropertyMetadata(typeof(WpSpinner)));
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(WpSpinner), new PropertyMetadata(10.0));

        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(WpSpinner), new PropertyMetadata(90.0));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(WpSpinner), new PropertyMetadata(100.0));

        public double WidthSpin
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(double), typeof(WpSpinner), new PropertyMetadata(100.0));

        public double HeightSpin
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty =
             DependencyProperty.Register(nameof(Progress), typeof(double), typeof(WpSpinner), new PropertyMetadata(0.0));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty IsTextVisibleProperty =
             DependencyProperty.Register(nameof(IsTextVisible), typeof(bool), typeof(WpSpinner), new PropertyMetadata(true));

        public bool IsTextVisible
        {
            get => (bool)GetValue(IsTextVisibleProperty);
            set => SetValue(IsTextVisibleProperty, value);
        }

        public static readonly DependencyProperty IsSpinningProperty =
            DependencyProperty.Register(nameof(IsSpinning), typeof(bool), typeof(WpSpinner), new PropertyMetadata(true));

        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }


    }
}
