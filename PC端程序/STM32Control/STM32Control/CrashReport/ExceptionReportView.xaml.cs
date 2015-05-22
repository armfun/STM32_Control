using STM32Control.CrashReport;
using STM32Control.CrashReport.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// ExceptionReportView.xaml 的交互逻辑
    /// </summary>
    public partial class ExceptionReportView : Window
    {
        ExceptionReportInfo reportInfo = new ExceptionReportInfo();
        ExceptionReportPresenter presenter;
        public ExceptionReportView(params Exception[] e)
        {
            InitializeComponent();
            reportInfo.SetExceptions(e);   
            //ShowFullDetail();
            ShowLastReportInfot(reportInfo);
            presenter = new ExceptionReportPresenter(reportInfo);
        }

        private  void  btnDetail_Click(object sender, RoutedEventArgs e)
        {
            ShowFullDetail();
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            reportInfo.Description = tb_Describe.Text;
            if (CbkPersonalInfo.IsChecked == true)
            {
                reportInfo.IsSendPersonalInfo = true;
            }
            else
            {
                reportInfo.IsSendPersonalInfo = false;
            }
            presenter.SendReportByEmail();
            this.Close();
        }

        private void ShowFullDetail()
        {
            if (tb_Error.Visibility.Equals(Visibility.Collapsed))
            {
                this.Height = 400;
                HideRow.Height = new GridLength(130);
                tb_Error.Visibility = Visibility.Visible;
                btnDetail.Template = (ControlTemplate)App.Current.Resources["btnDetail2"];
            }
            else
            {
                tb_Error.Visibility = Visibility.Collapsed;
                this.Height = 250;
                HideRow.Height = new GridLength(0);
                btnDetail.Template = (ControlTemplate)App.Current.Resources["btnDetail"];

            }
        }

        private void ShowLastReportInfot(ExceptionReportInfo reportInfo)
        {
            ShowFullDetail();
            tb_Error.Text = reportInfo.MainException.Message + "\r\n\r\n" + reportInfo.Exceptions[0].StackTrace.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            presenter.ScreenshotFileDelte();
            this.Close();
        }
    }
}
