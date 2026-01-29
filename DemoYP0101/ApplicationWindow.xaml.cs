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
    /// Логика взаимодействия для ApplicationWindow.xaml
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        private Entities context;
        private bool showHistory = false;
        private string addressFilter = "";
        private string applicantFilter = "";

        public ApplicationWindow()
        {
            InitializeComponent();
            context = Entities.Getcontext();
            LoadApplications();
        }

        private void LoadApplications()
        {
            try
            {
                // Получаем базовый запрос
                IQueryable<Applications> applicationsQuery = context.Applications
                    .Include("ListHousingStock")
                    .Include("Executor")
                    .Include("Users");

                // Фильтрация по статусу (текущие/история)
                if (showHistory)
                {
                    applicationsQuery = applicationsQuery
                        .Where(a => a.ExecutorId == 2);
                }
                else
                {
                    applicationsQuery = applicationsQuery
                        .Where(a => a.ExecutorId == 1);
                }

                // Применяем фильтр по адресу (регистронезависимый поиск)
                if (!string.IsNullOrWhiteSpace(addressFilter))
                {
                    applicationsQuery = applicationsQuery
                        .Where(a => a.ListHousingStock.Adress.ToLower()
                                    .Contains(addressFilter.ToLower()));
                }

                // Применяем фильтр по заявителю (регистронезависимый поиск)
                if (!string.IsNullOrWhiteSpace(applicantFilter))
                {
                    applicationsQuery = applicationsQuery
                        .Where(a => a.Users.Name.ToLower()
                                    .Contains(applicantFilter.ToLower()));
                }

                // Сортируем и загружаем данные
                ApplicationsGrid.ItemsSource = applicationsQuery
                    .OrderByDescending(a => a.Id)
                    .ToList();

                // Обновляем статусную строку (опционально)
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatusBar()
        {
            int count = (ApplicationsGrid.ItemsSource as System.Collections.IList)?.Count ?? 0;

            // Если нужно добавить статусную строку, можно использовать TextBlock в XAML
            // StatusTextBlock.Text = $"Найдено заявок: {count}";
        }

        // Обработчик изменения текста в полях фильтрации
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем фильтры
            addressFilter = AddressFilterTextBox.Text?.Trim() ?? "";
            applicantFilter = ApplicantFilterTextBox.Text?.Trim() ?? "";

            // Перезагружаем данные с фильтрами
            LoadApplications();
        }

        // Кнопка очистки фильтров
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            AddressFilterTextBox.Text = "";
            ApplicantFilterTextBox.Text = "";

            // Сброс фильтров и загрузка данных
            addressFilter = "";
            applicantFilter = "";
            LoadApplications();
        }

        // Остальные методы остаются без изменений
        private void CurrentButton_Click(object sender, RoutedEventArgs e)
        {
            showHistory = false;
            LoadApplications();
            CurrentButton.IsEnabled = false;
            HistoryButton.IsEnabled = true;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            showHistory = true;
            LoadApplications();
            CurrentButton.IsEnabled = true;
            HistoryButton.IsEnabled = false;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddApplicationWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadApplications();
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = ApplicationsGrid.SelectedItem as Applications;
            if (selectedApplication != null)
            {
                try
                {
                    selectedApplication.ExecutorId = 2;
                    context.SaveChanges();
                    LoadApplications();
                    MessageBox.Show("Заявка переведена в выполненные", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления статуса: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку для выполнения", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new MenuWindow().Show();
            Close();
        }
    }
}