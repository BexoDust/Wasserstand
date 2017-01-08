using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfHelper.Sources
{
    // https://www.codeproject.com/articles/39244/scroll-synchronization
    /// <summary>
    /// A class which adds an additional properties to scrollviewers, which lets you add them to groups. 
    /// Every scrollviewer wihtin a group is set to the same scroll offset, making them scroll synchroniously
    /// </summary>
    public class ScrollHelper : DependencyObject
    {
        private const double ScrollTolerance = 0.01;

        public static readonly DependencyProperty HorizontalScrollGroupProperty =
            DependencyProperty.RegisterAttached(
                "HorizontalScrollGroup",
                typeof(string),
                typeof(ScrollHelper),
                new PropertyMetadata(OnHorizontalScrollGroupChanged));

        private static readonly Dictionary<ScrollViewer, string> HorizontalScrollViewers =
            new Dictionary<ScrollViewer, string>();


        private static readonly Dictionary<string, double> HorizontalScrollOffsets =
            new Dictionary<string, double>();


        public static void SetHorizontalScrollGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(HorizontalScrollGroupProperty, scrollGroup);
        }

        public static string GetHorizontalScrollGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(HorizontalScrollGroupProperty);
        }


        private static void OnHorizontalScrollGroupChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer != null)
            {
                if (!string.IsNullOrEmpty((string)e.OldValue))
                {
                    // Remove scrollviewer
                    if (HorizontalScrollViewers.ContainsKey(scrollViewer))
                    {
                        scrollViewer.ScrollChanged -= ScrollViewer_HorizontalScrollChanged;
                        HorizontalScrollViewers.Remove(scrollViewer);
                    }
                }

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    // If group already exists, set scrollposition of 
                    // new scrollviewer to the scrollposition of the group
                    if (HorizontalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToHorizontalOffset(
                            HorizontalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        HorizontalScrollOffsets.Add((string)e.NewValue,
                            scrollViewer.HorizontalOffset);
                    }

                    // Add scrollviewer
                    HorizontalScrollViewers.Add(scrollViewer, (string)e.NewValue);
                    scrollViewer.ScrollChanged += ScrollViewer_HorizontalScrollChanged;
                }
            }
        }

        private static void ScrollViewer_HorizontalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.HorizontalChange) > ScrollTolerance)
            {
                var changedScrollViewer = sender as ScrollViewer;
                ScrollHorizontal(changedScrollViewer);
            }
        }

        private static void ScrollHorizontal(ScrollViewer changedScrollViewer)
        {
            var group = HorizontalScrollViewers[changedScrollViewer];
            HorizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

            foreach (var scrollViewer in HorizontalScrollViewers.Where(s => s.Value == group && !Equals(s.Key, changedScrollViewer)))
            {
                if (Math.Abs(scrollViewer.Key.HorizontalOffset - changedScrollViewer.HorizontalOffset) > ScrollTolerance)
                {
                    scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
                }
            }
        }



        public static readonly DependencyProperty VerticalScrollGroupProperty =
            DependencyProperty.RegisterAttached(
                "VerticalScrollGroup",
                typeof(string),
                typeof(ScrollHelper),
                new PropertyMetadata(OnVerticalScrollGroupChanged));

        private static readonly Dictionary<ScrollViewer, string> VerticalScrollViewers =
            new Dictionary<ScrollViewer, string>();

        private static readonly Dictionary<string, double> VerticalScrollOffsets =
            new Dictionary<string, double>();

        public static void SetVerticalScrollGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(VerticalScrollGroupProperty, scrollGroup);
        }

        public static string GetVerticalScrollGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(VerticalScrollGroupProperty);
        }

        private static void OnVerticalScrollGroupChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer != null)
            {
                if (!string.IsNullOrEmpty((string)e.OldValue))
                {
                    // Remove scrollviewer
                    if (VerticalScrollViewers.ContainsKey(scrollViewer))
                    {
                        scrollViewer.ScrollChanged -= ScrollViewer_VerticalScrollChanged;
                        VerticalScrollViewers.Remove(scrollViewer);
                    }
                }

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    // If group already exists, set scrollposition of 
                    // new scrollviewer to the scrollposition of the group
                    if (VerticalScrollOffsets.Keys.Contains((string)e.NewValue))
                    {
                        scrollViewer.ScrollToVerticalOffset(VerticalScrollOffsets[(string)e.NewValue]);
                    }
                    else
                    {
                        VerticalScrollOffsets.Add((string)e.NewValue, scrollViewer.VerticalOffset);
                    }

                    // Add scrollviewer
                    VerticalScrollViewers.Add(scrollViewer, (string)e.NewValue);
                    scrollViewer.ScrollChanged += ScrollViewer_VerticalScrollChanged;
                }
            }
        }

        private static void ScrollViewer_VerticalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Math.Abs(e.VerticalChange) > ScrollTolerance)
            {
                var changedScrollViewer = sender as ScrollViewer;
                ScrollVertical(changedScrollViewer);
            }
        }


        private static void ScrollVertical(ScrollViewer changedScrollViewer)
        {
            var group = VerticalScrollViewers[changedScrollViewer];
            VerticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;

            foreach (var scrollViewer in VerticalScrollViewers.Where(s => s.Value == group && !Equals(s.Key, changedScrollViewer)))
            {
                if (Math.Abs(scrollViewer.Key.VerticalOffset - changedScrollViewer.VerticalOffset) > ScrollTolerance)
                {
                    scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
                }
            }
        }
    }
}