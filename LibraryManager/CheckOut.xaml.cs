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
using System.Security.Principal;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for CheckOut.xaml
    /// </summary>
    public partial class CheckOutWindow : LocalizedWindow
    {
        List<string> checkOutBookList = new List<string>();

        public CheckOutWindow()
        {
            InitializeComponent();
            
        }

        private string bookNotFound, borrowerNotFound, checkOutComplete, checkOutError;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateWindow();
            cmdOK.IsEnabled = false;
            txtBorrowerID.Focus();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();

            bookNotFound = LanguageHelper("BookNotFound", "Book not found in Library. Create book first!");
            borrowerNotFound = LanguageHelper("BorrowerNotFound", "Borrower not found. Create borrower first!");
            checkOutComplete = LanguageHelper("CheckOutComplete", "Check out completed successfully!");
            checkOutError = LanguageHelper("CheckOutError", "An error occured during check-out!");

            lblCheckOutStatus.Content = "";
            cmdCancel_Click(null, null);
        }

        private void txtBorrowerID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBorrowerID.Text != "")
            {
                string borrowerName = "";
                bool borrowerSuspended = false;
                string borrowerNotes = "";
                //Try to Look up borrower
                SqlCeConnection sqlConn = new SqlCeConnection();
                sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                sqlConn.Open();
                string queryValue = "SELECT BorrowerName, BorrowerSuspended, BorrowerNotes FROM Borrowers WHERE BorrowerBarCode='" + txtBorrowerID.Text + "'";
                SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                SqlCeDataReader rdr = newQuery.ExecuteReader();
                while (rdr.Read())
                {
                    borrowerName = (string)rdr["BorrowerName"];
                    borrowerSuspended = (bool)rdr["BorrowerSuspended"];
                    borrowerNotes = (string)rdr["BorrowerNotes"];
                }

                if (borrowerName != "")
                {
                    lblBorrowerName.Content = borrowerName;
                    if (borrowerSuspended)
                    {
                        System.Media.SystemSounds.Exclamation.Play();
                        lblBorrowerName.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                        lblBorrowerName.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    if (borrowerNotes != "")
                    {
                        imgNotes.Visibility = Visibility.Visible;
                        ToolTip theTip = new ToolTip();
                        theTip.Content = borrowerNotes;
                        imgNotes.ToolTip = theTip;
                    }
                    else
                    {
                        imgNotes.Visibility = Visibility.Collapsed;
                    }

                    txtBookID.Text = "";
                    lblBookName.Content = "";
                    checkOutBookList.Clear();
                    txtBookID.Focus();

                }
                else //if borrower not found
                {
                    txtBorrowerID.SelectAll();
                    lblBorrowerName.Content = borrowerNotFound;
                    lblBorrowerName.Foreground = new SolidColorBrush(Colors.Black);
                    imgNotes.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                lblBookName.Content = "";
                checkOutBookList.Clear();
                imgNotes.Visibility = Visibility.Collapsed;
            }
        }

        private void txtBookID_LostFocus(object sender, RoutedEventArgs e)
        {
            lblCheckOutStatus.Content = "";

            if (txtBookID.Text == "")
            {
                if (lblBorrowerName.Content.ToString() != "" && lblBorrowerName.Content.ToString() != borrowerNotFound)
                {
                    if (checkOutBookList.Count > 0)
                        cmdOK.IsEnabled = true;
                }
                if (cmdOK.IsEnabled)
                    cmdOK.Focus();
            }
            else
            {
                string bookName = "";
                //Try to Look up book
                SqlCeConnection sqlConn = new SqlCeConnection();
                sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                sqlConn.Open();
                string queryValue = "SELECT BookName FROM Books WHERE BookBarCode='" + txtBookID.Text + "'";
                SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                SqlCeDataReader rdr = newQuery.ExecuteReader();
                while (rdr.Read())
                {
                    bookName = (string)rdr["BookName"];
                }

                if (bookName != "")
                {
                    checkOutBookList.Add(txtBookID.Text);

                    lblBookName.Content = lblBookName.Content + bookName + "\n";
                    System.Media.SystemSounds.Beep.Play();
                    txtBookID.Text = "";
                    if (lblBorrowerName.Content.ToString() != "" && lblBorrowerName.Content.ToString() != borrowerNotFound)
                        cmdOK.IsEnabled = true;

                }
                else //if book not found
                {
                    txtBookID.SelectAll();
                    lblBookName.Content = lblBookName.Content + bookNotFound + "\n";
                }
            }
        }

        private void txtBorrowerID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                txtBorrowerID_LostFocus(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                txtBorrowerID_LostFocus(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Escape)
            {
                txtBorrowerID.Text = "";
                lblBorrowerName.Content = "";
                imgNotes.Visibility = Visibility.Collapsed;
            }
        }

        private void txtBookID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                txtBookID_LostFocus(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Return)
            {
                e.Handled = true;
                txtBookID_LostFocus(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                if (txtBookID.Text == "")
                {
                    lblBookName.Content = "";
                    checkOutBookList.Clear();

                    txtBorrowerID.SelectAll();
                    lblBorrowerName.Content = "";
                    imgNotes.Visibility = Visibility.Collapsed;
                    txtBorrowerID.Focus();
                }
                else
                {
                    txtBookID.Text = "";
                    txtBookID.Focus();
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            checkOutBookList.Clear();

            txtBookID.Text = "";
            lblBookName.Content = "";
            txtBorrowerID.Text = "";
            lblBorrowerName.Content = "";
            imgNotes.Visibility = Visibility.Collapsed;
            
            cmdOK.IsEnabled = false;
            txtBorrowerID.Focus();
        }

        private void cmdOK_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                cmdOK_Click(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                cmdOK.IsEnabled = false;
                txtBookID_PreviewKeyDown(sender, e);
            }
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            if (checkOutBookList.Count > 0)
            {
                string queryValue = "";
                try
                {
                    //write check-out record
                    SqlCeConnection sqlConn = new SqlCeConnection();
                    sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                    sqlConn.Open();
                    string makeDate = System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + "/" + System.DateTime.Now.Year + " " + System.DateTime.Now.ToShortTimeString();
                    foreach (string thisBook in checkOutBookList)
                    {
                        queryValue = "INSERT INTO LENDING (BookID, BorrowerID, LendDate, Returned) VALUES ('" + thisBook + "', '" + txtBorrowerID.Text + "', '" + makeDate + "', 'false')";
                        SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                        newQuery.ExecuteNonQuery();
                    }

                    lblCheckOutStatus.Content = checkOutComplete;
                    cmdCancel_Click(sender, e);
                    cmdOK.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    lblCheckOutStatus.Content = checkOutError;
                    WindowsIdentity wi = WindowsIdentity.GetCurrent();
                    WindowsPrincipal wp = new WindowsPrincipal(wi);
                    if (wp.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        MessageBox.Show(ex.Message + "\n" + queryValue);
                    }
                    else
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(queryValue);
                    }
                }
            }
            else
            {
                cmdCancel_Click(sender, e);
            }
        }

        private void ScrollViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //remove last item from list
            string bookNames = lblBookName.Content.ToString();
            string[] bookName = bookNames.Split('\n');
            bookNames = "";
            for (int i = 0; i < bookName.Length - 2; i++)
            {
                bookNames = bookNames + bookName[i] + "\n";
            }
            
            //update
            lblBookName.Content = bookNames;
            checkOutBookList.RemoveAt(checkOutBookList.Count - 1);
            if (bookNames == "" || bookNames == string.Empty)
            {
                cmdOK.IsEnabled = true;
            }
        }
    }
}
