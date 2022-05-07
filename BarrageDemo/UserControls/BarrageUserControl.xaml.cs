using BarrageDemo.BarrageParameters;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            barrageConfig.InitializeRuntimeParameters(canvas);
            //设置定时器
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);   //时间间隔
            timer.Tick += new EventHandler(ShowMessage);
            timer.Start();
        }

        public void ShowMessage(object sender, EventArgs e)
        {
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
        }
    }
}
