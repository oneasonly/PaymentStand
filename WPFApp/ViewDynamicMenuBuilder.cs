﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WPFApp.Interfaces;
using WPFApp.ViewModels;
using XmlStructureComplat;

namespace WPFApp
{
    public class ViewDynamicMenuBuilder
    {
        #region fields
        private DynamicMenuWindowViewModel model;
        private Window window;
        private FormAround form = new FormAround();
        private ViewPagesManager views = new ViewPagesManager();
        private iControls _Controls;
        private int fakeCount;
        #endregion
        #region ctor
        public ViewDynamicMenuBuilder(Window incWindow/*, iControls Controls*/)
        {
            window = incWindow;
            //_Controls = Controls;
            try
            {
                model = window.DataContext as DynamicMenuWindowViewModel;
                model.NewResponseComeEvent += BuildMenuOnReponse;
                model.PropertyChanged += IsLoadingMenuChanged;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        private void IsLoadingMenuChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(model.IsLoadingMenu))
            {
                window.Content = "Loading...";
            }
            if (e.PropertyName == nameof(model.Exception))
            {
                try
                {
                    throw new Exception("",model.Exception);
                }
                catch (Exception ex)
                {
                    DisplayErrorPage(ex?.InnerException ?? ex);
                }
            }
        }

        #region Properties
        public bool IsPageAvaiable => views.IsNextAvaible;
        #endregion
        private void ClearOnResponse()
        {
            views = new ViewPagesManager();
            fakeCount = 0;
        }
        private void BuildMenuOnReponse()
        {
            try
            {
                ClearOnResponse();
                BuildsPages();
            }
            catch (Exception ex)
            {
                DisplayErrorPage(ex);
            }
        }

        private void DisplayErrorPage(Exception ex)
        {
            DisplayErrorPage($"{ex.Message}\n\n{ex.StackTrace}");
        }
        private void DisplayErrorPage(string msgError)
        {
            var str = "Извините, произошла непредвиденная ошибка. " +
                $"Обратитесь к администратору.\n\n{msgError}";
            window.Content = CentralLabelBorder(str);
        }

        private void BuildsPages()
        {
            this.ResponseAnalizeAndBuild();
            this.fakeCount = views.Count;
            if (views.Count == 1)
            {
                //window.Content = pages.Page;
                this.NextPage();
            }
            if (views.Count > 1)
            {
                this.views = new ViewPagesManager();
                model.ClearChildLookupVM();
                Trace.WriteLine($"{nameof(BuildsPages)}(): if(pages.Count > 1) Clear & reBuild");
                this.ResponseAnalizeAndBuild();
                this.NextPage();
            }
            if (views.Count <= 0)
            {
                var str = "Извините, мы не смогли построить никакой " +
                    "страницы на ответ сервера.\n(pages.Count <= 0)";
                window.Content = CentralLabelBorder(str);                
            }
        }
        private void ResponseAnalizeAndBuild()
        {
            if (model == null) { throw new NullReferenceException("main VM = null"); };
            var rootResponse = model.Responce;
            var resp = rootResponse.GetListResponse;
            var paylist = resp.PayRecord;
            if(resp != null && resp?.ErrorCode != 0)
            {
                string str = $"{resp.ErrorCode}\n{resp.ErrorText}";
                var control = CentralLabelBorder(str);
                views.AddControl(control);
            }
            if (paylist.Count > 1)
            {
                views.NewPage();
                foreach (var payrec in paylist)
                {
                    var button = ButtonSelect(payrec);
                    CheckButtonCommand(button, paylist.Last() == payrec);
                    views.AddControl(button);
                }
            }
            if (paylist.Count == 1)
            {
                var attrRecords = paylist.First().AttrRecord;
                //LOOKUPs display every lookup as 1 page
                foreach (var attr in attrRecords)
                {
                    if (attr.Edit == 1 && attr.View == 1 && !string.IsNullOrEmpty(attr.Lookup))
                    {
                        Trace.WriteLine($"{nameof(ResponseAnalizeAndBuild)}(): LOOKUP display add: attr={attr.Name} Lookup={attr.Lookup}");
                        List<Lookup> lookups = paylist?.First()?.Lookups;
                        Lookup selectedLookup = lookups?.Where(x => x.Name.ToLower() == attr.Lookup.ToLower() )?.Single();
                        int index = model.PayrecToSend.AttrRecord.FindIndex(x => x == attr);
                        views.NewPage();                        
                        LookupVM childVM = model.GetNewLookupVM();
                        childVM.Lookup = selectedLookup;
                        views.AddDataContext(childVM);                        
                        views.AddControl(new Label() { Content = selectedLookup.Name });
                        var lookItems = selectedLookup.Item;
                        lookItems.ForEach(arg =>
                        {
                            //var binding = new Binding($"{nameof(model.PayrecToSend)}.AttrRecord[{index}].Value");
                            //binding.Mode = BindingMode.OneWay;
                            var btn = new Button();
                            btn.Content = arg.Value;
                            btn.Command = childVM?.SelectLookupCommand;
                            btn.CommandParameter = arg;
                            btn.Click += (sender, evArg) => NextPage();
                            views.AddControl(btn);
                        });
                        //LookupButtons(selectedLookup);
                        views.NewPage();
                    }
                }
                Trace.WriteLine($"{nameof(ResponseAnalizeAndBuild)}(): After LOOKup ViewPages={views.Count};");
                views.NewPage();
                //ATTRs first display filled data attrs
                foreach (var attr in attrRecords)
                {
                    if (attr.Edit != 1 && attr.View == 1)
                    {
                        Trace.WriteLine($"{nameof(ResponseAnalizeAndBuild)}(): ATTR filled info display: attr={attr.Name} value={attr.Value}");
                        var label = CentralLabelBorder();
                        label.Content = $"{attr.Name} = {attr.Value}";
                        views.AddControl(label);
                    }
                }
                //ATTRs at last display attrs need to enter with textbox
                foreach (var attr in attrRecords)
                {
                    if (attr.Edit == 1 && attr.View == 1 && string.IsNullOrEmpty(attr.Lookup))
                    {
                        Trace.WriteLine($"{nameof(ResponseAnalizeAndBuild)}(): ATTR input: attr={attr.Name}");
                        var label = new Label();
                        label.Content = $"{attr.Name}:";
                        var inputbox = new TextBox();
                        int index = model.PayrecToSend.AttrRecord.FindIndex(x => x==attr);
                        var binding = new Binding($"{nameof(model.PayrecToSend)}.AttrRecord[{index}].Value");
                        inputbox.SetBinding(TextBox.TextProperty, binding);

                        views.AddControl(label);
                        views.AddControl(inputbox);
                    }
                }
                views.AddControl(new TextBlock());
                var button = new Button() 
                { 
                    Content="Продолжить",
                    Command = model.SendVmPayrecCommand
                };
                views.AddControl(button);
            }
        }

        private void CheckButtonCommand(Button button, bool isLastPage)
        {
            if (isLastPage) return;
            if (fakeCount > 1)
            {
                button.Command = null;
                button.CommandParameter = null;
                button.Click += (s, arg) => NextPage();
            }
        }
        private void RecurseRemoveButtonCommands(UIElement arg)
        {
            if(arg is Button)
            {
                Button button = arg as Button;                
                button.Command = null;
                if (button.Command is Prism.Commands.DelegateCommand<object>)
                    button.CommandParameter = null;
            }
            if (arg is Panel)
            {
                Panel pnl = arg as Panel;
                foreach (UIElement item in pnl.Children)
                {
                    RecurseRemoveButtonCommands(item);
                }
            }
        }
        private void ChangeCommandsFromVMToViewClick()
        {
            for (int i = 0; i < views.Count - 1; i++)
            {
                var page = views[i];
                foreach (UIElement item in page.Children)
                {
                    RecurseRemoveButtonCommands(item);
                }
            }
        }
        private void NextPage(object param=null)
        {
            Trace.WriteLine($"DynamicMenuBuilder.Next VIEW Page: {views.IsNextAvaible}; Current={views.CurrIndex} Count={views.Count}");
            if(views.IsNextAvaible)
            {
                window.Content = views.NextPage();
            }
            else
            {
                model.SendParamCommand.Execute(param);
            }
        }
        private static Label CentralLabelBorder(string arg = "")
        {
            var info = new Label();
            info.HorizontalContentAlignment = HorizontalAlignment.Center;
            info.VerticalContentAlignment = VerticalAlignment.Center;
            info.BorderThickness = new Thickness(2);
            info.BorderBrush = System.Windows.Media.Brushes.Black;
            info.Margin = info.BorderThickness;
            var text = new TextBlock();
            text.Text = arg;
            text.TextWrapping = TextWrapping.Wrap;
            info.Content = text;
            return info;
        }
        private void LookupButtons(XmlStructureComplat.Lookup selectedLookup)
        {
            views.AddControl(new Label(){ Content = selectedLookup.Name});
            var lookItems = selectedLookup.Item;
            lookItems.ForEach(x =>
            {
                views.AddControl(new Button()
                {
                    Content = $"{x.Value}",
                });
            });
        }
        private Button ButtonSelect(XmlStructureComplat.PayRecord payrec)
        {
            var button = new Button();
            button.Content = payrec.Name;
            button.Command = model.SendParamCommand;
            button.CommandParameter = payrec;
            return button;
        }
    }
}