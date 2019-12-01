﻿using System;
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
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIWPFClean
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
            get => this.scroller.Content;
            set => this.scroller.Content = value;            
        }

        private void MainGrid_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
            var uniform = new UniformGrid();
            uniform.Columns = 2;
            var hz = new ScrollViewer();
            var textb = new TextBox();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var keybox = new TextBoxDrawer();
            stackPanel1.Children.Add(keybox);
        }

        void SetTrigger(ContentControl contentControl)
        {
            // create the command action and bind the command to it
            var invokeCommandAction = new InvokeCommandAction { CommandParameter = Dock.Bottom };
            var binding = new Binding { Path = new PropertyPath(@"x:Static materialDesign:DrawerHost.CloseDrawerCommand") };
            BindingOperations.SetBinding(invokeCommandAction, InvokeCommandAction.CommandProperty, binding);

            // create the event trigger and add the command action to it
            var eventTrigger = new System.Windows.Interactivity.EventTrigger { EventName = "GotFocus" };
            eventTrigger.Actions.Add(invokeCommandAction);

            // attach the trigger to the control
            var triggers = Interaction.GetTriggers(contentControl);
            triggers.Add(eventTrigger);
        }
    }
}
