﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace InterviewManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            Blur.Radius = 10;
            new RegisterLoginWindow().ShowDialog();
            Blur.Radius = 0;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Blur.Radius = 10;
            if (new RegisterLoginWindow() { IsRegistration = false }.ShowDialog().Value)
            {
                Hide();
            }
            Blur.Radius = 0;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }
    }
}
