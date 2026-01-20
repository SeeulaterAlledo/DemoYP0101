using Demo2modl.Statics;
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

namespace DemoYP0101
{
    /// <summary>
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private Entities _db = new Entities();
        public ReportWindow()
        {
            InitializeComponent();
            LoadReport();
        }
        public ReportWindow(Users user)
        {
            InitializeComponent();
            LoadReport();
        }

        private void LoadReport()
        {

            ReportList.ItemsSource = _db.PaymentReport.ToList();
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

    }
}
