﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApp.Helpers;

namespace WPFApp.Views.Elements
{
    /// <summary>
    /// Interaction logic for TilePanelNoScroller.xaml
    /// </summary>
    public partial class TilePanelNoScroller : UserControl, IAddChild, iItemsSource
    {
        public TilePanelNoScroller()
        {
            InitializeComponent();
        }

        public IEnumerable ItemsSource
        {
            get => itemsControl1.ItemsSource;
            set => itemsControl1.ItemsSource = value;
        }
    }
}
