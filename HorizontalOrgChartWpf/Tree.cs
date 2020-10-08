using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace HorizontalOrgChartWpf
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Tree<T>
    {
        private Tree<T> _parent = null;
        /// <summary>
        /// 
        /// </summary>
        public Tree<T> Parent { get { return _parent; } }

        private T _content;
        /// <summary>
        /// 
        /// </summary>
        public T Content { get { return _content; } }

        private List<Tree<T>> _childs = new List<Tree<T>>();
        /// <summary>
        /// 
        /// </summary>
        public List<Tree<T>> Childs { get { return _childs; } }
        private SizeF _size;
        /// <summary>
        /// 
        /// </summary>
        public SizeF Size { get { return _size; } }
        private Rectangle _rect;
        /// <summary>
        /// 
        /// </summary>
        public Rectangle Rect { get { return _rect; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="content"></param>
        public Tree(Tree<T> parent,T content)
        {
            _parent = parent;

            _content = content;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Tree<T> Add(T content)
        {
            Tree<T> tree = new Tree<T>(this, content);
            _childs.Add(tree);

            return tree;
        }

        /// <summary>
        /// 测量全部尺寸
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="font"></param>
        /// <param name="addWidth"></param>
        private void MeatureAllSize(Graphics  graphics,Font font,int addWidth)
        {
            _size = graphics.MeasureString(_content.ToString(), font);
            _size.Width += addWidth;
            foreach (Tree<T> tree in Childs)
            {
                tree.MeatureAllSize(graphics, font, addWidth);
            }
        }

        /// <summary>
        /// 获取树层级
        /// </summary>
        /// <returns></returns>
        private List<List<Tree<T>>> GetTreeLayers()
        {
            var layers = new List<List<Tree<T>>>();

            GetTreeLayers(layers, new List<Tree<T>>(new Tree<T>[] { this }), 0);

            return layers;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="childs"></param>
        /// <param name="level"></param>
        private void GetTreeLayers(List<List<Tree<T>>> layers,List<Tree<T>> childs,int level)
        {
            if (childs.Count == 0) return;
            if (layers.Count <= level)
            {
                layers.Add(new List<Tree<T>>());
            }

            for (int i = 0; i < childs.Count; i++)
            {
                layers[level].Add(childs[i]);
                GetTreeLayers(layers, childs[i].Childs, level + 1);
            }
        }

        /// <summary>
        /// 设置显示区域（从最后一层最左开始）
        /// </summary>
        /// <param name="level"></param>
        /// <param name="height"></param>
        /// <param name="interval"></param>
        /// <param name="left"></param>
        void SetRectangle(int level, int height, int hInterval, int vInterval, int left)
        {
            int index = 0;
            if (Parent != null) index = Parent.Childs.IndexOf(this);

            if (Childs.Count == 0)
            {
                // 没有儿子，就向前靠
                if (left > 0) left += hInterval;
            }
            else
            {
                // 有儿子，就在儿子中间
                int centerX = (Childs[0].Rect.Left + Childs[Childs.Count - 1].Rect.Right) / 2;
                left = centerX - (int)_size.Width / 2;

                // 并且不能和前面的重复，如果重复，联同子孙和子孙的右边节点右移
                if (Parent != null && index > 0)
                {
                    int ex = (Parent.Childs[index - 1]._rect.Right + hInterval) - left;
                    if (index > 0 && ex > 0)
                    {
                        for (int i = index; i < Parent.Childs.Count; i++)
                            Parent.Childs[i].RightChilds(ex);
                        left += ex;
                    }
                }
            }
            _rect = new Rectangle(left, (height + vInterval) * level, (int)_size.Width, height);
        }

        /// <summary>
        /// 所有子孙向右平移
        /// </summary>
        /// <param name="ex"></param>
        void RightChilds(int ex)
        {
            Rectangle rec;
            for (int i = 0; i < _childs.Count; i++)
            {
                rec = _childs[i].Rect;
                rec.Offset(ex, 0);
                _childs[i]._rect = rec;
                _childs[i].RightChilds(ex);
            }
        }

        void Offset(int x, int y)
        {
            _rect.Offset(x, y);
            
            for (int i = 0; i < _childs.Count; i++)
            {
                _childs[i].Offset(x, y);
            }
        }

        public Bitmap DrawAsImage()
        {
            return DrawAsImage(Pens.Black, new Font("宋体", 10.5f), 26, 20, 5, 20, 26);
        }

        public Bitmap DrawAsImage(Pen pen, Font font, int h, int horPadding,
            int horInterval, int verInterval, int borderWidth)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(bmp);
            // 把树扁平化
            List<List<Tree<T>>> layers = GetTreeLayers();

            // 算出每个单元的大小
            MeatureAllSize(g, font, horPadding);
            g.Dispose();
            bmp.Dispose();

            // 从最后一层开始排列
            int left = 0;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < layers[i].Count; j++)
                {
                    layers[i][j].SetRectangle(i, h, horInterval, verInterval, left);
                    left = layers[i][j].Rect.Right;
                }
            }

            Offset(borderWidth, borderWidth);

            // 获取画布需要的大小
            int maxHeight = (h + verInterval) * layers.Count - verInterval + borderWidth * 2;
            int maxWidth = 0;
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < layers[i].Count; j++)
                {
                    if (layers[i][j].Rect.Right > maxWidth)
                    {
                        maxWidth = layers[i][j].Rect.Right;
                    }
                }
            }
            maxWidth += borderWidth; // 边宽

            // 画
            bmp = new Bitmap(maxWidth, maxHeight);
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            StringFormat format = (StringFormat)StringFormat.GenericDefault.Clone();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            Rectangle rec, recParent;
            for (int i = 0; i < layers.Count; i++)
            {
                for (int j = 0; j < layers[i].Count; j++)
                {
                    // 画字
                    rec = (Rectangle)layers[i][j].Rect;
                    g.DrawRectangle(pen, rec);
                    g.DrawString(layers[i][j].Content.ToString(), font, new SolidBrush(pen.Color),
                        rec, format);
                    // 画到父亲的线
                    if (layers[i][j].Parent != null)
                    {
                        recParent = layers[i][j].Parent.Rect;
                        g.DrawLine(pen, rec.Left + rec.Width / 2, rec.Top, rec.Left + rec.Width / 2, rec.Top - verInterval / 2);
                        g.DrawLine(pen, recParent.Left + recParent.Width / 2, recParent.Bottom,recParent.Left + recParent.Width / 2, rec.Top - verInterval / 2);
                        g.DrawLine(pen, rec.Left + rec.Width / 2, rec.Top - verInterval / 2, recParent.Left + recParent.Width / 2, rec.Top - verInterval / 2);
                    }
                }
            }

            g.Flush();
            g.Dispose();

            return bmp;
        }

    }
}