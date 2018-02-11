using BarrageDemo.BarrageParameters;
using LTM.WeiXin.WPF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BarrageDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //弹幕撑满屏幕宽度
            hahaWindow.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //设置弹幕高度
            hahaWindow.Height = int.Parse(ConfigurationManager.AppSettings["height"]);
        }

        /// <summary>
        /// 这里是为了可以拖拽弹幕  注意只有点中弹幕的字才可以拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hahaWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
