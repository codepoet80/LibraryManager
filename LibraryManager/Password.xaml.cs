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
using System.IO;
using System.Security.Principal;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for Password.xaml
    /// </summary>
    public partial class PasswordWindow : LocalizedWindow
    {
        public PasswordWindow()
        {
            InitializeComponent();
        }

        private bool recoveryMode = false;

        private bool changePassword = false;
        public bool ChangePassword
        {
            set
            {
                changePassword = value;
                configWindow();
            }
            get
            {
                return changePassword;
            }
        }

        private void configWindow()
        {
            if (changePassword)
            {
                lblNewPassword1.Visibility = Visibility.Visible;
                lblNewPassword2.Visibility = Visibility.Visible;
                txtNewPassword1.Visibility = Visibility.Visible;
                txtNewPassword2.Visibility = Visibility.Visible;
            }
            else
            {
                lblNewPassword1.Visibility = Visibility.Collapsed;
                lblNewPassword2.Visibility = Visibility.Collapsed;
                txtNewPassword1.Visibility = Visibility.Collapsed;
                txtNewPassword2.Visibility = Visibility.Collapsed;
            }
        }

        int attemptCount = 1;
        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            //see if current password is correct
            if (checkPasswordCorrect(txtCurrentPassword.Password))
            {
                if (changePassword)
                {
                    if (txtNewPassword1.Password != "" && txtNewPassword1.Password == txtNewPassword2.Password)
                    {
                        //change password
                        SqlCeConnection sqlConn = new SqlCeConnection();
                        sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                        sqlConn.Open();
                        string queryValue = "UPDATE Settings SET Password = '" + txtNewPassword1.Password + "' Where UserId=1";
                        SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                        newQuery.ExecuteNonQuery();
                        DialogResult = true;
                    }
                    else
                    {
                        txtNewPassword2.Password = "";
                        txtNewPassword1.Focus();
                        txtNewPassword1.SelectAll();
                    }
                }
                else
                {
                    DialogResult = true;
                }
            }
            else
            {
                if (attemptCount < 3)
                {
                    txtNewPassword1.Password = "";
                    txtNewPassword2.Password = "";
                    txtCurrentPassword.Focus();
                    txtCurrentPassword.SelectAll();
                    attemptCount++;
                }
                else
                {
                    //Beep?
                    DialogResult = false;
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool checkPasswordCorrect(string checkPassword)
        {
            if (recoveryMode && checkPassword == "1234")
                return true;

            string actualPassword = "1234";
            try
            {
                SqlCeConnection sqlConn = new SqlCeConnection();
                sqlConn.ConnectionString = LibraryManager.Properties.Settings.Default.LibraryDataConnectionString;
                sqlConn.Open();
                string queryValue = "SELECT Password FROM Settings WHERE UserId=1";
                SqlCeCommand newQuery = new SqlCeCommand(queryValue, sqlConn);
                SqlCeDataReader rdr = newQuery.ExecuteReader();
                while (rdr.Read())
                {
                    actualPassword = (string)rdr["Password"];
                }
            }
            catch (Exception ex)
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);
                if (wp.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("Error checking password in database, assuming default password! " + ex.Message);
                }
                else
                {
                    Console.WriteLine("Error checking password in database, assuming default password! " + ex.Message);
                }
            }
            if (actualPassword == checkPassword)
                return true;

            return false;
        }

        private void LocalizedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Check for recovery file and Run As Administrator
            WindowsIdentity wi = WindowsIdentity.GetCurrent(); 
            WindowsPrincipal wp = new WindowsPrincipal(wi);
            if (wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                if (File.Exists("c:\\RecoverLMPasswordJW.txt"))
                {
                    recoveryMode = true;
                    txtCurrentPassword.Password = "1234";
                    changePassword = true;
                    MessageBox.Show(LanguageHelper("RecoveryModeWarning", "Password Recovery Mode - Delete recovery file when done!"));
                    txtNewPassword1.Focus();
                }
            }
            configWindow();

            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
                txtCurrentPassword.Focus();
        }



        private void txtCurrentPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!changePassword && e.Key == Key.Enter)
            {
                e.Handled = true;
                cmdOK_Click(sender, null);
            }
        }

    }
}
