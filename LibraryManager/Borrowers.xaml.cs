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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlServerCe;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for Borrowers.xaml
    /// </summary>
    public partial class BorrowersWindow : LocalizedWindow
    {
        public BorrowersWindow()
        {
            InitializeComponent();
            SecureWindow();
        }

        private LibraryManager.LibraryDataDataSet libraryDataDataSet;
        private LibraryManager.LibraryDataDataSetTableAdapters.BorrowersTableAdapter libraryDataDataSetBorrowersTableAdapter;
        private System.Windows.Data.CollectionViewSource borrowersViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            libraryDataDataSet = ((LibraryManager.LibraryDataDataSet)(this.FindResource("libraryDataDataSet")));
            // Load data into the table Borrowers. You can modify this code as needed.
            libraryDataDataSetBorrowersTableAdapter = new LibraryManager.LibraryDataDataSetTableAdapters.BorrowersTableAdapter();
            libraryDataDataSetBorrowersTableAdapter.Fill(libraryDataDataSet.Borrowers);
            borrowersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowersViewSource")));
            borrowersViewSource.View.MoveCurrentToFirst();

            TranslateWindow();
        }

        public override void SecureWindow()
        {
            base.SecureWindow();
            if (AdminMode)
            {
                cmdDelete.IsEnabled = true;
                cmdSave.IsEnabled = true;
                cmdNew.IsEnabled = true;
                gridEditBorrower.IsEnabled = true;
            }
            else
            {
                cmdDelete.IsEnabled = false;
                cmdSave.IsEnabled = false;
                cmdNew.IsEnabled = false;
                gridEditBorrower.IsEnabled = false;
            }
        }

        #region Navigation

        private void doFirst(object sender, RoutedEventArgs e)
        {
            borrowersViewSource.View.MoveCurrentToFirst();
        }

        private void doPrev(object sender, RoutedEventArgs e)
        {
            borrowersViewSource.View.MoveCurrentToPrevious();
        }

        private void doNext(object sender, RoutedEventArgs e)
        {
            borrowersViewSource.View.MoveCurrentToNext();
        }

        private void doLast(object sender, RoutedEventArgs e)
        {
            borrowersViewSource.View.MoveCurrentToLast();
        }

        private void borrowersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (borrowersDataGrid.SelectedIndex != -1)
            {
                cmdDelete.Visibility = Visibility.Visible;
                cmdSave.Visibility = Visibility.Visible;
            }
            else
            {
                cmdSave.Visibility = Visibility.Collapsed;
                cmdDelete.Visibility = Visibility.Collapsed;
            }
        }

        #endregion 

        #region Administration

        private void doDelete(object sender, RoutedEventArgs e)
        {
            if (borrowersDataGrid.SelectedIndex != -1 && borrowersViewSource != null)
            {
                string promptText = LanguageHelper("ConfirmDelete", "Are you sure you want to delete?");
                string promptTitle = LanguageHelper("Confirm", "Confirm Action");
                MessageBoxResult prompt = System.Windows.MessageBox.Show(promptText, promptTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (prompt == MessageBoxResult.Yes)
                {
                    borrowersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowersViewSource")));
                    System.Data.DataRowView thisRowView = (System.Data.DataRowView)borrowersViewSource.View.CurrentItem;
                    int thisID = (int)thisRowView.Row["BorrowerId"];
                    libraryDataDataSetBorrowersTableAdapter.Delete(thisID, 1);

                    //Refresh
                    libraryDataDataSet.Books.AcceptChanges();
                    libraryDataDataSetBorrowersTableAdapter.Fill(libraryDataDataSet.Borrowers);
                    borrowersViewSource.View.MoveCurrentToFirst();
                }
            }
        }

        private void doCancel(object sender, RoutedEventArgs e)
        {
            libraryDataDataSet.Books.RejectChanges();

            libraryDataDataSetBorrowersTableAdapter.Fill(libraryDataDataSet.Borrowers);
            borrowersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowersViewSource")));
            borrowersViewSource.View.MoveCurrentToLast();
            
            spnlNavigateBorrowers.IsEnabled = true;
            borrowersDataGrid.IsEnabled = true;
            borrowerIDTextBox.Visibility = Visibility.Visible;
            cmdCancel.Visibility = Visibility.Collapsed;
            cmdSave.Visibility = Visibility.Collapsed;
            cmdGetNextNumber.Visibility = Visibility.Collapsed;
        }

        private void doSave(object sender, RoutedEventArgs e)
        {
            if (borrowerIDTextBox.Text != "-1" && borrowerIDTextBox.Text != "")
            {
                if (borrowersDataGrid.SelectedIndex != -1)
                {
                    libraryDataDataSetBorrowersTableAdapter.Update(libraryDataDataSet.Borrowers);
                    libraryDataDataSet.Books.AcceptChanges();

                    //Reset UI for Browse
                    borrowerIDTextBox.Visibility = Visibility.Visible;
                    borrowersDataGrid.IsEnabled = true;
                    spnlNavigateBorrowers.IsEnabled = true;
                    cmdCancel.Visibility = Visibility.Collapsed;
                    cmdDelete.Visibility = Visibility.Visible;
                    cmdGetNextNumber.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                bool borrowerSuspended = false;
                if ((bool)borrowerSuspendedCheckBox.IsChecked)
                    borrowerSuspended = true;
                if (borrowerNameTextBox.Text != "" && borrowerBarCodeTextBox.Text != "")
                {
                    if (!checkBarCodeExists(borrowerBarCodeTextBox.Text))
                    {
                        libraryDataDataSetBorrowersTableAdapter.Insert(borrowerNameTextBox.Text, borrowerAddressTextBox.Text, borrowerPhoneTextBox.Text, borrowerEmailTextBox.Text, borrowerSuspended, borrowerBarCodeTextBox.Text, txtNotes.Text);
                        libraryDataDataSet.Books.AcceptChanges();

                        libraryDataDataSetBorrowersTableAdapter.Fill(libraryDataDataSet.Borrowers);
                        borrowersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowersViewSource")));
                        borrowersViewSource.View.MoveCurrentToLast();

                        //Reset UI for Browse
                        borrowerIDTextBox.Visibility = Visibility.Visible;
                        borrowersDataGrid.IsEnabled = true;
                        spnlNavigateBorrowers.IsEnabled = true;
                        cmdCancel.Visibility = Visibility.Collapsed;
                        cmdDelete.Visibility = Visibility.Visible;
                        cmdGetNextNumber.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show(LanguageHelper("BarCodeExists", "Cannot create a record with this bar code because it already exists somewhere in this table. Ensure that you are creating a unique bar code."));
                    }
                }   
            }
        }

        private void doNew(object sender, RoutedEventArgs e)
        {
            libraryDataDataSet.Borrowers.AddBorrowersRow("", "", "", "", false, "", "");
            borrowersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowersViewSource")));
            borrowersViewSource.View.MoveCurrentToLast();
            borrowerIDTextBox.Visibility = Visibility.Collapsed;
            spnlNavigateBorrowers.IsEnabled = false;
            borrowersDataGrid.IsEnabled = false;
            cmdDelete.Visibility = Visibility.Collapsed;
            cmdCancel.Visibility = Visibility.Visible;
            cmdSave.Visibility = Visibility.Visible;
            cmdGetNextNumber.Visibility = Visibility.Visible;
            borrowerNameTextBox.Focus();
        }

        private void doGetNextNumber(object sender, RoutedEventArgs e)
        {
            double lastBarCode = -1;

            //Find the last (numeric) barcode the system knows about and add one to try to create a unique one
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "SELECT BorrowerBarCode FROM Borrowers Order By BorrowerBarCode";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            SqlCeDataReader rdr = newQuery.ExecuteReader();
            while (rdr.Read())
            {
                string thisBarCode = (string)rdr["BorrowerBarCode"];
                if (double.TryParse(thisBarCode, out lastBarCode))
                    break;
            }
            lastBarCode ++;

            //Make sure we somehow didn't create a duplicate!
            bool found = true;
            while (found)
            {
                found = false;
                if (checkBarCodeExists(lastBarCode.ToString()))
                {
                    found = true;
                    lastBarCode++;
                }
            }
            borrowerBarCodeTextBox.Text = lastBarCode.ToString();
        }

        private bool checkBarCodeExists(string barCodeToCheck)
        {
            //Find the last (numeric) barcode the system knows about and add one to try to create a unique one
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "SELECT BorrowerBarCode FROM Borrowers Where BorrowerBarCode='" + barCodeToCheck + "'";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            SqlCeDataReader rdr = newQuery.ExecuteReader();
            while (rdr.Read())
            {
                string thisBarCode = (string)rdr["BorrowerBarCode"];
                if (thisBarCode == barCodeToCheck)
                    return true;
            }
            return false;
        }

        #endregion

    }
}
