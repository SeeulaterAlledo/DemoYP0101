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
    /// Логика взаимодействия для AddEditArrearsWindow.xaml
    /// </summary>
    public partial class AddEditArrearsWindow : Window
    {
        private Entities _db = new Entities();
        private Arrears _arrearToEdit = null;

        public AddEditArrearsWindow()
        {
            InitializeComponent();
            LoadComboBoxData();
            Title = "Добавление задолженности";
        }

        public AddEditArrearsWindow(Arrears arrearToEdit)
        {
            InitializeComponent();
            _arrearToEdit = arrearToEdit;
            LoadComboBoxData();
            LoadArrearData();
            Title = "Редактирование задолженности";
        }

        private void LoadComboBoxData()
        {
            try
            {
                
                AddressComboBox.ItemsSource = _db.ListHousingStock.ToList();

                
                UserComboBox.ItemsSource = _db.Users.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadArrearData()
        {
            if (_arrearToEdit != null)
            {
                AddressComboBox.SelectedValue = _arrearToEdit.AdressId;
                FlatTextBox.Text = _arrearToEdit.Flat;
                UserComboBox.SelectedValue = _arrearToEdit.UserId;
                PhoneTextBox.Text = _arrearToEdit.Phone;
                WaterTextBox.Text = _arrearToEdit.Water.ToString();
                ElectricPowerTextBox.Text = _arrearToEdit.ElectricPower.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                if (AddressComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите адрес");
                    return;
                }

                if (string.IsNullOrWhiteSpace(FlatTextBox.Text))
                {
                    MessageBox.Show("Введите номер квартиры");
                    return;
                }

                if (UserComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Выберите владельца");
                    return;
                }

                decimal water, electricPower;
                if (!decimal.TryParse(WaterTextBox.Text, out water) ||
                    !decimal.TryParse(ElectricPowerTextBox.Text, out electricPower))
                {
                    MessageBox.Show("Введите корректные числовые значения для задолженностей");
                    return;
                }
                if (!PhoneTextBox.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Номер телефона должен содержать только цифры", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    PhoneTextBox.Focus();
                    PhoneTextBox.SelectAll();
                    return;
                }

                // Дополнительная проверка длины (пример)
                if (PhoneTextBox.Text.Length < 10)
                {
                    MessageBox.Show("Номер телефона должен содержать не менее 10 цифр", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    PhoneTextBox.Focus();
                    PhoneTextBox.SelectAll();
                    return;
                }
                if (_arrearToEdit == null)
                {
                    
                    var newArrear = new Arrears
                    {
                        AdressId = (int)AddressComboBox.SelectedValue,
                        Flat = FlatTextBox.Text,
                        UserId = (int)UserComboBox.SelectedValue,
                        Phone = PhoneTextBox.Text,
                        Water = water,
                        ElectricPower = electricPower
                    };

                    _db.Arrears.Add(newArrear);
                }
                else
                {
                    
                    var arrear = _db.Arrears.Find(_arrearToEdit.Id);
                    if (arrear != null)
                    {
                        arrear.AdressId = (int)AddressComboBox.SelectedValue;
                        arrear.Flat = FlatTextBox.Text;
                        arrear.UserId = (int)UserComboBox.SelectedValue;
                        arrear.Phone = PhoneTextBox.Text;
                        arrear.Water = water;
                        arrear.ElectricPower = electricPower;
                    }
                }

                _db.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}