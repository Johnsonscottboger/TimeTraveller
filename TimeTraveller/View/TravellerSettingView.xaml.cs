using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeTraveller.View
{
    /// <summary>
    /// TravellerSettingView.xaml 的交互逻辑
    /// </summary>
    public partial class TravellerSettingView : UserControl
    {
        public TravellerSettingView()
        {
            InitializeComponent();
        }

        private void GitHubButton_OnClick(Object sender,RoutedEventArgs e)
        {
            Process.Start(ConfigurationManager.AppSettings["GitHub"]);
        }
    }
}
