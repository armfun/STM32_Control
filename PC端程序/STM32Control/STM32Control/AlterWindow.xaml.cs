using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace STM32Control
{
    public partial class AlterWindow : Window
    {
        #region 修改硬件信息
        public AlterWindow(string id,string pwd,string ip,string port)
        {
            InitializeComponent();
            this.tb_ID.Text = id;
            this.tb_PWD.Text = pwd;
            this.tb_IP.Text = ip;
            this.tb_Port.Text = port;
        }

        public string ID { set; get; }
        public string PWD { set; get; }
        public string IP { set; get; }
        public string Port { set; get; }
        public bool IsNeedAlter { set; get; }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.IsNeedAlter = false;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.IsNeedAlter = false;
            this.DragMove();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (CheckHelper.IPCheck(this.tb_IP.Text.Trim()) && CheckHelper.PortCheck(this.tb_Port.Text.Trim()) && !string.IsNullOrEmpty(this.tb_ID.Text) && !string.IsNullOrEmpty(this.tb_PWD.Text))
            {
                this.IsNeedAlter = true;
                this.ID = this.tb_ID.Text.ToString();
                this.PWD = this.tb_PWD.Text.ToString();
                this.IP = this.tb_IP.Text.ToString();
                this.Port = this.tb_Port.Text.ToString();
                this.Close();
            }
            else
            {
                if (CheckHelper.IPCheck(this.tb_IP.Text.Trim()))
                {
                    this.tb_WarningIP.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.tb_WarningIP.Visibility = Visibility.Visible;
                }
                if (CheckHelper.PortCheck(this.tb_Port.Text.Trim()))
                {
                    this.tb_WarningPort.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.tb_WarningPort.Visibility = Visibility.Visible;
                }
                if (!string.IsNullOrEmpty(this.tb_ID.Text))
                {
                    this.tb_WarningID.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.tb_WarningID.Visibility = Visibility.Visible;
                }
                if (!string.IsNullOrEmpty(this.tb_PWD.Text))
                {
                    this.tb_WarningPWD.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.tb_WarningPWD.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion
    }
}
