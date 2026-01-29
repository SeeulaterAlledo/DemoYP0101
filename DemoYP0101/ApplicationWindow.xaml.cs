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
                var applicationsQuery = context.Applications
                    .Include("ListHousingStock")  // Для получения адреса
                    .Include("Executor")          // Для получения статуса
                    .Include("Users");            // Для получения пользователя

                if (showHistory)
                {
                    // Заявки со статусом "Выполнена" (ID = 2)
                    ApplicationsGrid.ItemsSource = applicationsQuery
                        .Where(a => a.ExecutorId == 2)
                        .OrderByDescending(a => a.Id)
                        .ToList();
                }
                else
                {
                    // Заявки со статусом "В процессе" (ID = 1)
                    ApplicationsGrid.ItemsSource = applicationsQuery
                        .Where(a => a.ExecutorId == 1)
                        .OrderByDescending(a => a.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
            // Создаем окно для добавления заявки
            var addWindow = new AddApplicationWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadApplications(); // Обновить список после создания
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
            this.Close();
        }
    }
}