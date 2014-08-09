using System;
using System.Collections.Generic;
using System.Drawing;
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
        public static readonly DependencyProperty CharacterDimensionsProperty = DependencyProperty.Register("CharacterDimensions", typeof(SizeF), typeof(ConsoleDisplay), new FrameworkPropertyMetadata(SizeF.Empty,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => { }));
        public static readonly DependencyProperty FontProperty = DependencyProperty.Register("Font", typeof(string), typeof(ConsoleDisplay), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnFontChanged)));
        public static readonly DependencyProperty CaretPositionProperty = DependencyProperty.Register("CaretPosition", typeof(int), typeof(ConsoleDisplay), new PropertyMetadata(0, (d, e) => { }));

        private System.Windows.Point _cursorLocation;
        private SizeF _glyphDimensions;
        private Glyphs _cursorGlyph;

        private string _fontFace;
        private int _fontSize;

        private DateTime _delay;

        public ConsoleDisplay()
        {
            InitializeComponent();
            _fontSize = 12; // will be configurable someday.
            _fontFace = @"C:\WINDOWS\Fonts\CONSOLA.TTF";
            
            _cursorLocation = new System.Windows.Point(0, 0);
            CreateCursor();
            _canvas.Children.Add(_cursorGlyph);
            new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 0, 5), System.Windows.Threading.DispatcherPriority.Input, (s, e) =>
            {
                if(_delay > DateTime.Now){
                    _cursorGlyph.Opacity = 1;
                    return;
                }
                    
                int ms = DateTime.Now.Millisecond;
                if (ms > 500)
                {
                    _cursorGlyph.Opacity = 0; 
                }
                else
                {
                    _cursorGlyph.Opacity = Math.Min((double)ms / 250, 1D);
                }
            }, this.Dispatcher).Start();
        }
        
        //public int ViewableLinesCount
        //{
        //    get
        //    {
        //        if (_canvas.ActualHeight > 0)
        //        {
        //            return (int)Math.Ceiling((decimal)_canvas.ActualHeight / (decimal)LineHeight);
        //        }
        //        return 0;
        //    }
        //}
        
        private void InternalUpdateOutput(IEnumerable<char[]> canvasData)
        {
            _canvas.Children.Clear();
            var array = canvasData.Reverse().ToArray();
            
            int lastLineLength = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == array.Length - 1)
                {
                    lastLineLength = array[i].Length + 1;
                }
                DrawLine(array[i], i);
            }
            _cursorLocation = new System.Windows.Point(lastLineLength + CaretPosition, array.Length - 1);
            _delay = DateTime.Now.AddMilliseconds(500);
            Canvas.SetTop(_cursorGlyph, _cursorLocation.Y * _glyphDimensions.Height);
            Canvas.SetLeft(_cursorGlyph, _cursorLocation.X * _glyphDimensions.Width);
            _canvas.Children.Add(_cursorGlyph);
        }

        //private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    var sb = sender as CustomScrollBar;
        //}

        private void _scroll_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            //_displayHandler.SetLinePosition((int)e.NewValue);
        }

        private void SetFont(string newFont)
        {
            if (_fontFace != newFont)
                return;
            _fontFace = newFont;
            var typeFace = new GlyphTypeface(new Uri(newFont)); // picking random index.. this needs to be a monotype font anyway.
            _glyphDimensions = new SizeF(
                (float)(typeFace.AdvanceWidths[3] * _fontSize),
                (float)(typeFace.AdvanceHeights[3] * _fontSize));
            CharacterDimensions = _glyphDimensions;
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
        public int CaretPosition
        {
            set { SetValue(CaretPositionProperty, value); }
            get { return (int)GetValue(CaretPositionProperty); }
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
        public SizeF CharacterDimensions
        {
            set { SetValue(CharacterDimensionsProperty, value); }
            get { return (SizeF)GetValue(CharacterDimensionsProperty); }
        }
        public static void OnMaximumScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (int)e.NewValue;

            ((ConsoleDisplay)d)._scroll.IsEnabled = value > 0;
            ((ConsoleDisplay)d)._scroll.Maximum = value;
            
        }
        public static void OnFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ConsoleDisplay)d).SetFont((string)e.NewValue);
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


        private void DrawLine(char[] lineData, int lineNumber)
        {
            var text = new string(lineData);
            if (string.IsNullOrEmpty(text))
                return;
            var glyph = new Glyphs()
            {
                FontUri = new Uri(_fontFace),
                
                FontRenderingEmSize = 12.0,
                StyleSimulations = System.Windows.Media.StyleSimulations.None,
                UnicodeString = text,
                Fill = new SolidColorBrush(Colors.White),
                Opacity = 1
            };

            Canvas.SetLeft(glyph, 5);
            Canvas.SetTop(glyph, lineNumber * _glyphDimensions.Height);
            
            _canvas.Children.Add(glyph);
        }

        private void CreateCursor()
        {
            _cursorGlyph = new Glyphs()
            {
                FontUri = new Uri(_fontFace),
                FontRenderingEmSize = 12.0,
                StyleSimulations = System.Windows.Media.StyleSimulations.None,
                UnicodeString = "_",
                Fill = new SolidColorBrush(Colors.White),
                Opacity = 1
                //OriginX = 5,
                //OriginY = lineNumber * LineHeight
            };
        }

        private void _scroll_changed(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            ScrollPosition = (int)e.NewValue;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
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
            SetFont(_fontFace);
        }
    }
}
