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

namespace LibraryManager.Reports
{
    /// <summary>
    /// Interaction logic for BooksOnLoan.xaml
    /// </summary>
    public partial class BooksOnLoan : LocalizedReport
    {
        public BooksOnLoan()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BooksOnLoan_Loaded);
        }

        private void BooksOnLoan_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private LibraryManager.Reports.BooksOnLoanDataSet reportDataSet;
        private LibraryManager.Reports.BooksOnLoanDataSetTableAdapters.booksOnLoanDataTableTableAdapter reportDataSetBooksOnLoanTableAdapter;
        private System.Windows.Data.CollectionViewSource booksOnLoanDataTableViewSource;

        public override void LoadReport()
        {
            reportDataSet = ((LibraryManager.Reports.BooksOnLoanDataSet)(this.FindResource("booksOnLoanDataSet")));
            // Load data into the table Borrowers. You can modify this code as needed.
            reportDataSetBooksOnLoanTableAdapter = new LibraryManager.Reports.BooksOnLoanDataSetTableAdapters.booksOnLoanDataTableTableAdapter();
            reportDataSetBooksOnLoanTableAdapter.Fill(reportDataSet.booksOnLoanDataTable);
            booksOnLoanDataTableViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("booksOnLoanDataTableViewSource")));
            booksOnLoanDataTableViewSource.View.MoveCurrentToFirst();
        }
    }
}
