using LTM.WeiXin.WPF.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BarrageDemo.BarrageParameters
{
    public class BarrageConfig
    {
        #region 相关数据
        /// <summary>
        /// 每行的起始高度
        /// </summary>
        public List<decimal> heightList;
        public List<Color> colorList;
        public Random random;
        /// <summary>
        /// 弹幕行数
        /// </summary>
        public int rowCount;
        /// <summary>
        /// 每次循环时字幕的显示（减小lengthList）的速度
        /// </summary>
        public decimal reduceSpeed;
        /// <summary>
        /// 单个弹幕的结束时间
        /// </summary>
        public double initFinishTime;
        /// <summary>
        /// 弹幕字体大小
        /// </summary>
        public double fontSize;
        /// <summary>
        /// 弹幕总高度
        /// </summary>
        public int height;
        #region 运行时
        /// <summary>
        /// 每行的弹幕未显示长度
        /// </summary>
        private List<decimal> lengthList;
        /// <summary>
        /// 当前可以加弹幕的位置（行数）集合
        /// </summary>
        private List<int> locations;
        /// <summary>
        /// 存储当前没有展现的弹幕
        /// </summary>
        private List<MessageInformation> messages;
        /// <summary>
        /// 是否是开发者模式
        /// </summary>
        private bool isDevelopMode;
        private Canvas canvas;
        #endregion
        #endregion

        #region 初始化
        public BarrageConfig(Canvas canvas)
        {
            InitializeColors();
            this.canvas = canvas;
            random = new Random();
            rowCount = int.Parse(ConfigurationManager.AppSettings["rowCount"]);
            reduceSpeed = decimal.Parse(ConfigurationManager.AppSettings["reduceSpeed"]);
            initFinishTime = double.Parse(ConfigurationManager.AppSettings["initFinishTime"]);
            fontSize = double.Parse(ConfigurationManager.AppSettings["fontSize"]);
            height = int.Parse(ConfigurationManager.AppSettings["height"]);
            
        }

        public void InitializeRuntimeParameters(Canvas canvas)
        {
            messages = new List<MessageInformation>();
            isDevelopMode = bool.Parse(ConfigurationManager.AppSettings["isDevelopMode"]);
            //计算三排的起始高度
            CaculateHeightList(canvas.ActualHeight);
            lengthList = new List<decimal>(rowCount);
            for (int i = 0; i < rowCount; i++)
            {
                lengthList.Add(0);
            }
            locations = new List<int>(rowCount);
        }

        private void CaculateHeightList(double canvasActualHeight)
        {
            heightList = new List<decimal>();
            decimal actualHeight = decimal.Parse(canvasActualHeight.ToString());
            heightList.Add(0);
            //rowCount分之一高
            decimal threeHeight = actualHeight / rowCount;
            for (int i = 1; i < rowCount; i++)
            {
                heightList.Add(threeHeight * i);
            }
        }

        /// <summary>
        /// 初始化弹幕颜色
        /// </summary>
        private void InitializeColors()
        {
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
        }

        #endregion

        #region 运行时
        /// <summary>
        /// 按时间减少每行的未显示长度
        /// </summary>
        public void ReduceLengthList(decimal reduceSpeed)
        {
            for (int i = 0; i < lengthList.Count; i++)
            {
                if (lengthList[i] > 0)
                {
                    //这里根据动画的速度调节，每个计时器的间隔走了X个字
                    lengthList[i] -= reduceSpeed;
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
        public async Task GetMessages()
        {
            //防止网络错误，直接调用本次调用
            try
            {
                //todo 这里调用自己的服务器的弹幕接口
                //建议不要一次性把弹幕从服务器都读过来，客户端超过X条未显示弹幕的时候，跳过读取接口的步骤，静待弹幕刷去一些再读取接口数据
                //get请求调用服务接口
                //string apiUrl = ConfigurationManager.AppSettings["apiUrl"];
                //HttpClient client = new HttpClient();
                //var result = await client.GetAsync(apiUrl);
                //string content = await result.Content.ReadAsStringAsync();
                ////最新的弹幕
                //GetMessageInformationResult newMessages = JsonConvert.DeserializeObject<GetMessageInformationResult>(content);
                GetMessageInformationResult newMessages = null;
                if (newMessages == null)
                {
                    newMessages = new GetMessageInformationResult();
                }
                if (newMessages.result == null)
                {
                    newMessages.result = new List<MessageInformation>();
                }
                //测试用例
                if (isDevelopMode)
                {
                    newMessages.result.Add(new MessageInformation() { nickName = "Lulus", content = "你们的你们的你们的你们的你们的" });
                    newMessages.result.Add(new MessageInformation() { nickName = "无尽的梦", content = "生活" });
                    newMessages.result.Add(new MessageInformation() { content = "真丰富真丰富真丰富真丰富真丰富真丰富" });
                    newMessages.result.Add(new MessageInformation() { nickName = "SevenS", content = "不像我不像我不像我不像我" });
                    newMessages.result.Add(new MessageInformation() { content = "帅" });
                    newMessages.result.Add(new MessageInformation() { nickName = "钢铁战士", content = "字" });
                    newMessages.result.Add(new MessageInformation() { content = "表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了" });
                    newMessages.result.Add(new MessageInformation() { content = "一生" });
                    newMessages.result.Add(new MessageInformation() { nickName = "勿忘心安. گق", content = "你们的你们的你们的你们的你们的" });
                    newMessages.result.Add(new MessageInformation() { content = "生活" });
                    newMessages.result.Add(new MessageInformation() { nickName = "Garrie", content = "真丰富真丰富真丰富真丰富真丰富真丰富" });
                    newMessages.result.Add(new MessageInformation() { content = "不像我不像我不像我不像我" });
                    newMessages.result.Add(new MessageInformation() { content = "帅" });
                    newMessages.result.Add(new MessageInformation() { nickName = "远轩微", content = "字" });
                    newMessages.result.Add(new MessageInformation() { content = "表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了" });
                    newMessages.result.Add(new MessageInformation() { content = "一生" });
                    newMessages.result.Add(new MessageInformation() { content = "你们的你们的你们的你们的你们的" });
                    newMessages.result.Add(new MessageInformation() { content = "生活" });
                    newMessages.result.Add(new MessageInformation() { content = "真丰富真丰富真丰富真丰富真丰富真丰富" });
                    newMessages.result.Add(new MessageInformation() { content = "不像我不像我不像我不像我" });
                    newMessages.result.Add(new MessageInformation() { content = "帅" });
                    newMessages.result.Add(new MessageInformation() { content = "字" });
                    newMessages.result.Add(new MessageInformation() { content = "表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了" });
                    newMessages.result.Add(new MessageInformation() { content = "一生" });
                    newMessages.result.Add(new MessageInformation() { content = "你们的你们的你们的你们的你们的" });
                    newMessages.result.Add(new MessageInformation() { content = "生活" });
                    newMessages.result.Add(new MessageInformation() { content = "真丰富真丰富真丰富真丰富真丰富真丰富" });
                    newMessages.result.Add(new MessageInformation() { content = "不像我不像我不像我不像我" });
                    newMessages.result.Add(new MessageInformation() { content = "帅" });
                    newMessages.result.Add(new MessageInformation() { content = "字" });
                    newMessages.result.Add(new MessageInformation() { content = "表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了表现了" });
                    newMessages.result.Add(new MessageInformation() { content = "一生" });
                }


                messages.AddRange(newMessages.result);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 获得最新的三个弹幕
        /// </summary>
        /// <returns></returns>
        public List<MessageInformation> GetTopThreeMessages()
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
                //设置完成动画的时间
                //实例化TextBlock和设置基本属性,并添加到Canvas中
                TextBlock textblock = new TextBlock();
                //加上昵称显示
                if (!string.IsNullOrEmpty(contentlist[i].nickName))
                {
                    textblock.Text = $"{contentlist[i].nickName}:{contentlist[i].content}";
                }
                else
                {
                    textblock.Text = contentlist[i].content;
                }
                textblock.FontSize = fontSize;
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
                animation.Duration = TimeSpan.FromSeconds(initFinishTime);
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
            decimal target = heightList[index % rowCount];
            return double.Parse(target.ToString());
        }
        #endregion
    }
}
