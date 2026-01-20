using Demo2modl.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoYP0101
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Entities _db = new Entities();
        private MassageHelper _mh = new MassageHelper();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void LoginButton_click(object sender, RoutedEventArgs e)
        {
            string login = LoginEnter.Text;
            string password = PasswordEnter.Password;

            var user = _db.Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();

            if (user == null)
            {
                _mh.ShowError("Введен не правильный логин или пароль!");
                return;
            }
            else
            {
                CurrentSession.CurrentUser = user;
                new ReportWindow(user).Show();
                Close();
            }
        }

        private void TextBloxk_MouseDown(object sender, RoutedEventArgs e)
        {
            new ReportWindow().Show();
            Close();
        }


    }
}