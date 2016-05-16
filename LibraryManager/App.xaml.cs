using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Shell;
using System.IO;
using System.Security.Principal;

namespace LibraryManager
{
    /// <summary>
    /// Library Manager Application with local database
    /// and multi-language support.
    /// Written by Jonathan Wise, September 2010
    /// for Dave Brubacher, SEND International, Canada
    /// for the Christian Resource Center in Ulan Ude, Siberia
    /// ------------------------------------------------------
    /// This application, for better or worse, in function and
    /// in defect, was written for the glory of God, that His
    /// name may be made known to the nations. It is free for
    /// use, re-use, modification and improvement, provided
    /// it is distributed without cost, and without removal
    /// of attribution.
    /// </summary>
    /// 
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string appDataPath;
            string CurrentDatabasePath = "default";
            
            //Determine Database path in %AppData% folder
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            appDataPath = appDataPath + "\\LibraryManager";

            //Local Database path override
            if (e.Args.Length > 0 && e.Args[0] == "-db")
            {
                appDataPath = Environment.CurrentDirectory + "\\..\\..";
            }

            //Open specific database override
            if (e.Args.Length > 0 && e.Args[0].IndexOf(":\\") != -1)
            {
                appDataPath = e.Args[0];
                appDataPath = appDataPath.ToLower().Replace("\\librarydata.sdf", "");
            }

            AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);
            if (e.Args.Length > 0)
            {
                CurrentDatabasePath = appDataPath + "\\LibraryData.sdf";
            }

            App.Current.Properties.Add("CurrentDatabasePath", CurrentDatabasePath);
        }
    }

}
