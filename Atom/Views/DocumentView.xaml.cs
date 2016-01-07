﻿using System;
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

namespace Atom.Views
{
    /// <summary>
    /// Логика взаимодействия для DocumentView.xaml
    /// </summary>
    public partial class DocumentView : Window
    {
        public DocumentView()
        {
            InitializeComponent();
        }

        private void Wizard_OnCancelled(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Wizard_OnFinished(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
