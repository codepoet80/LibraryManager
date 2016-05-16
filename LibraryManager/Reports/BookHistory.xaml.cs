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
    /// Interaction logic for BookHistory.xaml
    /// </summary>
    public partial class BookHistory : LocalizedReport
    {
        public BookHistory()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BookHistory_Loaded);
        }

        void BookHistory_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private LibraryManager.Reports.BookHistoryDataSet reportDataSet;
        private LibraryManager.Reports.BookHistoryDataSetTableAdapters.BookHistoryTableAdapter reportDataSetTableAdapter;
        private System.Windows.Data.CollectionViewSource reportDataTableViewSource;

        public override void LoadReport()
        {
            reportDataSet = ((LibraryManager.Reports.BookHistoryDataSet)(this.FindResource("bookHistoryDataSet")));
            // Load data into the table Borrowers. You can modify this code as needed.
            reportDataSetTableAdapter = new LibraryManager.Reports.BookHistoryDataSetTableAdapters.BookHistoryTableAdapter();
            reportDataSetTableAdapter.Fill(reportDataSet.BookHistoryQuery, parameter1Value);
            reportDataTableViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("bookHistoryQueryViewSource")));
            reportDataTableViewSource.View.MoveCurrentToFirst();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();
            parameter1Name = LanguageHelper("BookBarCode", "Book Bar Code:");
        }
    }
}
