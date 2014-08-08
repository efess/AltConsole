using AltConsole.Interfaces;
using AltConsole.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AltConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HwndSource _hwndSource;

        public MainWindow()
        {
            SourceInitialized += OnSourceInitialized;
            InitializeComponent();

            SetItUp();
            PreviewMouseMove += OnPreviewMouseMove;
        }

        private void SetItUp()
        {
            // Set the external process 
            IExternalProcess externalProcess = new WindowsConsole();
            var inputHandler = new InputHandler(new WpfControlInput(display));
            var outputHandler = new BufferProvider();
            outputHandler.SetExternalProcess(externalProcess);
            var inputOutputHandler = new InputOutputBufferHandler(outputHandler, inputHandler);

            display.DataContext = new FixedDimensionsDisplayViewModel(inputOutputHandler, 20, 80);

            var process = new WindowsConsole();

            this.Closing += (s, e) => externalProcess.Stop();
            this.Activated += (s, e) => this.BorderBrush = new SolidColorBrush(Colors.Black) { Opacity = .2 };
            this.Deactivated += (s, e) => this.BorderBrush = new SolidColorBrush(Colors.Gray);

            externalProcess.Run();
        }

        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MaximizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void ChangeViewButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragableGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        public override void OnApplyTemplate()
        {
            /* omitted */

            if (resizeGrid != null)
            {
                foreach (UIElement element in resizeGrid.Children)
                {
                    Rectangle resizeRectangle = element as Rectangle;
                    if (resizeRectangle != null)
                    {
                        resizeRectangle.PreviewMouseDown += ResizeRectangle_PreviewMouseDown;
                        resizeRectangle.MouseMove += ResizeRectangle_MouseMove;
                    }
                }
            }

            base.OnApplyTemplate();
        }

        private void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        protected void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Cursors.Arrow;
        }
        protected void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    break;
            }
        }
        
        private void OnSourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        } 

        private void ResizeWindow(ResizeDirection direction)
        {
            Win32.SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        protected void ResizeRectangle_MouseMove(Object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "top":
                    Cursor = Cursors.SizeNS;
                    break;
                case "bottom":
                    Cursor = Cursors.SizeNS;
                    break;
                case "left":
                    Cursor = Cursors.SizeWE;
                    break;
                case "right":
                    Cursor = Cursors.SizeWE;
                    break;
                case "topLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "topRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "bottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindowPtr = new WindowInteropHelper(this).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            if (mainWindowSrc != null)
                if (mainWindowSrc.CompositionTarget != null)
                    mainWindowSrc.CompositionTarget.BackgroundColor = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

            var margins = new NonClientRegionAPI.MARGINS
            {
                cxLeftWidth = 0,
                cxRightWidth = Convert.ToInt32(this.Width) * Convert.ToInt32(this.Width),
                cyTopHeight = 0,
                cyBottomHeight = Convert.ToInt32(this.Height) * Convert.ToInt32(this.Height)
            };

            if (mainWindowSrc != null) NonClientRegionAPI.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            return;

            //try
            //{
            //    // Obtain the window handle for WPF application
            //    IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            //    HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            //    mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            //    // Get System Dpi
            //    System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
            //    float DesktopDpiX = desktop.DpiX;
            //    float DesktopDpiY = desktop.DpiY;

            //    // Set Margins
            //    NonClientRegionAPI.MARGINS margins = new NonClientRegionAPI.MARGINS();

            //    // Extend glass frame into client area 
            //    // Note that the default desktop Dpi is 96dpi. The  margins are 
            //    // adjusted for the system Dpi.
            //    margins.cxLeftWidth = Convert.ToInt32(5 * (DesktopDpiX / 96));
            //    margins.cxRightWidth = Convert.ToInt32(5 * (DesktopDpiX / 96));
            //    margins.cyTopHeight = Convert.ToInt32(5 * (DesktopDpiX / 96));
            //    margins.cyBottomHeight = Convert.ToInt32(5 * (DesktopDpiX / 96));

            //    int hr = NonClientRegionAPI.DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            //    // 
            //    if (hr < 0)
            //    {
            //        //DwmExtendFrameIntoClientArea Failed
            //    }
            //}
            //// If not Vista, paint background white. 
            //catch (DllNotFoundException)
            //{
            //    Application.Current.MainWindow.Background = Brushes.White;
            //}
        }

    }
}
