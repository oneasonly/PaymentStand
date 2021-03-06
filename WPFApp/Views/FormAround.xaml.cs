﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for FormAround.xaml
    /// </summary>
    public partial class FormAround : UserControl
    {
        public FormAround()
        {
            InitializeComponent();
        }

        public object GetDataContext => this.DataContext;

        public object ContentControls
        {
            get => this.ContentContainer.Content;
            set => this.ContentContainer.Content = value;            
        }

        public Button BackButton => this.LeftPanelButtons.BackButton;
        public Button HomeButton => this.LeftPanelButtons.HomeButton;

        public ScrollViewer HeaderScroller => this.HeaderPath.Scroller;

        private void MainGrid_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void DialogHostTop_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            DialogContent.OnDialogOpened(sender, eventArgs);
        }
    }
}
