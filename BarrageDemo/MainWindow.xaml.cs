using LTM.WeiXin.WPF.Models;
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
        /// <summary>
        /// 每行的起始高度
        /// </summary>
        private List<double> heightList;
        /// <summary>
        /// 每行的弹幕未显示长度
        /// </summary>
        private List<double> lengthList;
        /// <summary>
        /// 当前可以加弹幕的位置（行数）集合
        /// </summary>
        private List<int> locations;
        private List<Color> colorList;
        private DispatcherTimer timer;
        private Random random;
        /// <summary>
        /// 存储当前没有展现的弹幕
        /// </summary>
        private List<MessageInformation> messages;
        /// <summary>
        /// 弹幕行数
        /// </summary>
        private int rowCount = 3;
        private bool isOver;
        private object locker;

        public MainWindow()
        {
            InitializeComponent();

            //弹幕颜色  初始化
            colorList = new List<Color>();
            colorList.Add(Color.FromRgb(255, 255, 255));
            colorList.Add(Color.FromRgb(0, 0, 0));
            colorList.Add(Color.FromRgb(0, 255, 0));
            colorList.Add(Color.FromRgb(0, 0, 255));

            colorList.Add(Color.FromRgb(0, 0, 102));
            colorList.Add(Color.FromRgb(0, 0, 204));
            colorList.Add(Color.FromRgb(153, 0, 0));
            colorList.Add(Color.FromRgb(153, 0, 102));
            colorList.Add(Color.FromRgb(153, 0, 204));

            colorList.Add(Color.FromRgb(0, 255, 102));
            colorList.Add(Color.FromRgb(0, 255, 204));
            colorList.Add(Color.FromRgb(153, 255, 0));
            colorList.Add(Color.FromRgb(153, 255, 102));
            colorList.Add(Color.FromRgb(153, 255, 204));

            colorList.Add(Color.FromRgb(255, 0, 0));
            colorList.Add(Color.FromRgb(102, 0, 102));
            colorList.Add(Color.FromRgb(102, 0, 204));
            colorList.Add(Color.FromRgb(255, 0, 102));
            colorList.Add(Color.FromRgb(255, 0, 204));

            colorList.Add(Color.FromRgb(102, 255, 102));
            colorList.Add(Color.FromRgb(102, 255, 204));
            colorList.Add(Color.FromRgb(255, 255, 0));
            colorList.Add(Color.FromRgb(255, 255, 102));
            colorList.Add(Color.FromRgb(255, 255, 204));

            messages = new List<MessageInformation>();
            random = new Random();
            hahaWindow.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
        }

        public void ShowMessage(object sender, EventArgs e)
        {
            //如果时间间隔太短，可能出现字幕叠加，是因为同一时间几个计时器都在用数据 ->lock
            if (isOver)
            {
                //异步解决卡顿的问题
                Task.Run(() =>
                {
                    lock (locker)
                    {
                        isOver = false;
                    }
                    //非UI线程调用UI组件
                    System.Windows.Application.Current.Dispatcher.Invoke(async () =>
                    {
                        //查询新的弹幕
                        await GetMessages();
                        ReduceLengthList();
                        //显示弹幕
                        Barrage(GetTopThreeMessages());
                        lock (locker)
                        {
                            isOver = true;
                        }
                    });
                });
            }


        }

        /// <summary>
        /// 按时间减少每行的未显示长度
        /// </summary>
        private void ReduceLengthList()
        {
            for (int i = 0; i < lengthList.Count; i++)
            {
                if (lengthList[i] > 0)
                {
                    //这里根据动画的速度调节，每个计时器的间隔走了X个字
                    lengthList[i] -= 0.5;
                }
                //减得过多了，重置为0，认为该行已展示完成
                if (lengthList[i] < 0)
                {
                    lengthList[i] = 0;
                }
            }
        }

        /// <summary>
        /// 查询新的弹幕
        /// </summary>
        private async Task GetMessages()
        {
            GetMessageInformationResult newMessages = new GetMessageInformationResult();
            newMessages.result = new List<MessageInformation>();
            newMessages.result.Add(new MessageInformation() { content = "你们的" });
            newMessages.result.Add(new MessageInformation() { content = "生活" });
            newMessages.result.Add(new MessageInformation() { content = "真丰富" });
            newMessages.result.Add(new MessageInformation() { content = "不像我" });
            newMessages.result.Add(new MessageInformation() { content = "帅" });
            newMessages.result.Add(new MessageInformation() { content = "字" });
            newMessages.result.Add(new MessageInformation() { content = "竟贯穿了" });
            newMessages.result.Add(new MessageInformation() { content = "一生" });

            messages.AddRange(newMessages.result);
        }

        /// <summary>
        /// 获得最新的三个弹幕
        /// </summary>
        /// <returns></returns>
        private List<MessageInformation> GetTopThreeMessages()
        {
            List<MessageInformation> infos = new List<MessageInformation>();
            //查询哪些行数的字已经显示完成
            GetAllShowLocations();
            int number = locations.Count;
            for (int i = 0; i < number; i++)
            {
                infos.Add(messages[i]);
            }
            messages.RemoveRange(0, number);
            return infos;
        }

        /// <summary>
        /// 查询哪些行数的字已经显示完成
        /// </summary>
        private void GetAllShowLocations()
        {
            //重新初始化
            locations = new List<int>();
            for (int i = 0; i < lengthList.Count; i++)
            {
                if (lengthList[i] == 0)
                {
                    locations.Add(i);
                }
            }
        }

        /// <summary>
        /// 在Window界面上显示弹幕信息,速度和位置随机产生
        /// </summary>
        /// <param name="contentlist"></param>
        public void Barrage(List<MessageInformation> contentlist)
        {
            Random random = new Random();
            //当前读取弹幕的位置
            for (int i = 0; i < contentlist.Count(); i++)
            {
                //获取高度位置(置顶、三分之一、三分之二)
                double inittop = GetHeight(locations[i]);
                //获取速度随机数
                //double randomspeed = random.NextDouble();
                double initspeed = 30;
                //实例化TextBlock和设置基本属性,并添加到Canvas中
                TextBlock textblock = new TextBlock();
                textblock.Text = contentlist[i].content;
                textblock.FontSize = 20;
                textblock.Foreground = GetRandomColor();
                //记录该行的未显示字幕数
                lengthList[locations[i]] = lengthList[locations[i]] + contentlist[i].content.Length;
                //这里设置了弹幕的高度
                Canvas.SetTop(textblock, inittop);
                canvas.Children.Add(textblock);
                //实例化动画
                DoubleAnimation animation = new DoubleAnimation();
                Timeline.SetDesiredFrameRate(animation, 60);  //如果有性能问题,这里可以设置帧数
                                                              //从右往左
                animation.From = canvas.ActualWidth;
                animation.To = 0;
                animation.Duration = TimeSpan.FromSeconds(initspeed);
                animation.AutoReverse = false;
                animation.Completed += (object sender, EventArgs e) =>
                {
                    canvas.Children.Remove(textblock);
                };
                //启动动画
                textblock.BeginAnimation(Canvas.LeftProperty, animation);
            }

        }

        /// <summary>
        /// 颜色随机
        /// </summary>
        /// <returns></returns>
        private Brush GetRandomColor()
        {
            int index = random.Next(colorList.Count);
            return new SolidColorBrush(colorList[index]);
        }

        /// <summary>
        /// 依次从第一行放置到第三行
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private double GetHeight(int index)
        {
            return heightList[index % 3];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //计算三排的起始高度
            heightList = new List<double>();
            double height = canvas.ActualHeight;
            heightList.Add(0);
            //三分之一高
            double threeHeight = height / rowCount;
            for (int i = 1; i < rowCount; i++)
            {
                heightList.Add(threeHeight * i);
            }

            lengthList = new List<double>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                lengthList.Add(0);
            }
            locations = new List<int>(rowCount);

            isOver = true;
            locker = new object();

            //设置定时器
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);   //时间间隔
            timer.Tick += new EventHandler(ShowMessage);
            timer.Start();
        }
    }
}
