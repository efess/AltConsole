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
        public MainWindow()
        {
            InitializeComponent();

            SetItUp();
        }

        private void SetItUp()
        {
            // Set the external process 
            IExternalProcess externalProcess = new WindowsConsole();
            var inputHandler = new WpfControlInput(display);
            var outputHandler = new BufferProvider();
            outputHandler.SetExternalProcess(externalProcess);
            var inputOutputHandler = new InputOutputBufferHandler(outputHandler, inputHandler);

            display.DataContext = new FixedDimensionsDisplayViewModel(inputOutputHandler, 20, 80);

            var process = new WindowsConsole();

            this.Closing += (s, e) => externalProcess.Stop();
            externalProcess.Run();
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
