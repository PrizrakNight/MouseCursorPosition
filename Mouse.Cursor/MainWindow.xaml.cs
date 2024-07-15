using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;

namespace Mouse.Cursor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer? _timer;

        private SynchronizationContext? _synchronizationContext;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }

            _synchronizationContext = SynchronizationContext.Current;

            _timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(200));
            _timer.Elapsed += TrackMouse;

            _timer.Start();
        }

        private void TrackMouse(object? sender, ElapsedEventArgs a)
        {
            _synchronizationContext?.Send(_ =>
            {
                if (GetCursorPos(out var point))
                {
                    MousePosLabel.Content = $"X: {point.X}, Y: {point.Y}";
                }

            }, sender);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}