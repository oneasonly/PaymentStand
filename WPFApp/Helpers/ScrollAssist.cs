﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace WPFApp.Helpers
{
    public static class ScrollAssist
    {
        #region autoscroll
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ScrollAssist), new PropertyMetadata(false, AutoScrollPropertyChanged));

        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer != null && (bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;                
                scrollViewer.ScrollToRightEnd();
            }
            else
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentWidthChange != 0 || e.ViewportWidthChange!=0)
            {
                var scrollViewer = sender as ScrollViewer;
                scrollViewer?.ScrollToRightEnd();
            }
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
        #endregion
    }
}
