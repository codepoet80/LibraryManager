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
using System.Security.Principal;

namespace LibraryManager
{
    /// <summary>
    /// Interaction logic for LanguagesWindow.xaml
    /// </summary>
    public partial class LanguagesWindow : LocalizedWindow
    {
        public LanguagesWindow()
        {
            InitializeComponent();
        }

        LibraryManager.LibraryDataDataSet libraryDataDataSet;
        LibraryManager.LibraryDataDataSetTableAdapters.LanguageTableAdapter libraryDataDataSetLanguageTableAdapter;
        System.Windows.Data.CollectionViewSource languageViewSource;

        private void LocalizedWindow_Loaded(object sender, RoutedEventArgs e)
        {

            libraryDataDataSet = ((LibraryManager.LibraryDataDataSet)(this.FindResource("libraryDataDataSet")));
            // Load data into the table Language. You can modify this code as needed.
            libraryDataDataSetLanguageTableAdapter = new LibraryManager.LibraryDataDataSetTableAdapters.LanguageTableAdapter();
            libraryDataDataSetLanguageTableAdapter.Fill(libraryDataDataSet.Language);
            languageViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("languageViewSource")));
            languageViewSource.View.MoveCurrentToFirst();

            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal wp = new WindowsPrincipal(wi);
            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                languageDataGrid.CanUserDeleteRows = false;
                languageDataGrid.CanUserAddRows = false;
                promptLanguageIdColumn.IsReadOnly = true;
                promptTagColumn.IsReadOnly = true;
            }
            promptIdColumn.IsReadOnly = true;
        }

        private void doSave(object sender, RoutedEventArgs e)
        {
            libraryDataDataSetLanguageTableAdapter.Update(libraryDataDataSet);
            libraryDataDataSet.Languages.AcceptChanges();
        }

    }
}
