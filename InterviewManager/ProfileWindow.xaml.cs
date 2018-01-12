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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InterviewManager
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public string ProfileIn { get; set; }
        public string ProfileOut { get; set; }
        public bool IsUser { get; set; } = true;

        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void Init()
        {
            Text.Text = ProfileIn;
            if(!IsUser)
            {
                Text.IsReadOnly = true;
                Cancel.Visibility = Visibility.Hidden;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            ProfileOut = Text.Text;
            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }
    }
}
