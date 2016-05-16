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
    /// Interaction logic for Books.xaml
    /// </summary>
    public partial class BooksWindow : LocalizedWindow
    {

        public BooksWindow()
        {
            InitializeComponent();
            SecureWindow();
            
        }

        private LibraryManager.LibraryDataDataSet libraryDataDataSet;
        private LibraryManager.LibraryDataDataSetTableAdapters.BooksTableAdapter libraryDataDataSetBooksTableAdapter;
        private System.Windows.Data.CollectionViewSource booksViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            libraryDataDataSet = ((LibraryManager.LibraryDataDataSet)(this.FindResource("libraryDataDataSet")));
            // Load data into the table Books. You can modify this code as needed.
            libraryDataDataSetBooksTableAdapter = new LibraryManager.LibraryDataDataSetTableAdapters.BooksTableAdapter();
            libraryDataDataSetBooksTableAdapter.Fill(libraryDataDataSet.Books);
            booksViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksViewSource")));
            booksViewSource.View.MoveCurrentToFirst();

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
                gridEditBook.IsEnabled = true;
                txtNotes.IsEnabled = true;
                cmdDuplicate.IsEnabled = true;
            }
            else
            {
                cmdDelete.IsEnabled = false;
                cmdSave.IsEnabled = false;
                cmdNew.IsEnabled = false;
                gridEditBook.IsEnabled = false;
                txtNotes.IsEnabled = false;
                cmdDuplicate.IsEnabled = false;
            }
        }

        #region Navigation

        private void doFirst(object sender, RoutedEventArgs e)
        {
            booksViewSource.View.MoveCurrentToFirst();
        }

        private void doPrev(object sender, RoutedEventArgs e)
        {
            booksViewSource.View.MoveCurrentToPrevious();
        }

        private void doNext(object sender, RoutedEventArgs e)
        {
            booksViewSource.View.MoveCurrentToNext();
        }

        private void doLast(object sender, RoutedEventArgs e)
        {
            booksViewSource.View.MoveCurrentToLast();
        }

        private void booksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (booksDataGrid.SelectedIndex != -1)
            {
                cmdDelete.Visibility = Visibility.Visible;
                cmdSave.Visibility = Visibility.Visible;
                cmdDuplicate.Visibility = Visibility.Visible;
            }
            else
            {
                cmdSave.Visibility = Visibility.Collapsed;
                cmdDelete.Visibility = Visibility.Collapsed;
                cmdDuplicate.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region Administration

        private void doDelete(object sender, RoutedEventArgs e)
        {
            if (booksDataGrid.SelectedIndex != -1)
            {
                string promptText = LanguageHelper("ConfirmDelete", "Are you sure you want to delete?");
                string promptTitle = LanguageHelper("Confirm", "Confirm Action");
                MessageBoxResult prompt = System.Windows.MessageBox.Show(promptText, promptTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (prompt == MessageBoxResult.Yes)
                {
                    booksViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksViewSource")));
                    System.Data.DataRowView thisRowView = (System.Data.DataRowView)booksViewSource.View.CurrentItem;
                    int thisID = (int)thisRowView.Row["BookId"];
                    libraryDataDataSetBooksTableAdapter.Delete(thisID, 1);

                    //Refresh
                    libraryDataDataSet.Books.AcceptChanges();
                    libraryDataDataSetBooksTableAdapter.Fill(libraryDataDataSet.Books);
                    booksViewSource.View.MoveCurrentToFirst();
                }
            }
        }

        private void doCancel(object sender, RoutedEventArgs e)
        {
            libraryDataDataSet.Books.RejectChanges();
            libraryDataDataSetBooksTableAdapter.Fill(libraryDataDataSet.Books);
            booksViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksViewSource")));
            booksViewSource.View.MoveCurrentToLast();

            spnlNavigateBooks.IsEnabled = true;
            booksDataGrid.IsEnabled = true;
            bookIDTextBox.Visibility = Visibility.Visible;
            cmdCancel.Visibility = Visibility.Collapsed;
            cmdSave.Visibility = Visibility.Collapsed;
            cmdGetNextNumber.Visibility = Visibility.Collapsed;
        }

        private void doSave(object sender, RoutedEventArgs e)
        {
            //Update book
            if (bookIDTextBox.Text != "-1" && bookIDTextBox.Text != "")
            {
                if (booksDataGrid.SelectedIndex != -1)
                {
                    libraryDataDataSetBooksTableAdapter.Update(libraryDataDataSet.Books);
                    libraryDataDataSet.Books.AcceptChanges();

                    //Reset UI for browse
                    spnlNavigateBooks.IsEnabled = true;
                    booksDataGrid.IsEnabled = true;
                    cmdCancel.Visibility = Visibility.Collapsed;
                    cmdDelete.Visibility = Visibility.Visible;
                    cmdDuplicate.Visibility = Visibility.Visible;
                    cmdGetNextNumber.Visibility = Visibility.Collapsed;
                    bookIDTextBox.Visibility = Visibility.Visible;
                }
            }
            //New Book
            else
            {
                int bookYear = -1;
                if (bookNameTextBox.Text != "" && bookAuthorTextBox.Text != "" && int.TryParse(bookYearTextBox.Text, out bookYear))
                {
                    if (!checkBarCodeExists(bookBarCodeTextBox.Text))
                    {
                        libraryDataDataSetBooksTableAdapter.Insert(bookNameTextBox.Text, bookAuthorTextBox.Text, bookYear, bookBarCodeTextBox.Text, bookCategoryTextBox.Text, bookTypeTextBox.Text, txtNotes.Text);
                        libraryDataDataSet.Books.AcceptChanges();
                        libraryDataDataSetBooksTableAdapter.Fill(libraryDataDataSet.Books);
                        booksViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksViewSource")));
                        booksViewSource.View.MoveCurrentToLast();
                        
                        //Reset UI for browse
                        spnlNavigateBooks.IsEnabled = true;
                        booksDataGrid.IsEnabled = true;
                        cmdCancel.Visibility = Visibility.Collapsed;
                        cmdDelete.Visibility = Visibility.Visible;
                        cmdDuplicate.Visibility = Visibility.Visible;
                        cmdGetNextNumber.Visibility = Visibility.Collapsed;
                        bookIDTextBox.Visibility = Visibility.Visible;

                        //Select and scroll to the last inserted row
                        //TODO: Still doesn't work for non-numerical sort orders
                        DataRow[] foundRows;
                        foundRows = libraryDataDataSet.Tables["Books"].Select("BookBarCode Like '" + bookBarCodeTextBox.Text + "'");
                        if (foundRows != null && foundRows[0] != null)
                        {
                            //Find a rowview whose row matches the last inserted row (TODO: Is there a better way to do this?)
                            foreach (DataRowView rowView in booksViewSource.View)
                            {
                                DataRow row = rowView.Row;
                                if (rowView.Row == foundRows[0])
                                {
                                    booksViewSource.View.MoveCurrentTo(rowView);
                                    booksDataGrid.SelectedItem = booksViewSource.View.CurrentItem;
                                }
                            }
                        }

                        //Scroll to the seleted item
                        if (booksDataGrid.SelectedItem != null)
                            booksDataGrid.ScrollIntoView(booksDataGrid.SelectedItem);

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
            booksViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksViewSource")));
            DataRow newRow = libraryDataDataSet.Books.AddBooksRow("", "", 0, "", "", "", "");
            booksViewSource.View.MoveCurrentTo(newRow); //May 2016: Move to the newly created row, instead of the bottom of the grid.
            bookIDTextBox.Visibility = Visibility.Collapsed;
            spnlNavigateBooks.IsEnabled = false;
            booksDataGrid.IsEnabled = false;
            cmdDelete.Visibility = Visibility.Collapsed;
            cmdDuplicate.Visibility = Visibility.Collapsed;
            cmdCancel.Visibility = Visibility.Visible;
            cmdSave.Visibility = Visibility.Visible;
            cmdGetNextNumber.Visibility = Visibility.Visible;
            tabDetails.IsSelected = true;
            bookNameTextBox.Focus();
        }

        private void doDuplicate(object sender, RoutedEventArgs e)
        {
            int newYear = 0;
            int.TryParse(bookYearTextBox.Text, out newYear);
            string newName = bookNameTextBox.Text;
            string newAuthor = bookAuthorTextBox.Text;
            string newCategory = bookCategoryTextBox.Text;
            string newType = bookTypeTextBox.Text;
            string newNotes = txtNotes.Text;

            doNew(sender, e);

            bookNameTextBox.Text = newName;
            bookAuthorTextBox.Text = newAuthor;
            bookYearTextBox.Text = newYear.ToString();
            bookCategoryTextBox.Text = newCategory;
            bookTypeTextBox.Text = newType;
            txtNotes.Text = newNotes;

            bookBarCodeTextBox.Focus();
        }

        private void doGetNextNumber(object sender, RoutedEventArgs e)
        {
            try
            {
                double lastBarCode = -1;
                //Find the last (numeric) barcode the system knows about and add one to try to create a unique one
                SqlCeConnection sqlConn = new SqlCeConnection();
                sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                sqlConn.Open();
                string queryValue = "SELECT BookBarCode FROM Books Order By BookBarCode";
                SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                SqlCeDataReader rdr = newQuery.ExecuteReader();
                while (rdr.Read())
                {
                    string thisBarCode = (string)rdr["BookBarCode"];
                    double checkBarCode;
                    if (double.TryParse(thisBarCode, out checkBarCode))
                    {
                        if (checkBarCode > lastBarCode)
                            lastBarCode = checkBarCode;
                    }
                }
                lastBarCode++;

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
                bookBarCodeTextBox.Text = lastBarCode.ToString();
            }
            catch (Exception ex)
            {
                //give up
            }
        }

        private bool checkBarCodeExists(string barCodeToCheck)
        {
            //Find the last (numeric) barcode the system knows about and add one to try to create a unique one
            SqlCeConnection sqlConn = new SqlCeConnection();
            sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
            sqlConn.Open();
            string queryValue = "SELECT BookBarCode FROM Books Where BookBarCode='" + barCodeToCheck + "'";
            SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
            SqlCeDataReader rdr = newQuery.ExecuteReader();
            while (rdr.Read())
            {
                string thisBarCode = (string)rdr["BookBarCode"];
                if (thisBarCode == barCodeToCheck)
                    return true;
            }
            return false;
        }

        #endregion

        private void LocalizedWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double availSpace = e.NewSize.Width;
            availSpace = availSpace - (availSpace * 0.4);
            if (availSpace > 400)
                availSpace = e.NewSize.Width - 400;
            availSpace = availSpace - 40;   //room for scrollbars and margins/padding
            if (availSpace < 430)
                availSpace = 430;
           booksDataGrid.Width = availSpace;

            double allColumns = 0;
            allColumns = allColumns + bookIDColumn.ActualWidth;
            allColumns = allColumns + bookAuthorColumn.ActualWidth;
            allColumns = allColumns + bookYearColumn.ActualWidth;
            allColumns = allColumns + bookCategoryColumn.ActualWidth;
            allColumns = allColumns + bookTypeColumn.ActualWidth;
            allColumns = allColumns + bookBarCodeColumn.ActualWidth;

            availSpace = availSpace - allColumns;

            allColumns = allColumns + bookNameColumn.ActualWidth;

            if (availSpace > 0)
            {
                bookNameColumn.MaxWidth = bookNameColumn.MinWidth + availSpace;
                bookNameColumn.Width = booksDataGrid.Width - allColumns;
            }
            else
                bookNameColumn.Width = 200;            
        }

    }
}
