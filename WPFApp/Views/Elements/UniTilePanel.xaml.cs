﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFApp.Views.Elements
{
    /// <summary>
    /// Interaction logic for UniTilePanel.xaml
    /// </summary>
    public partial class UniTilePanel : UserControl
    {
        public UniTilePanel()
        {
            InitializeComponent();
        }
        public IEnumerable ItemsSource
        {
            get => itemsControl1.ItemsSource;
            set => itemsControl1.ItemsSource = value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine($"scroller.VertBarVisibility={scroller.ComputedVerticalScrollBarVisibility}");
            Trace.WriteLine($"scroller.2VertBarVisibility={scroller.VerticalScrollBarVisibility}");
            Trace.WriteLine($"scroller.3VertBarVisibility={scroller.Visibility}");
        }
    }
}
