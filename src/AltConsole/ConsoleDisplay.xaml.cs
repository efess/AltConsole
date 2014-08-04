using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AltConsole
{
    /// <summary>
    /// Interaction logic for ConsoleDisplay.xaml
    /// </summary>
    public partial class ConsoleDisplay : UserControl
    {
        public static readonly DependencyProperty LinesProperty = DependencyProperty.Register("Lines", typeof(IEnumerable<char[]>), typeof(ConsoleDisplay), new PropertyMetadata(null, new PropertyChangedCallback(OnLinesChanged)));
        public static readonly DependencyProperty ScrollPositionProperty = DependencyProperty.Register("ScrollPosition", typeof(int), typeof(ConsoleDisplay), new FrameworkPropertyMetadata(0,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,(d,e) =>{}));
        public static readonly DependencyProperty MaximumScrollProperty = DependencyProperty.Register("MaximumScroll", typeof(int), typeof(ConsoleDisplay), new PropertyMetadata(0, new PropertyChangedCallback(OnMaximumScrollChanged)));
        public static readonly DependencyProperty ScreenWidthProperty = DependencyProperty.Register("ScreenWidth", typeof(int), typeof(ConsoleDisplay), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => { }));
        public static readonly DependencyProperty ScreenHeightProperty = DependencyProperty.Register("ScreenHeight", typeof(int), typeof(ConsoleDisplay), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => { }));
        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.Register("LineHeight", typeof(int), typeof(ConsoleDisplay), new PropertyMetadata(0, (d, e) => { }));

        public ConsoleDisplay()
        {
            InitializeComponent();
        }
        
        
        public int ViewableLinesCount
        {
            get
            {
                if (_canvas.ActualHeight > 0)
                {
                    return (int)Math.Ceiling((decimal)_canvas.ActualHeight / (decimal)LineHeight);
                }
                return 0;
            }
        }
        
        private void InternalUpdateOutput(IEnumerable<char[]> canvasData)
        {
            _canvas.Children.Clear();
            int i = 0;
            foreach (var chars in canvasData.Reverse())
            {
                DrawLine(chars, i++);
            }
        }
        //private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    var sb = sender as CustomScrollBar;
        //}

        private void _scroll_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //_displayHandler.SetLinePosition((int)e.NewValue);
        }

        private void textBlock1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _canvas.Focus();
        }
        
        private void LinesChanged(IEnumerable<char[]> newData)
        {
            InternalUpdateOutput(newData);
        }
        
        public IEnumerable<char[]> Lines
        {
            set { SetValue(LinesProperty, value); }
            get { return (IEnumerable<char[]>)GetValue(LinesProperty); }
         
        }
        public int ScrollPosition
        {
            set { SetValue(ScrollPositionProperty, value); }
            get { return (int)GetValue(ScrollPositionProperty); }
        }

        public int MaximumScroll
        {
            set { SetValue(MaximumScrollProperty, value); }
            get { return (int)GetValue(MaximumScrollProperty); }
        }

        public int ScreenWidth
        {
            set { SetValue(ScreenWidthProperty, value); }
            get { return (int)GetValue(ScreenWidthProperty); }
        }

        public int ScreenHeight
        {
            set { SetValue(ScreenHeightProperty, value); }
            get { return (int)GetValue(ScreenHeightProperty); }
        }
        public int LineHeight
        {
            set { SetValue(LineHeightProperty, value); }
            get { return (int)GetValue(LineHeightProperty); }
        }
        public static void OnMaximumScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (int)e.NewValue;

            ((ConsoleDisplay)d)._scroll.IsEnabled = value > 0;
            ((ConsoleDisplay)d)._scroll.Maximum = value;
            
        }
        public static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ConsoleDisplay)d).LinesChanged((IEnumerable<char[]>)e.NewValue);
        }
        public static void OnScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Don't think we care "right now"
            //((ConsoleDisplay)d).ScrollPositionChanged((int)e.NewValue);
        }      

        //FontUri             = "C:\WINDOWS\Fonts\COUR.TTF"
        //FontRenderingEmSize = "36"
        //StyleSimulations    = "BoldSimulation"
        //UnicodeString       = "Hello World!"
        //Fill                = "Maroon"
        //OriginX             = "50"
        //OriginY             = "300"

        private void DrawLine(char[] lineData, int lineNumber)
        {
            var text = new string(lineData);
            if (string.IsNullOrEmpty(text))
                return;
            var glyph = new Glyphs()
            {
                FontUri = new Uri(@"C:\WINDOWS\Fonts\CONSOLA.TTF"),
                FontRenderingEmSize = 12.0,
                StyleSimulations = System.Windows.Media.StyleSimulations.None,
                UnicodeString = text,
                Fill = new SolidColorBrush(Colors.White),
                Opacity= 1
                //OriginX = 5,
                //OriginY = lineNumber * LineHeight
            };
            Canvas.SetLeft(glyph, 5);
            Canvas.SetTop(glyph, lineNumber * LineHeight);
            _canvas.Children.Add(glyph);
        }

        private void _scroll_changed(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            ScrollPosition = (int)e.NewValue;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != e.PreviousSize.Height)
            {
                ScreenHeight = (int)e.NewSize.Height;
            }
            if (e.NewSize.Width != e.PreviousSize.Width)
            {
                ScreenWidth = (int)e.NewSize.Width;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _scroll.IsEnabled = false;
            ScreenHeight = (int)this.ActualHeight;
            ScreenWidth = (int)this.ActualWidth;
        }
    }
}
