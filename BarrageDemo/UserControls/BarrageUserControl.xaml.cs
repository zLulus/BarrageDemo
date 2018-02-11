using BarrageDemo.BarrageParameters;
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
using System.Windows.Threading;

namespace BarrageDemo.UserControls
{
    /// <summary>
    /// BarrageUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BarrageUserControl : UserControl
    {
        BarrageConfig barrageConfig;
        private DispatcherTimer timer;

        private bool isOver;
        private object locker;
        public BarrageUserControl()
        {
            InitializeComponent();

            barrageConfig = new BarrageConfig(canvas);
            Barrage.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            //设置弹幕高度
            Barrage.Height = barrageConfig.height;
        }

        private void Barrage_Loaded(object sender, RoutedEventArgs e)
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
    }
}
