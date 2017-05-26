using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace lb01
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 文件读取
        private void readData(object sender, RoutedEventArgs e)
        {
            Window1 readFileWindow = new Window1();
            readFileWindow.Show();
        }
        #endregion

        #region 高程计算
        private void elevation(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 水准网平差
        private void levellingNetwork(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 精度计算
        private void precition(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 保存数据
        private void saveData(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 退出
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }
}
