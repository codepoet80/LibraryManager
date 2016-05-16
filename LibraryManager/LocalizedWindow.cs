using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace LibraryManager
{
    public delegate string LanguageHelperDelegate(string promptTag, string defaultString);

    public class LocalizedWindow : Window
    {
        private LanguageHelperDelegate languageHelper;
        public LanguageHelperDelegate LanguageHelper
        {
            get
            {
                return languageHelper;
            }
            set
            {
                languageHelper = value;
                TranslateWindow();
            }
        }
        private int languageID = 0;
        public int LanguageID
        {
            get
            {
                return languageID;
            }
            set
            {
                if (value != languageID)
                {
                    languageID = value;
                    TranslateWindow();
                }
            }
        }

        private bool adminMode = false;
        public bool AdminMode
        {
            get
            {
                return adminMode;
            }
            set
            {
                adminMode = value;
                SecureWindow();
            }
        }

        public virtual void SecureWindow()
        {
            //do nothing;
        }

        public override void EndInit()
        {
            if (languageHelper == null)
                languageHelper = DefaultLanguageHelper;
            base.EndInit();
        }

        public virtual void TranslateWindow()
        {
            if (this.Tag != null && this.Tag.ToString() != "")
            {
                this.Title = LanguageHelper(this.Tag.ToString(), this.Tag.ToString());
            }
            ForEachControlRecursive(this, TranslateControl);
        }

        public virtual void TranslateControl(Control thisControl)
        {
            if (thisControl.GetType() == typeof(Button))
            {
                Button changeControl = (Button)thisControl;
                if (changeControl.Tag != null)
                    changeControl.Content = LanguageHelper(changeControl.Tag.ToString(), changeControl.Content.ToString());
            }
            if (thisControl.GetType() == typeof(ToggleButton))
            {
                ToggleButton changeControl = (ToggleButton)thisControl;
                if (changeControl.Tag != null)
                    changeControl.Content = LanguageHelper(changeControl.Tag.ToString(), changeControl.Content.ToString());
            }
            if (thisControl.GetType() == typeof(Label))
            {
                Label changeControl = (Label)thisControl;
                if (changeControl.Tag != null)
                {
                    if (changeControl.Tag != null)
                        changeControl.Content = LanguageHelper(changeControl.Tag.ToString(), changeControl.Content.ToString());
                }
            }
            if (thisControl.GetType() == typeof(TextBox))
            {
                TextBox changeControl = (TextBox)thisControl;
                if (changeControl.Tag != null)
                {
                    changeControl.Text = LanguageHelper(changeControl.Tag.ToString(), changeControl.Text.ToString());
                }
            }
            if (thisControl.GetType() == typeof(MenuItem))
            {
                MenuItem changeControl = (MenuItem)thisControl;
                if (changeControl.Tag != null)
                {
                    changeControl.Header = LanguageHelper(changeControl.Tag.ToString(), changeControl.Header.ToString());
                }
            }
            if (thisControl.GetType() == typeof(TabItem))
            {
                TabItem changeControl = (TabItem)thisControl;
                if (changeControl.Tag != null)
                {
                    changeControl.Header = LanguageHelper(changeControl.Tag.ToString(), changeControl.Header.ToString());
                }
            }
            if (thisControl.GetType() == typeof(DataGrid))
            {
                DataGrid changeControl = (DataGrid)thisControl;
                foreach (DataGridColumn thisColumn in changeControl.Columns)
                {                    
                    if (thisColumn.GetType() == typeof(DataGridTextColumn))
                    {
                        DataGridTextColumn thisTextColumn = (DataGridTextColumn)thisColumn;
                        thisTextColumn.Header = LanguageHelper(thisTextColumn.Header.ToString(), thisTextColumn.Header.ToString());

                        if (thisTextColumn.Binding != null && thisTextColumn.Binding.GetType() == typeof(Binding))
                        {
                            Binding thisBinding = (Binding)thisTextColumn.Binding;
                            if (thisBinding.Path != null)
                            {
                                PropertyPath thisPath = thisBinding.Path;
                                if (thisPath.Path != null && thisPath.Path != "")
                                {
                                    thisTextColumn.Header = LanguageHelper(thisPath.Path, thisPath.Path);
                                }
                            }
                        }
                    }
                    if (thisColumn.GetType() == typeof(DataGridCheckBoxColumn))
                    {
                        DataGridCheckBoxColumn thisTextColumn = (DataGridCheckBoxColumn)thisColumn;
                        thisTextColumn.Header = LanguageHelper(thisTextColumn.Header.ToString(), thisTextColumn.Header.ToString());

                        if (thisTextColumn.Binding != null && thisTextColumn.Binding.GetType() == typeof(Binding))
                        {
                            Binding thisBinding = (Binding)thisTextColumn.Binding;
                            if (thisBinding.Path != null)
                            {
                                PropertyPath thisPath = thisBinding.Path;
                                if (thisPath.Path != null && thisPath.Path != "")
                                {
                                    thisTextColumn.Header = LanguageHelper(thisPath.Path, thisPath.Path);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ForEachControlRecursive(object root, Action<Control> action)
        {
            Control control = root as Control;
            if (control != null)
            {
                action(control);
            }

            if (root is DependencyObject)
            {
                foreach (object child in LogicalTreeHelper.GetChildren((DependencyObject)root))
                {
                    ForEachControlRecursive(child, action);
                }
            }
        }
        
        public string DefaultLanguageHelper(string promptTag, string defaultString)
        {
            Console.WriteLine("ID: " + promptTag + " at: " + System.DateTime.Now.ToString() + " - No language helper was found, using default string");
            return defaultString;
        }

    }
}
