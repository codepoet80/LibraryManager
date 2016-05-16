using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Sql;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Windows.Shell;

namespace LibraryManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LocalizedWindow
    {
        LocalizedWindow checkoutWin, checkinWin, bookWin, borrowerWin, langWin;
        ReportsWindow reportsWin;
        
        public MainWindow()
        {
            InitializeComponent();
            LanguageHelper = new LanguageHelperDelegate(GetLanguageString);
            LanguageID = 1;
            ListLanguages();
            SecureWindow();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();
            foreach (ThumbButtonInfo thumbButton in this.TaskbarItemInfo.ThumbButtonInfos)
            {
                if (thumbButton.ImageSource.ToString().ToLower().IndexOf("bookin") != -1)
                    thumbButton.Description = lblCheckin.Content.ToString();
                if (thumbButton.ImageSource.ToString().ToLower().IndexOf("bookout") != -1)
                    thumbButton.Description = lblCheckout.Content.ToString();
            }
            if (App.Current.Properties.Contains("CurrentDatabasePath") && App.Current.Properties["CurrentDatabasePath"] != null && (string)App.Current.Properties["CurrentDatabasePath"] != "" && (string)App.Current.Properties["CurrentDatabasePath"] != "default")
            {
                this.Title = "[" + this.Title + "]";
                this.ToolTip = App.Current.Properties["CurrentDatabasePath"];
            }
        }

        /// <summary>
        /// Primary Language Helper for the whole app
        /// </summary>
        /// <param name="promptTag"></param>
        /// <param name="defaultString"></param>
        /// <returns></returns>
        public string GetLanguageString(string promptTag, string defaultString)
        {
            string promptValue = defaultString;
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "SELECT PromptString FROM Language WHERE PromptLanguageId = " + LanguageID + " AND PromptTag = '" + promptTag + "'";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            SqlCeDataReader rdr = newQuery.ExecuteReader();
            while (rdr.Read())
            {
                promptValue = (string)rdr["PromptString"];
            }

            if (LanguageID == 0)
                promptValue = "(" + promptTag + ")" + promptValue;

            return promptValue;
            
        }

        #region Window Management

        public override void SecureWindow()
        {
            base.SecureWindow();

            if (AdminMode)
            {
                cmdLanguage.IsEnabled = true;
            }
            else
            {
                cmdLanguage.IsEnabled = false;
            }
        }

        private void openCheckout(object sender, RoutedEventArgs e)
        {
            if (checkoutWin != null && checkoutWin.IsLoaded)
                checkoutWin.Focus();
            else
            {
                if (checkoutWin != null)
                    checkoutWin = null;

                checkoutWin = new CheckOutWindow();
                checkoutWin.LanguageHelper = this.LanguageHelper;
                checkoutWin.AdminMode = AdminMode;
                checkoutWin.Show();
                checkoutWin.Focus();
            }
        }

        private void openCheckin(object sender, RoutedEventArgs e)
        {
            if (checkinWin != null && checkinWin.IsLoaded)
                checkinWin.Focus();
            else
            {
                if (checkinWin != null)
                    checkinWin = null;

                checkinWin = new CheckInWindow();
                checkinWin.LanguageHelper = this.LanguageHelper;
                checkinWin.AdminMode = AdminMode;
                checkinWin.Show();
                checkinWin.Focus();
            }
        }

        private void openBooks(object sender, RoutedEventArgs e)
        {
            if (bookWin != null && bookWin.IsLoaded)
                bookWin.Focus();
            else
            {
                if (bookWin != null)
                    bookWin = null;

                bookWin = new BooksWindow();
                bookWin.LanguageHelper = this.LanguageHelper;
                bookWin.AdminMode = AdminMode;
                bookWin.Show();
                bookWin.Focus();
            }
        }

        private void openBorrowers(object sender, RoutedEventArgs e)
        {
            if (borrowerWin != null && borrowerWin.IsLoaded)
                borrowerWin.Focus();
            else
            {
                if (borrowerWin != null)
                    borrowerWin = null;

                borrowerWin = new BorrowersWindow();
                borrowerWin.LanguageHelper = this.LanguageHelper;
                borrowerWin.AdminMode = AdminMode;
                borrowerWin.Show();
                borrowerWin.Focus();
            }
        }

        private void openLanguages(object sender, RoutedEventArgs e)
        {
            if (langWin != null && langWin.IsLoaded)
                langWin.Focus();
            else
            {
                if (langWin != null)
                    langWin = null;

                langWin = new LanguagesWindow();
                langWin.LanguageHelper = this.LanguageHelper;
                langWin.AdminMode = AdminMode;
                langWin.Show();
                langWin.Focus();
            }
        }

        private void openReports(object sender, RoutedEventArgs e)
        {
            string currentReport = "";
            //get tag of sender
            if (sender.GetType() == typeof(MenuItem))
            {
                MenuItem thisItem = (MenuItem)sender;
                if (thisItem.Tag != null && thisItem.Tag.GetType() == typeof(string) && (string)thisItem.Tag != "")
                {
                    //assign it to currentReport of new Window
                    currentReport = (string)thisItem.Tag;
                }
            }

            if (reportsWin != null && reportsWin.IsLoaded)
            {
                reportsWin.Focus();
                reportsWin.CurrentReport = currentReport;
            }
            else
            {
                if (reportsWin != null)
                    reportsWin = null;

                reportsWin = new ReportsWindow();
                reportsWin.LanguageHelper = this.LanguageHelper;
                reportsWin.AdminMode = AdminMode;

                reportsWin.CurrentReport = currentReport;

                reportsWin.Show();
                reportsWin.Focus();
            }
        }

        private void closeWindows()
        {
            foreach (Window thisWindow in Application.Current.Windows)
            {
                if (thisWindow != this)
                {
                    thisWindow.Close();
                }
            }
        }

        private void updateWindows()
        {
            foreach (Window thisWindow in Application.Current.Windows)
            {
                if (thisWindow != this)
                {
                    try
                    {
                        LocalizedWindow localizedWindow = (LocalizedWindow)thisWindow;
                        localizedWindow.AdminMode = AdminMode;
                        localizedWindow.LanguageID = LanguageID;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unknown window type! " + ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Language List

        LibraryManager.LibraryDataDataSet libraryDataDataSet;
        LibraryManager.LibraryDataDataSetTableAdapters.LanguagesTableAdapter libraryDataDataSetLanguagesTableAdapter;
        System.Windows.Data.CollectionViewSource languagesViewSource;

        private void ListLanguages()
        {
            libraryDataDataSet = ((LibraryManager.LibraryDataDataSet)(this.FindResource("libraryDataDataSet")));
            // Load data into the table Languages. You can modify this code as needed.
            libraryDataDataSetLanguagesTableAdapter = new LibraryManager.LibraryDataDataSetTableAdapters.LanguagesTableAdapter();
            libraryDataDataSetLanguagesTableAdapter.Fill(libraryDataDataSet.Languages);
            languagesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("languagesViewSource")));
            languagesViewSource.View.MoveCurrentToPosition(LanguageID);
        }

        private void languagesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageID = cmbLanguage.SelectedIndex;
            updateWindows();
        }

        #endregion

        #region Settings

        private void getUserSettings()
        {
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "SELECT LanguageID FROM Settings WHERE UserId=1";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            SqlCeDataReader rdr = newQuery.ExecuteReader();
            while (rdr.Read())
            {
                int languageId;
                if (int.TryParse(rdr["LanguageID"].ToString(), out languageId))
                {
                    cmbLanguage.SelectedIndex = languageId;
                }
            }
        }

        private void saveUserSettings()
        {
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "UPDATE Settings SET LanguageID = " + LanguageID + " Where UserId=1";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            newQuery.ExecuteNonQuery();
        }

        #endregion

        #region Main Window Events and Taskbar

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closeWindows();
            saveUserSettings();
        }

        private void LocalizedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getUserSettings();

        }

        private void ThumbButtonInfo_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(ThumbButtonInfo))
            {
                ThumbButtonInfo thumbButton = (ThumbButtonInfo)sender;
                if (thumbButton.ImageSource.ToString().ToLower().IndexOf("bookin") != -1)
                    openCheckin(sender, null);
                if (thumbButton.ImageSource.ToString().ToLower().IndexOf("bookout") != -1)
                    openCheckout(sender, null);
            }
        }

        #endregion

        #region Admin mode

        private void AdminToggleButton_Click(object sender, RoutedEventArgs e)
        {
            toggleAdmin(sender, null);
        }

        private void toggleAdmin(object sender, MouseButtonEventArgs e)
        {
            if (AdminMode)
            {
                AdminToggleButton.IsChecked = false;
                AdminMode = false;
                this.TaskbarItemInfo.Overlay = null;
            }
            else
            {
                PasswordWindow passwordWin = new PasswordWindow();
                passwordWin.LanguageHelper = this.LanguageHelper;

                //If they right click, change password
                if (e != null && e.ChangedButton == MouseButton.Right)
                    passwordWin.ChangePassword = true;

                if ((bool)passwordWin.ShowDialog())
                {
                    AdminToggleButton.IsChecked = true;
                    BitmapImage img = new BitmapImage();
                    this.TaskbarItemInfo.Overlay = imgAdmin.Source;
                    AdminMode = true;
                }
                else
                {
                    AdminToggleButton.IsChecked = false;
                    this.TaskbarItemInfo.Overlay = null;
                    AdminMode = false;
                }
            }
            updateWindows();
        }

        #endregion

    }
}
