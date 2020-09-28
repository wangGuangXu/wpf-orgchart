using System.Windows;
using System.Windows.Controls;

namespace OrgChartWpf.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                //this.orgChart.Arrange(new Rect(new Size(this.orgChart.ActualWidth, this.orgChart.ActualHeight)));
                //printDialog.PrintVisual(this.orgChart, "");

                Window window = Window.GetWindow(orgChart);
                Point point = orgChart.TransformToAncestor(window).Transform(new Point(0, 0));//获取当前控件 的坐标
                this.orgChart.Measure(new Size(orgChart.ActualWidth, orgChart.ActualHeight));
                this.orgChart.Arrange(new Rect(new Size(orgChart.ActualWidth, orgChart.ActualHeight)));
                printDialog.PrintVisual(this.orgChart, "");
                this.orgChart.Arrange(new Rect(point.X, point.Y, orgChart.ActualWidth, orgChart.ActualHeight));//设置为原来的位置
            }
        }
    }
}
