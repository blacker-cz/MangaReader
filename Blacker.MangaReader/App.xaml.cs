using System;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Blacker.MangaReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(App));

        public App()
        {
            // Initialize log4net
            log4net.Config.XmlConfigurator.Configure();

            _log.InfoFormat("Starting up MangaScraper. Assembly version: {0}. Targeted framework: {1} {2}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                AssemblyInfo.TargetFramework,
                AssemblyInfo.TargetFrameworkVersion);
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            // define application exception handler
            Current.DispatcherUnhandledException += AppDispatcherUnhandledException;

            MigrateSettings();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            // save settings on exit
            MangaReader.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Migrate user settings in case of upgrade
        /// </summary>
        private void MigrateSettings()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            Version appVersion = a.GetName().Version;
            string appVersionString = appVersion.ToString();

            if (MangaReader.Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                MangaReader.Properties.Settings.Default.Upgrade();
                MangaReader.Properties.Settings.Default.ApplicationVersion = appVersionString;
            }
        }

        void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //process exception
#if !DEBUG
            MessageBox.Show("Application encountered following unrecoverable error and will now shut down:\n\n\"" + e.Exception.Message + "\"", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            _log.Fatal("Unexpected error.", e.Exception);

            e.Handled = true;
            
            // kill application
            this.MainWindow.Close();
#endif
        }
    }
}
