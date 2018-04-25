using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OrgChartWpf.Control
{
    public partial class OrgChart : TreeView
    {
        #region Variable

        private const string PART_BORDER = "PART_Border";

        #endregion

        #region Dependency Property

        #region LineBrush

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush",
                typeof(Brush),
                typeof(OrgChart),
                new PropertyMetadata(Brushes.Black, ChangedLineBrushProperty));

        private static void ChangedLineBrushProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var orgChart = obj as OrgChart;

            orgChart._pen.Brush = e.NewValue as Brush;
            orgChart.InvalidateVisual();
        }

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        #endregion        

        #region LineThickness

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness",
                typeof(double),
                typeof(OrgChart),
                new PropertyMetadata(1d, ChangedLineThicknessProperty));

        private static void ChangedLineThicknessProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var orgChart = obj as OrgChart;

            orgChart._pen.Thickness = (double)e.NewValue;
            orgChart.InvalidateVisual();
        }

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        #endregion

        #region ArrowSize

        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register("ArrowSize",
                typeof(double),
                typeof(OrgChart),
                new PropertyMetadata(6d, ChangedArrowSizeProperty));

        private static void ChangedArrowSizeProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var orgChart = obj as OrgChart;

            orgChart._pen.Thickness = (double)e.NewValue;
            orgChart.InvalidateVisual();
        }

        public double ArrowSize
        {
            get { return (double)GetValue(ArrowSizeProperty); }
            set { SetValue(ArrowSizeProperty, value); }
        }

        #endregion

        #region VerticalOffset

        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset",
                typeof(double),
                typeof(OrgChart),
                new PropertyMetadata(50d, ChangedVerticalOffsetProperty));

        private static void ChangedVerticalOffsetProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var orgChart = obj as OrgChart;

            orgChart.InvalidateVisual();
        }

        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        #endregion

        #region HorizontalOffset

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset",
                typeof(double),
                typeof(OrgChart),
                new PropertyMetadata(20d, ChangedHorizontalOffsetProperty));

        private static void ChangedHorizontalOffsetProperty(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var orgChart = obj as OrgChart;

            orgChart.InvalidateVisual();
        }

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        #endregion

        #endregion

        #region Variable

        private Pen _pen;

        #endregion

        #region Constructor

        public OrgChart()
        {
            InitializeComponent();

            _pen = new Pen(LineBrush, LineThickness);

            Background = Brushes.Transparent;
            ClipToBounds = true;
            BorderThickness = new Thickness(0);

            Loaded += (sender, args) =>
            {
                AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler((s, e) => { InvalidateVisual(); }));
                AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler((s, e) => { InvalidateVisual(); }));
                AddHandler(TreeViewItem.CollapsedEvent, new RoutedEventHandler((s, e) => { InvalidateVisual(); }));
            };
        }

        #endregion

        #region Protected Method

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (ItemContainerGenerator.Items.Count == 0)
            {
                return;
            }

            var item = ItemContainerGenerator.Items[0];
            DrawLine(item, ItemContainerGenerator, drawingContext);
            
            base.OnRender(drawingContext);
        }

        #endregion

        #region Private Method

        private void DrawLine(object item, ItemContainerGenerator itemContainerGenerator, DrawingContext drawingContext)
        {
            var rootItem = itemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

            if (rootItem == null || !rootItem.IsExpanded) return;

            var items = rootItem.ItemContainerGenerator.Items;

            if (items.Count > 0)
            {
                var firstItem = rootItem.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
                var lastItem = rootItem.ItemContainerGenerator.ContainerFromIndex(items.Count - 1) as TreeViewItem;

                var firstItemRect = GetItemRect(firstItem);
                var rootItemRect = GetItemRect(rootItem);

                var lineHeight = (firstItemRect.Top - rootItemRect.Bottom) / 2;

                DrawBottomLine(rootItem, lineHeight, drawingContext);
                DrawTopLine(firstItem, lineHeight, drawingContext);

                DrawLine(firstItem.Header, rootItem.ItemContainerGenerator, drawingContext);
                
                for (int i = 1; i < items.Count; i++)
                {
                    var _item = items[i];
                    var subItem = rootItem.ItemContainerGenerator.ContainerFromItem(_item) as TreeViewItem;
                      
                    DrawTopLine(subItem, lineHeight, drawingContext);
                    DrawLine(_item, rootItem.ItemContainerGenerator, drawingContext);
                }

                DrawHorizontalLine(firstItem, lastItem, lineHeight, drawingContext);
            }
        }

        private void DrawBottomLine(TreeViewItem item, double lineHeight, DrawingContext drawingContext)
        {
            var rect = GetItemRect(item);

            var point1 = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height);
            var point2 = new Point(point1.X, point1.Y + lineHeight);
            
            drawingContext.DrawLine(_pen, point1, point2);
        }

        private void DrawTopLine(TreeViewItem item, double lineHeight, DrawingContext drawingContext)
        {
            var rect = GetItemRect(item);

            var point1 = new Point(rect.X + rect.Width / 2, rect.Y);
            var point2 = new Point(point1.X, point1.Y - lineHeight);

            drawingContext.DrawLine(_pen, point1, point2);

            DrawTriangle(rect, lineHeight, drawingContext);
        }

        private void DrawTriangle(Rect rect, double lineHeight, DrawingContext drawingContext)
        {
            var arrowHalfSize = ArrowSize / 2;
            var point1 = new Point(rect.X + rect.Width / 2 - arrowHalfSize, rect.Y - arrowHalfSize);
            var point2 = new Point(point1.X + ArrowSize, point1.Y);
            var point3 = new Point(point2.X - arrowHalfSize, point2.Y + arrowHalfSize);

            var streamGeometry = new StreamGeometry();

            using (var geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(point1, true, true);
                PointCollection points = new PointCollection { point2, point3 };
                geometryContext.PolyLineTo(points, true, true);
            }

            streamGeometry.Freeze();
            drawingContext.DrawGeometry(_pen.Brush, _pen, streamGeometry);
        }

        private void DrawHorizontalLine(TreeViewItem firstItem, TreeViewItem lastItem, double lineHeight, DrawingContext drawingContext)
        {
            var rect1 = GetItemRect(firstItem);
            var rect2 = GetItemRect(lastItem);
            
            var point1 = new Point(rect1.X + rect1.Width / 2, rect1.Y - lineHeight);
            var point2 = new Point(rect2.X + rect2.Width / 2, rect2.Y - lineHeight);

            drawingContext.DrawLine(_pen, point1, point2);
        }

        private Rect GetItemRect(TreeViewItem item)
        {
            var content = item.Template.FindName(PART_BORDER, item) as FrameworkElement;

            if(content == null)
            {
                throw new NotImplementedException(PART_BORDER + " is not found.");
            }

            content.Margin = new Thickness(HorizontalOffset, 0, HorizontalOffset, VerticalOffset);

            return content.TransformToAncestor(this).TransformBounds(new Rect(0, 0, content.ActualWidth, content.ActualHeight));
        }

        #endregion
    }
}