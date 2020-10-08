using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HorizontalOrgChartWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            Tree<string> tree = new Tree<string>(null, "根");

            string[,] str = { { "a", "b", "c", "d", "e", "f", "g" }, { "1", "2", "3", "4", "5", "6", "7" }, { "10", "20", "30", "40", "50", "60", "70" } };

            for (int i = 0; i <= str.GetUpperBound(0); i++)
            {
                tree.Add(i.ToString());

                for (int j = 0; j <= str.GetUpperBound(1); j++)
                {
                    tree.Childs[i].Add(str[i, j]);
                    //string a = str[i, j];
                }
            }

            //tree.Add("北京公司").Add("a").Parent.Add("b").Add("c").Add("D");

            //tree.Add("董事秘书室特殊机构");

            //tree.Add("上海公司");

            //tree.Childs[0].Add("总经理办公室");

            //tree.Childs[0].Add("财务部");

            //tree.Childs[0].Add("销售部");

            //tree.Childs[1].Add("秘书长");

            //tree.Childs[1].Add("秘书总理");

            //tree.Childs[2].Add("上海销售部").Parent.Add("aaaaa");

            //tree.Childs[2].Add("上海分公司销售部").Add("无无针无针针");

            //Response.ContentType = "images/jpg";

            Bitmap bmp = tree.DrawAsImage();

            //bmp.Save(Response.OutputStream, ImageFormat.Jpeg);

            //Response.ClearContent();
        }
    }
}