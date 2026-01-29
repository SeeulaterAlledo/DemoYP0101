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
    /// Логика взаимодействия для AddApplicationWindow.xaml
    /// </summary>
    public partial class AddApplicationWindow : Window
    {
        private Entities context;

        public AddApplicationWindow()
        {
            InitializeComponent();
            context = Entities.Getcontext();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Загрузка адресов
                AddressComboBox.ItemsSource = context.ListHousingStock
                    .OrderBy(a => a.Adress)
                    .ToList();
                AddressComboBox.DisplayMemberPath = "Adress";
                AddressComboBox.SelectedValuePath = "Id";

                // Загрузка заявителей
                UserComboBox.ItemsSource = context.Users
                    .OrderBy(u => u.Name)
                    .ToList();
                UserComboBox.DisplayMemberPath = "Name";
                UserComboBox.SelectedValuePath = "Id";

                // Загрузка статусов
                StatusComboBox.ItemsSource = context.Executor
                    .OrderBy(e => e.Id)
                    .ToList();
                StatusComboBox.DisplayMemberPath = "Name";
                StatusComboBox.SelectedValuePath = "Id";

                // Установка значений по умолчанию
                if (StatusComboBox.Items.Count > 0)
                    StatusComboBox.SelectedIndex = 0;

                if (UserComboBox.Items.Count > 0)
                    UserComboBox.SelectedIndex = 0;

                if (AddressComboBox.Items.Count > 0)
                    AddressComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (AddressComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите адрес", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                AddressComboBox.Focus();
                return;
            }

            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявителя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UserComboBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Введите телефон", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                PhoneTextBox.Focus();
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

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                StatusComboBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
            {
                MessageBox.Show("Введите описание проблемы", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                DescriptionTextBox.Focus();
                return;
            }

            try
            {
                // РЕШЕНИЕ 1: Создаем новый контекст для избежания проблем с отслеживанием
                using (var localContext = new Entities())
                {
                    // Создание новой заявки
                    var newApplication = new Applications
                    {
                        AdressId = (int)AddressComboBox.SelectedValue,
                        UserId = (int)UserComboBox.SelectedValue,
                        Phone = PhoneTextBox.Text.Trim(),
                        Description = DescriptionTextBox.Text.Trim(),
                        ExecutorId = (int)StatusComboBox.SelectedValue
                    };

                    // РЕШЕНИЕ 2: Находим максимальный ID и устанавливаем следующий
                    if (!localContext.Applications.Any())
                    {
                        // Если таблица пуста, начинаем с 1
                        // Entity Framework должен сам генерировать ID, но если есть проблемы:
                        // Можно попробовать не устанавливать Id вообще
                    }
                    else
                    {
                        // Если нужно вручную установить Id (только если автоинкремент не работает)
                        // int maxId = localContext.Applications.Max(a => a.Id);
                        // newApplication.Id = maxId + 1;
                    }

                    localContext.Applications.Add(newApplication);
                    localContext.SaveChanges();
                }

                MessageBox.Show("Заявка успешно создана!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                // ИСПРАВЛЕНО: Отображаем внутреннее исключение
                string errorMessage = $"Ошибка сохранения заявки:\n{ex.Message}";

                // Добавляем все внутренние исключения
                Exception innerEx = ex.InnerException;
                int level = 1;
                while (innerEx != null)
                {
                    errorMessage += $"\n\nВнутренняя ошибка (уровень {level}):\n{innerEx.Message}";
                    innerEx = innerEx.InnerException;
                    level++;
                }

                // Для отладки можно добавить StackTrace
#if DEBUG
                errorMessage += $"\n\nStackTrace:\n{ex.StackTrace}";
#endif

                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}