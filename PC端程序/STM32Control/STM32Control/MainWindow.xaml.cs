using STM32Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace STM32Control
{
    public partial class MainWindow : Window
    {
        Proxy proxy;
        Client client;
        bool isConnected = false;
        bool hasIRCode = false;
        System.Windows.Media.Animation.Storyboard sb;
        public MainWindow()
        {
            InitializeComponent();
            sb = (System.Windows.Media.Animation.Storyboard)this.Resources["spread"];
            sb.Completed += (s, e) =>
            {
                sb = (System.Windows.Media.Animation.Storyboard)this.Resources["shrink"];
                sb.Completed += (sender, Event) => Application.Current.Shutdown();
            };
            if (sb != null)
            {
                sb.Begin();
            }
            LoadResource();
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            proxy = new Proxy();
            proxy.onConnect = new OnConnect(onConnect);
            proxy.onData = new OnData(onData);
            proxy.onDisconnect = new OnDisconnect(onDisconnect);
        }

        private void LoadResource()
        {
            tb_ConstID.Text = App.Current.TryFindResource("UI_DeviceID").ToString();
            tb_ConstIP.Text = App.Current.TryFindResource("UI_DeviceID").ToString();
            tb_ConstPort.Text = App.Current.TryFindResource("UI_DevicePort").ToString();
            tb_ConstPWD.Text = App.Current.TryFindResource("UI_DevicePassword").ToString();
            btn_Connect.Content = App.Current.TryFindResource("UI_Connect").ToString();
            btn_Alert.Content = App.Current.TryFindResource("UI_Modify").ToString();

            ((TabItem)(tab_Functions.Items[0])).Header = App.Current.TryFindResource("UI_Digit").ToString();
            ((TabItem)(tab_Functions.Items[1])).Header = App.Current.TryFindResource("UI_Analog").ToString();
            ((TabItem)(tab_Functions.Items[2])).Header = App.Current.TryFindResource("UI_Impulse").ToString();
            ((TabItem)(tab_Functions.Items[3])).Header = App.Current.TryFindResource("UI_Humiture").ToString();
            ((TabItem)(tab_Functions.Items[4])).Header = App.Current.TryFindResource("UI_IR").ToString();

            btn_GetDigit.Content = App.Current.TryFindResource("UI_GetDigit").ToString();
            btn_GetAnalog.Content = App.Current.TryFindResource("UI_GetAnalog").ToString();
        }

        #region 代理委托事件:OnConnect,OnData,OnDisconnect
        void onConnect(bool result)
        {
            isConnected = result;
            if (result)
            {
                LstInfo.Items.Insert(0, "已连接");
            }
        }

        void onData(byte[] data)
        {
            STM32Core.Decoder decoder = new STM32Core.Decoder();
            decoder.InitData(data);
            Operations? operation = decoder.GetOperation();
            if (operation.HasValue)
            {
                object dt = decoder.Decode(operation.Value);
                if (operation == Operations.GetDHT1x)
                {
                    if (dt.ToString().Contains("NotFind"))
                    {
                        LstInfo.Items.Insert(0, "未检测到DHT11");
                        return;
                    }
                    else if (dt.ToString().Contains("Wrong"))
                    {
                        LstInfo.Items.Insert(0, "读取错误");
                        return;
                    }
                    else if (dt.ToString().Contains("Miss"))
                    {
                        LstInfo.Items.Insert(0, "未知错误");
                        return;
                    }
                    Humiture humiture = dt as Humiture;
                    if (tb_Humidity.Text.Length != 0 || tb_Temperature.Text.Length != 0)
                    {
                        LstHumiture.Items.Insert(1, DateTime.Now.ToLongTimeString()+ ":温度" + tb_Temperature.Text + "      湿度" + tb_Humidity.Text);
                    }
                    tb_Temperature.Text = humiture.Temperature.ToString("n2") + "C";
                    tb_Humidity.Text = humiture.Humidity.ToString("n2") + "%";
                }
                else if (operation == Operations.SetDigital || operation == Operations.SetPWM)
                {
                    if (dt.ToString().Contains("OK"))
                    {
                        LstInfo.Items.Insert(0, "设置成功");
                    }
                    else
                    {
                        LstInfo.Items.Insert(0, dt.ToString());
                    }
                }
                else if (operation == Operations.GetDigitalAndAnalog)
                {
                    GPIOs gpios = client.Mapper.ToObject<GPIOs>(dt.ToString());
                    tb_IO_01.IsChecked = gpios.Digitals[0] == 0 ? false : true;
                    tb_IO_02.IsChecked = gpios.Digitals[1] == 0 ? false : true;
                    tb_IO_03.IsChecked = gpios.Digitals[2] == 0 ? false : true;
                    tb_IO_04.IsChecked = gpios.Digitals[3] == 0 ? false : true;
                    tb_IO_05.IsChecked = gpios.Digitals[4] == 0 ? false : true;
                    tb_IO_06.IsChecked = gpios.Digitals[5] == 0 ? false : true;
                    tb_IO_07.IsChecked = gpios.Digitals[6] == 0 ? false : true;
                    tb_IO_08.IsChecked = gpios.Digitals[7] == 0 ? false : true;
                    tb_A1.Text = gpios.Analogs[0].ToString();
                    tb_A2.Text = gpios.Analogs[1].ToString();
                    tb_A3.Text = gpios.Analogs[2].ToString();
                    tb_A4.Text = gpios.Analogs[3].ToString();
                    tb_A5.Text = gpios.Analogs[4].ToString();
                    tb_A6.Text = gpios.Analogs[5].ToString();
                    tb_A7.Text = gpios.Analogs[6].ToString();
                }
                else if (operation == Operations.IRLearn)
                {
                    if (dt.ToString().Contains("Learned"))
                    {
                        hasIRCode = true;
                        LstIR.Items.Insert(1, "学习完成!" + "     地址码:" + dt.ToString().Substring(7, 3) + "       数据码:" + dt.ToString().Substring(10, 3));
                    }
                    else if (dt.ToString().Contains("Failed")) 
                    {
                        hasIRCode = false;
                        LstIR.Items.Insert(1, "学习失败!");
                    }
                    else
                    {
                        hasIRCode = false;
                        LstIR.Items.Insert(1, dt.ToString());
                    }
                }
                else if (operation == Operations.IRSend)
                {
                    if (dt.ToString().Contains("Sent"))
                    {
                        LstIR.Items.Insert(1, "发射成功!");
                    }
                    else
                    {
                        LstIR.Items.Insert(1,"发射失败!");
                    }
                }
            }
        }

        void onDisconnect(string message)
        {
            isConnected = false;
            LstInfo.Items.Insert(0, message);
        }
        #endregion
        #region 功能函数
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (proxy != null)
            {
                client = new Client(tb_PWD.Text.ToString());
                proxy.Connect(tb_IP.Text.ToString(), Convert.ToInt32(tb_Port.Text), 200);
            }
        }
        /// <summary>
        /// 获取温度湿度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGetHumiture_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                proxy.ToSTM32(client.GetDHT1x());
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }

        #region 数字端口状态切换
        private void tb_IO_Checked(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = (sender as ToggleButton).Tag.ToString()+"ON";
        }

        private void tb_IO_Unchecked(object sender, RoutedEventArgs e)
        {
            (sender as ToggleButton).Content = (sender as ToggleButton).Tag.ToString()+"OFF";
        }
        #endregion
        /// <summary>
        /// 设置数字端口状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSetDigital_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                var tb = sender as ToggleButton;
                var pin = int.Parse(tb.Name.ToString().Substring(7, 1));
                proxy.ToSTM32(client.SetDigitalLevel(pin, tb.IsChecked.Value ? DigitalLevel.HIGH : DigitalLevel.LOW));
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }

        /// <summary>
        /// 获取数字端口和模拟端口状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGetDigitalAndAnalog_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                proxy.ToSTM32(client.GetDigitalAndAnalog());
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }

        /// <summary>
        /// 脉冲宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SdValueChanged_Click(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isConnected)
            {
                var sd = sender as Slider;
                var pin = int.Parse(sd.Name.ToString().Substring(8, 1));
                proxy.ToSTM32(client.SetPWM(pin, (byte)sd.Value));
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }

        /// <summary>
        /// 红外学习
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIRLearn_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                proxy.ToSTM32(client.IRLearn());
                LstIR.Items.Insert(1, "正在学习...");
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }

        /// <summary>
        ///红外发射
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIRSend_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                if (hasIRCode)
                {
                    proxy.ToSTM32(client.IRSend());
                }
                else
                    LstIR.Items.Insert(1, "没有可发射的地址码和数据码");
            }
            else
                LstInfo.Items.Insert(0, "请连接到设备");
        }
        #endregion
        #region 辅助功能
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this.Close();
            //this.Hide();
            if (sb != null)
            {
                sb.Begin();
            }
            //Application.Current.Shutdown();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {

            base.OnClosing(e);
            e.Cancel = true;
            btnClose_Click(null, null);
        }

        private void btnMinsize_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowState = WindowState.Minimized;
            this.Hide();
        }

        private void BtnAlert_Click(object sender, RoutedEventArgs e)
        {
            AlterWindow aw = new AlterWindow(this.tb_ID.Text,this.tb_PWD.Text,this.tb_IP.Text,this.tb_Port.Text);
            aw.Owner = this;
            aw.ShowDialog();
            if (aw.IsNeedAlter)
            {
                this.tb_ID.Text = aw.ID;
                this.tb_PWD.Text = aw.PWD;
                this.tb_IP.Text = aw.IP;
                this.tb_Port.Text = aw.Port;
            }
        }
        #endregion
    }
}
