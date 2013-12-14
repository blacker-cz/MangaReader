using System;
using System.Windows;
using System.Windows.Input;
using Blacker.MangaReader.ViewModels;
using MahApps.Metro.Controls;

namespace Blacker.MangaReader.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();

            KeyDown += OnKeyDown;
            StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            ShowTitleBar = WindowState != WindowState.Maximized;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}
