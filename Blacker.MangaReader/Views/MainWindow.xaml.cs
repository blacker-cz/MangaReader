using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Blacker.MangaReader.Services;
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

            var viewModel = new MainWindowViewModel(new InteractionService());
            viewModel.TurnPageRequested += OnTurnPageRequested;

            DataContext = viewModel;

            KeyDown += OnKeyDown;
            StateChanged += OnStateChanged;

            ShowTitleBar = WindowState != WindowState.Maximized;

            Height = Properties.Settings.Default.WindowHeight;
            Width = Properties.Settings.Default.WindowWidth;

            WindowState windowState;
            if (Enum.TryParse(Properties.Settings.Default.WindowState, out windowState))
            {
                WindowState = windowState;
            }
        }

        void OnTurnPageRequested(object sender, Utils.EventArgs<TurnPageRequestType> e)
        {
            switch (e.Value)
            {
                case TurnPageRequestType.Next:
                    BookControl.AnimateToNextPage(true, 600);
                    break;
                case TurnPageRequestType.Previous:
                    BookControl.AnimateToPreviousPage(true, 600);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            ShowTitleBar = WindowState != WindowState.Maximized;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape || e.Key == Key.F11) && WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else if (e.Key == Key.F11 && WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be the size of the screen
                Properties.Settings.Default.WindowHeight = RestoreBounds.Height;
                Properties.Settings.Default.WindowWidth = RestoreBounds.Width;
            }
            else
            {
                Properties.Settings.Default.WindowHeight = Height;
                Properties.Settings.Default.WindowWidth = Width;
            }

            Properties.Settings.Default.WindowState = WindowState.ToString();
        }
    }
}
