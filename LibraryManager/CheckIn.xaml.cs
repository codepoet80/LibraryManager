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
    /// Interaction logic for CheckInt.xaml
    /// </summary>
    public partial class CheckInWindow : LocalizedWindow
    {
        public CheckInWindow()
        {
            InitializeComponent();
            
        }

        private string checkOutNotFound, checkInComplete, checkInError;
        private int lendId = -1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateWindow();
            cmdOK.IsEnabled = false;
            txtBookID.Focus();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();

            checkOutNotFound = LanguageHelper("CheckOutNotFound", "A check-out record could not be found for this book. Ignore, or check the book out before attempting to return it!");
            checkInComplete = LanguageHelper("CheckInComplete", "Check-in completed successfully!");
            checkInError = LanguageHelper("CheckInError", "An error occured during check-in!");

            lblCheckInStatus.Content = "";
            cmdCancel_Click(null, null);
        }

        private void txtBookID_LostFocus(object sender, RoutedEventArgs e)
        {
            lblCheckInStatus.Content = "";

            if (txtBookID.Text != "")
            {
                string bookName = "";
                string borrowerName = "";
                //Try to Look up book
                SqlCeConnection sqlConn = new SqlCeConnection();
                sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                sqlConn.Open();
                string queryValue = "SELECT Lending.LendID, Books.BookName, Borrowers.BorrowerName, Books.BookBarCode FROM Lending INNER JOIN Books ON Lending.BookID = Books.BookBarCode INNER JOIN Borrowers ON Lending.BorrowerID = Borrowers.BorrowerBarCode WHERE (Lending.Returned = 'false') AND (Lending.BookID = '" + txtBookID.Text + "')";
                SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                SqlCeDataReader rdr = newQuery.ExecuteReader();
                while (rdr.Read())
                {
                    bookName = (string)rdr["BookName"];
                    borrowerName = (string)rdr["BorrowerName"];
                    lendId = (int)rdr["LendID"];
                }
                //Try to Look up book
                if (bookName != "" && lendId != -1)
                {
                    lblBookName.Content = bookName + " - " + borrowerName;
                    System.Media.SystemSounds.Beep.Play();
                    cmdOK.IsEnabled = true;
                    cmdOK.Focus();
                }

                if (bookName == "")
                {
                    txtBookID.SelectAll();
                    lblCheckInStatus.Content = checkOutNotFound;
                    lendId = -1;
                }
            }
            else
            {
                lblBookName.Content = "";
                lendId = -1;
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
                txtBookID.Text = "";
                lblBookName.Content = "";
                cmdOK.IsEnabled = false;
                lendId = -1;
                txtBookID.Focus();
            }

        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            txtBookID.Text = "";
            lblBookName.Content = "";
            lendId = -1;
            
            cmdOK.IsEnabled = false;
            txtBookID.Focus();
        }

        private void cmdOK_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cmdOK_Click(sender, new RoutedEventArgs());
            }
            if (e.Key == Key.Escape)
            {
                cmdOK.IsEnabled = false;
                txtBookID_PreviewKeyDown(sender, e);
            }
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            if (lblBookName.Content.ToString() != "")
            {
                try
                {
                    //change password
                    SqlCeConnection sqlConn = new SqlCeConnection();
                    sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                    sqlConn.Open();
                    string queryValue = "UPDATE LENDING SET Returned = 'true' WHERE LendId=" + lendId;
                    SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                    newQuery.ExecuteNonQuery();

                    lblCheckInStatus.Content = checkInComplete;
                    cmdCancel_Click(sender, e);
                    cmdOK.IsEnabled = false;
                    lendId = -1;
                }
                catch (Exception ex)
                {
                    lblCheckInStatus.Content = checkInError;
                    WindowsIdentity wi = WindowsIdentity.GetCurrent(); 
                    WindowsPrincipal wp = new WindowsPrincipal(wi);
                    if (wp.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        MessageBox.Show(ex.Message);
                    }
                    else
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                cmdCancel_Click(sender, e);
            }
        }
    }
}
