using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DSTServerManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs exception)
        {
            string errorCaption = "意外的操作";
            string errorInfoTip = "我们很抱歉,当前应用程序遇到一些问题..\r\n";
            MessageBox.Show(errorInfoTip + exception.Exception, errorCaption, MessageBoxButton.OK, MessageBoxImage.Information);
            exception.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs exception)
        {
            string errorCaption = "意外的操作";
            string errorInfoTip = "我们很抱歉,当前应用程序遇到一些问题,请联系管理员..\r\n";
            MessageBox.Show(errorInfoTip + exception.ExceptionObject, errorCaption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {

        }
    }
}
