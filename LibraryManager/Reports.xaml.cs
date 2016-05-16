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
using System.Printing;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class ReportsWindow : LocalizedWindow
    {
        public ReportsWindow()
        {
            InitializeComponent();
        }

        private string currentReport = "";
        public string CurrentReport
        {
            set
            {
                currentReport = value;
                try
                {
                    getReport();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Tried to load a report that doesn't exist! " + ex.Message);
                    this.Close();
                }
            }
            get
            {
                return currentReport;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TranslateWindow();
        }

        public override void TranslateWindow()
        {
            base.TranslateWindow();
            doRefresh(null, null);
        }

        private void getReport()
        {
            if (currentReport != null && currentReport != "")
            {
                Uri thisReport = new System.Uri("/Reports/" + currentReport + ".xaml", System.UriKind.Relative);
                frmReport.Navigate(thisReport);
                frmReport.Navigated += new System.Windows.Navigation.NavigatedEventHandler(frmReport_Navigated);
            }
        }

        private void frmReport_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                LocalizedReport thisReport = (LocalizedReport)frmReport.Content;
                thisReport.LanguageHelper = this.LanguageHelper;
                if (thisReport.Parameter1 != null && thisReport.Parameter1 != "")
                {
                    lblParameterName.Visibility = Visibility.Visible;
                    lblParameterName.Content = thisReport.Parameter1;
                    txtParameterValue.Visibility = Visibility.Visible;
                    txtParameterValue.Text = "";
                    cmdParameterSet.Visibility = Visibility.Visible;
                    txtParameterValue.Focus();
                }
                else
                {
                    lblParameterName.Visibility = Visibility.Collapsed;
                    txtParameterValue.Visibility = Visibility.Collapsed;
                    txtParameterValue.Text = "";
                    cmdParameterSet.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown report type being loaded. " + ex.Message);
            }
        }

        private void doRefresh(object sender, RoutedEventArgs e)
        {
            frmReport.Refresh();
        }

        private void doPrint(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            if (printDlg.ShowDialog() == true)
            {
                //get selected printer capabilities
                System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                this.ActualHeight);

                //Transform the Visual to scale
                this.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                this.Measure(sz);
                this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                printDlg.PrintVisual(frmReport, LanguageHelper(currentReport, currentReport));

            }
        }

        private void txtParameterValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                cmdParameterSet_Click(sender, null);
            }
        }

        private void cmdParameterSet_Click(object sender, RoutedEventArgs e)
        {
            if (txtParameterValue.Text != "")
            {
                LocalizedReport thisReport = (LocalizedReport)frmReport.Content;
                thisReport.Parameter1 = txtParameterValue.Text;
            }
        }
    }
}
