using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
            PreviewKeyDown += OnPreviewKeyDown;
            StateChanged += OnStateChanged;
            Closing += OnWindowClosing;

            Height = Properties.Settings.Default.WindowHeight;
            Width = Properties.Settings.Default.WindowWidth;

            WindowState windowState;
            if (Enum.TryParse(Properties.Settings.Default.WindowState, out windowState))
            {
                WindowState = windowState;
            }

            ShowTitleBar = WindowState != WindowState.Maximized;

            BookViewbox.MouseWheel += BookViewboxOnMouseWheel;
            BookViewbox.MouseMove += BookViewboxOnMouseMove;
        }

        private void OnTurnPageRequested(object sender, Utils.EventArgs<TurnPageRequestType> e)
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

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // this is a bit "hacky" but it solves the issue with focus on some active button when the SPACE key is pressed
            if (e.Key == Key.Space)
            {
                var vm = DataContext as MainWindowViewModel;

                if (vm != null && vm.NextPageCommand.CanExecute(null))
                {
                    vm.NextPageCommand.Execute(null);
                    e.Handled = true;
                }
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

        private void BookViewboxOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = TryGetTransform<ScaleTransform>(BookViewbox);

            double zoom = e.Delta > 0 ? 0.1 : -0.1;

            if ((!(e.Delta > 0) && (st.ScaleX <= 1 || st.ScaleY <= 1)) || (e.Delta > 0 && (st.ScaleX > 3 || st.ScaleY > 3)))
                return;

            Point relative = e.GetPosition(BookViewbox);

            st.ScaleX += zoom;
            st.ScaleY += zoom;

            if (Math.Abs(st.ScaleX - 1) < 0.001 || Math.Abs(st.ScaleY - 1) < 0.001)
            {
                st.ScaleX = 1;
                st.ScaleY = 1;

                ResetZoomOrigin(st);
            }
            else
            {
                st.CenterX = relative.X;
                st.CenterY = relative.Y;
            }
        }

        private void BookViewboxOnMouseMove(object sender, MouseEventArgs e)
        {
            var st = TryGetTransform<ScaleTransform>(BookViewbox);

            if (Math.Abs(st.ScaleX - 1) > 0.001 && Math.Abs(st.ScaleY - 1) > 0.001)
            {
                var position = e.GetPosition(BookViewbox);

                st.CenterX = position.X;
                st.CenterY = position.Y;
            }
            else
            {
                ResetZoomOrigin(st);
            }
        }

        private void ResetZoomOrigin(ScaleTransform scaleTransform)
        {
            // this will set the scale transform center to center of book
            scaleTransform.CenterX = BookViewbox.ActualWidth / 2;
            scaleTransform.CenterY = BookViewbox.ActualHeight / 2;
        }

        private T TryGetTransform<T>(UIElement element) where T : Transform
        {
            return ((TransformGroup) element.RenderTransform).Children.FirstOrDefault(tr => tr is T) as T;
        }
    }
}
