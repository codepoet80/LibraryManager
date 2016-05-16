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
    /// Interaction logic for BorrowerHistory.xaml
    /// </summary>
    public partial class BorrowerHistory : LocalizedReport
    {
        public BorrowerHistory()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BorrowerHistory_Loaded);
        }

        void BorrowerHistory_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReport();
        }

        private LibraryManager.Reports.BorrowerHistoryDataSet reportDataSet;
        private LibraryManager.Reports.BorrowerHistoryDataSetTableAdapters.BorrowerHistoryTableAdapter reportDataSetTableAdapter;
        private System.Windows.Data.CollectionViewSource reportDataTableViewSource;

        public override void LoadReport()
        {
            reportDataSet = ((LibraryManager.Reports.BorrowerHistoryDataSet)(this.FindResource("borrowerHistoryDataSet")));
            // Load data into the table Borrowers. You can modify this code as needed.
            reportDataSetTableAdapter = new LibraryManager.Reports.BorrowerHistoryDataSetTableAdapters.BorrowerHistoryTableAdapter();
            reportDataSetTableAdapter.Fill(reportDataSet.BorrowerHistoryQuery, parameter1Value);
            reportDataTableViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("borrowerHistoryQueryViewSource")));
            reportDataTableViewSource.View.MoveCurrentToFirst();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();
            parameter1Name = LanguageHelper("BorrowerBarCode", "Borrower Code:");
        }
    }
}
