﻿using BestAutoClicker.Commands;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BestAutoClicker.Views
{
    /// <summary>
    /// Interaction logic for KeyClicker.xaml
    /// </summary>
    public partial class KeyClicker : Page
    {
        public KeyClicker()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Test");
        }
    }
}
