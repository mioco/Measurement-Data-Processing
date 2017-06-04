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


namespace MDP
{
    public partial class MainWindow : Window
    {
        private void LB01(object sender, RoutedEventArgs e)
        {
            LB01 win = new LB01();
            win.Show();
        }

        private void LB02(object sender, RoutedEventArgs e)
        {
            LB02 win = new LB02();
            win.Show();
        }

        private void LB03(object sender, RoutedEventArgs e)
        {
            LB03 win = new LB03();
            win.Show();
        }
    }
}
