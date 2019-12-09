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
            if (e.ExtentHeightChange != 0)
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

        #region Friction

        /// <summary>
        /// Friction Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrictionProperty =
            DependencyProperty.RegisterAttached("Friction",
            typeof(double), typeof(ScrollAssist),
                new FrameworkPropertyMetadata((double)0.95));

        /// <summary>
        /// Gets the Friction property.  This dependency property
        /// indicates ....
        /// </summary>
        public static double GetFriction(DependencyObject d)
        {
            return (double)d.GetValue(FrictionProperty);
        }

        /// <summary>
        /// Sets the Friction property.
        /// </summary>
        public static void SetFriction(DependencyObject d, double value)
        {
            d.SetValue(FrictionProperty, value);
        }

        #endregion

        #region ScrollStartPoint

        /// <summary>
        /// ScrollStartPoint Attached Dependency Property
        /// </summary>
        private static readonly DependencyProperty ScrollStartPointProperty =
            DependencyProperty.RegisterAttached("ScrollStartPoint",
            typeof(Point), typeof(ScrollAssist),
                new FrameworkPropertyMetadata((Point)new Point()));

        /// <summary>
        /// Gets the ScrollStartPoint property.
        /// </summary>
        private static Point GetScrollStartPoint(DependencyObject d)
        {
            return (Point)d.GetValue(ScrollStartPointProperty);
        }

        /// <summary>
        /// Sets the ScrollStartPoint property.
        /// </summary>
        private static void SetScrollStartPoint(DependencyObject d,
            Point value)
        {
            d.SetValue(ScrollStartPointProperty, value);
        }

        #endregion

        #region ScrollStartOffset

        /// <summary>
        /// ScrollStartOffset Attached Dependency Property
        /// </summary>
        private static readonly DependencyProperty ScrollStartOffsetProperty =
            DependencyProperty.RegisterAttached("ScrollStartOffset",
            typeof(Point), typeof(ScrollAssist),
                new FrameworkPropertyMetadata((Point)new Point()));

        /// <summary>
        /// Gets the ScrollStartOffset property.
        /// </summary>
        private static Point GetScrollStartOffset(DependencyObject d)
        {
            return (Point)d.GetValue(ScrollStartOffsetProperty);
        }

        /// <summary>
        /// Sets the ScrollStartOffset property.
        /// </summary>
        private static void SetScrollStartOffset(DependencyObject d,
            Point value)
        {
            d.SetValue(ScrollStartOffsetProperty, value);
        }

        #endregion

        #region InertiaProcessor

        /// <summary>
        /// InertiaProcessor Attached Dependency Property
        /// </summary>
        private static readonly DependencyProperty InertiaProcessorProperty =
            DependencyProperty.RegisterAttached("InertiaProcessor",
            typeof(InertiaHandler), typeof(ScrollAssist),
                new FrameworkPropertyMetadata((InertiaHandler)null));

        /// <summary>
        /// Gets the InertiaProcessor property.
        /// </summary>
        private static InertiaHandler GetInertiaProcessor(DependencyObject d)
        {
            return (InertiaHandler)d.GetValue(InertiaProcessorProperty);
        }

        /// <summary>
        /// Sets the InertiaProcessor property.
        /// </summary>
        private static void SetInertiaProcessor(DependencyObject d,
            InertiaHandler value)
        {
            d.SetValue(InertiaProcessorProperty, value);
        }

        #endregion

        #region HandleKineticScrolling

        /// <summary>
        /// HandleKineticScrolling Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty
            HandleKineticScrollingProperty =
            DependencyProperty.RegisterAttached("HandleKineticScrolling",
            typeof(bool), typeof(ScrollAssist),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(
                        OnHandleKineticScrollingChanged)));

        /// <summary>
        /// Gets the HandleKineticScrolling property.
        /// </summary>
        public static bool GetHandleKineticScrolling(DependencyObject d)
        {
            return (bool)d.GetValue(HandleKineticScrollingProperty);
        }

        /// <summary>
        /// Sets the HandleKineticScrolling property.
        /// </summary>
        public static void SetHandleKineticScrolling(DependencyObject d,
            bool value)
        {
            d.SetValue(HandleKineticScrollingProperty, value);
        }

        /// <summary>
        /// Handles changes to the HandleKineticScrolling property.
        /// </summary>
        private static void OnHandleKineticScrollingChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scoller = d as ScrollViewer;
            if ((bool)e.NewValue)
            {                
                scoller.PreviewMouseDown += OnPreviewMouseDown;
                scoller.PreviewMouseMove += OnPreviewMouseMove;
                scoller.PreviewMouseUp += OnPreviewMouseUp;
                SetInertiaProcessor(scoller, new InertiaHandler(scoller));
            }
            else
            {
                scoller.PreviewMouseDown -= OnPreviewMouseDown;
                scoller.PreviewMouseMove -= OnPreviewMouseMove;
                scoller.PreviewMouseUp -= OnPreviewMouseUp;
                var inertia = GetInertiaProcessor(scoller);
                if (inertia != null)
                    inertia.Dispose();
            }

        }

        #endregion

        #region Mouse Events
        private static void OnPreviewMouseDown(object sender,
            MouseButtonEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.IsMouseOver)
            {
                // Save starting point, used later when
                //determining how much to scroll.
                SetScrollStartPoint(scrollViewer,
                    e.GetPosition(scrollViewer));
                SetScrollStartOffset(scrollViewer,
                    new Point(scrollViewer.HorizontalOffset,
                        scrollViewer.VerticalOffset));
                scrollViewer.CaptureMouse();
            }
        }


        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(scrollViewer);

                var scrollStartPoint = GetScrollStartPoint(scrollViewer);
                // Determine the new amount to scroll.
                Point delta = new Point(scrollStartPoint.X -
                    currentPoint.X, scrollStartPoint.Y - currentPoint.Y);

                var scrollStartOffset = GetScrollStartOffset(scrollViewer);
                Point scrollTarget = new Point(scrollStartOffset.X +
                    delta.X, scrollStartOffset.Y + delta.Y);

                var inertiaProcessor = GetInertiaProcessor(scrollViewer);
                if (inertiaProcessor != null)
                    inertiaProcessor.ScrollTarget = scrollTarget;

                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(scrollTarget.X);
                scrollViewer.ScrollToVerticalOffset(scrollTarget.Y);
            }
        }

        private static void OnPreviewMouseUp(object sender,
            MouseButtonEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (scrollViewer.IsMouseCaptured)
            {
                scrollViewer.ReleaseMouseCapture();
            }
        }
        #endregion

        #region Inertia Stuff

        /// <summary>
        /// Handles the inertia
        /// </summary>
        class InertiaHandler : IDisposable
        {
            private Point previousPoint;
            private Vector velocity;
            ScrollViewer scroller;
            DispatcherTimer animationTimer;

            private Point scrollTarget;
            public Point ScrollTarget
            {
                get { return scrollTarget; }
                set { scrollTarget = value; }
            }

            public InertiaHandler(ScrollViewer scroller)
            {
                this.scroller = scroller;
                animationTimer = new DispatcherTimer();
                animationTimer.Interval =
                    new TimeSpan(0, 0, 0, 0, 20);
                animationTimer.Tick +=
                    new EventHandler(HandleWorldTimerTick);
                animationTimer.Start();
            }

            private void HandleWorldTimerTick(object sender,
                EventArgs e)
            {
                if (scroller.IsMouseCaptured)
                {
                    Point currentPoint = Mouse.GetPosition(scroller);
                    velocity = previousPoint - currentPoint;
                    previousPoint = currentPoint;
                }
                else
                {
                    if (velocity.Length > 1)
                    {
                        scroller.ScrollToHorizontalOffset(
                            ScrollTarget.X);
                        scroller.ScrollToVerticalOffset(
                            ScrollTarget.Y);
                        scrollTarget.X += velocity.X;
                        scrollTarget.Y += velocity.Y;
                        velocity *=
                            ScrollAssist.GetFriction(scroller);
                    }
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                animationTimer.Stop();
            }

            #endregion
        }

        #endregion
    }
}
