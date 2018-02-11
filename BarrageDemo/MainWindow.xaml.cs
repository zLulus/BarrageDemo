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
        BarrageConfig barrageConfig;
        private DispatcherTimer timer;
        
        private bool isOver;
        private object locker;

        public MainWindow()
        {
            InitializeComponent();

            barrageConfig = new BarrageConfig(canvas);
            hahaWindow.Width =System.Windows.SystemParameters.PrimaryScreenWidth;
            //设置弹幕高度
            hahaWindow.Height = barrageConfig.height;
        }

        public void ShowMessage(object sender, EventArgs e)
        {
            //如果时间间隔太短，可能出现字幕叠加，是因为同一时间几个计时器都在用数据 ->lock
            if (isOver)
            {
                lock (locker)
                {
                    isOver = false;
                }
                //异步解决卡顿的问题
                Task.Run(() =>
                {
                    //非UI线程调用UI组件
                    System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                    {
                        //查询新的弹幕
                        await barrageConfig.GetMessages();
                        barrageConfig.ReduceLengthList(barrageConfig.reduceSpeed);
                        //显示弹幕
                        barrageConfig.Barrage(barrageConfig.GetTopThreeMessages());
                    });
                });
                lock (locker)
                {
                    isOver = true;
                }
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isOver = true;
            locker = new object();
            barrageConfig.InitializeRuntimeParameters(canvas);
            //设置定时器
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);   //时间间隔
            timer.Tick += new EventHandler(ShowMessage);
            timer.Start();
        }

        private void hahaWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
