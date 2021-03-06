﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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

namespace UIWPFClean
{
    /// <summary>
    /// Interaction logic for Snackbar1.xaml
    /// </summary>
    public partial class Snackbar1 : UserControl
    {
        public Snackbar1()
        {
            InitializeComponent();
        }
        public bool IsActive
        {
            get => SnackbarFive.IsActive;
            set => SnackbarFive.IsActive = value;
        }

        public Snackbar Snackbar => SnackbarFive;
    }
}
