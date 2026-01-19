using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Models;

namespace TaskManager.Views
{
    public partial class OrdersView : UserControl
    {
        // === ПЕРЕМЕННЫЕ ===
        private List<string> _selectedStatuses = new List<string>() { "Создан", "в работе", "Завершен", "Отклонен" };

        // Переменные для дат
        private string _dateMode = "All"; // Режимы: All, Today, Yesterday, Month, LastMonth, Custom
        private DateTime? _customStart = null;
        private DateTime? _customEnd = null;

        public OrdersView()
        {
            InitializeComponent();
        }

        // === 1. ФИЛЬТР СТАТУСОВ (Твой код) ===
        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            if (box == null) return;
            string status = box.Tag.ToString();

            if (box.IsChecked == true)
            {
                if (!_selectedStatuses.Contains(status)) _selectedStatuses.Add(status);
            }
            else
            {
                if (_selectedStatuses.Contains(status)) _selectedStatuses.Remove(status);
            }
            ApplyFilter();
        }

        // === 2. ФИЛЬТР ДАТ: Быстрые чекбоксы ===
        private void OnQuickDateFilter(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            if (box.IsChecked == true)
            {
                _dateMode = box.Tag.ToString(); // Получаем режим (Today, LastMonth и т.д.)

                // Снимаем галочку "Весь период"
                if (FindName("AllPeriodCheck") is CheckBox allCheck) allCheck.IsChecked = false;

                // Очищаем календари визуально
                ClearCalendars();

                ApplyFilter();
            }
        }

        // === 3. ФИЛЬТР ДАТ: Весь период ===
        private void OnAllPeriodClick(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            if (box.IsChecked == true)
            {
                ResetToAll();
            }
            else
            {
                // Если сняли галочку, но ничего не выбрали - возвращаем её обратно (защита от пустоты)
                box.IsChecked = true;
            }
        }

        // === 4. ФИЛЬТР ДАТ: Кнопка Применить ===
        private void ApplyDateRange_Click(object sender, RoutedEventArgs e)
        {
            var dpFrom = FindName("DateFrom") as DatePicker;
            var dpTo = FindName("DateTo") as DatePicker;

            if (dpFrom.SelectedDate == null && dpTo.SelectedDate == null)
            {
                ResetToAll(); // Если даты пустые - это "Весь период"
                return;
            }

            _customStart = dpFrom.SelectedDate;
            _customEnd = dpTo.SelectedDate;
            _dateMode = "Custom";

            // Снимаем галочку "Весь период"
            if (FindName("AllPeriodCheck") is CheckBox allCheck) allCheck.IsChecked = false;

            // Запускаем фильтр
            ApplyFilter();
        }

        // === 5. ФИЛЬТР ДАТ: Кнопка Сброс ===
        private void ResetDateFilter(object sender, RoutedEventArgs e)
        {
            ResetToAll();
        }

        // Вспомогательный метод сброса
        private void ResetToAll()
        {
            _dateMode = "All";
            ClearCalendars();
            if (FindName("AllPeriodCheck") is CheckBox allCheck) allCheck.IsChecked = true;
            ApplyFilter();
        }

        private void ClearCalendars()
        {
            if (FindName("DateFrom") is DatePicker dp1) dp1.SelectedDate = null;
            if (FindName("DateTo") is DatePicker dp2) dp2.SelectedDate = null;
            _customStart = null;
            _customEnd = null;
        }

        // === ГЛАВНЫЙ МЕТОД ФИЛЬТРАЦИИ ===
        private void ApplyFilter()
        {
            if (OrdersGrid.ItemsSource == null) return;
            var view = CollectionViewSource.GetDefaultView(OrdersGrid.ItemsSource);
            if (view == null) return;

            view.Filter = item =>
            {
                var req = item as Request;
                if (req == null) return false;

                // 1. Проверка Статуса
                if (!_selectedStatuses.Contains(req.status_requestion)) return false;

                // 2. Проверка Даты
                if (_dateMode == "All") return true;

                DateTime d = req.date_create.Date;
                DateTime today = DateTime.Today;

                if (_dateMode == "Today") return d == today;
                if (_dateMode == "Yesterday") return d == today.AddDays(-1);
                if (_dateMode == "Month") return d.Month == today.Month && d.Year == today.Year;

                if (_dateMode == "LastMonth")
                {
                    var lastMonth = today.AddMonths(-1);
                    return d.Month == lastMonth.Month && d.Year == lastMonth.Year;
                }

                if (_dateMode == "Custom")
                {
                    bool afterStart = _customStart == null || d >= _customStart.Value;
                    bool beforeEnd = _customEnd == null || d <= _customEnd.Value;
                    return afterStart && beforeEnd;
                }

                return true;
            };
        }

        // === ТВОЙ ПОИСК (Сохранен) ===
        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
                // Сюда потом впишешь логику поиска
            }
        }
    }
}