using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using TaskManager.Core;
using TaskManager.Data;
using System.Windows.Media;

namespace TaskManager.ViewModels
{
    public class AnalyticsViewModel : ObservableObject
    {
        // Поля для хранения (нужны для OnPropertyChanged)
        private SeriesCollection _ratioSeriesCollection;
        private SeriesCollection _createdRequestsSeries;
        private SeriesCollection _completedRequestsSeries;
        private List<string> _timeLabels;

        // Свойства с уведомлением UI
        public SeriesCollection RatioSeriesCollection
        {
            get => _ratioSeriesCollection;
            set { _ratioSeriesCollection = value; OnPropertyChanged(); }
        }

        public SeriesCollection CreatedRequestsSeries
        {
            get => _createdRequestsSeries;
            set { _createdRequestsSeries = value; OnPropertyChanged(); }
        }

        public SeriesCollection CompletedRequestsSeries
        {
            get => _completedRequestsSeries;
            set { _completedRequestsSeries = value; OnPropertyChanged(); }
        }

        public List<string> TimeLabels
        {
            get => _timeLabels;
            set { _timeLabels = value; OnPropertyChanged(); }
        }

        public AnalyticsViewModel()
        {
            LoadAnalyticsData();
        }

        private void LoadAnalyticsData()
        {
            using (var db = new AppDbContext())
            {
                // Загружаем в память, чтобы избежать проблем с трансляцией дат в SQL
                var allRequests = db.Zapros.ToList();

                // 1. КРУГОВАЯ ДИАГРАММА
                int totalCreated = allRequests.Count;
                int totalClosed = allRequests.Count(r => r.StatusRequest == "Завершен" || r.DateComplete.HasValue);

                RatioSeriesCollection = new SeriesCollection
                {
                    new PieSeries
                    {
                        Title = "Создано",
                        Values = new ChartValues<int> { totalCreated },
                        DataLabels = true
                    },
                    new PieSeries
                    {
                        Title = "Закрыто",
                        Values = new ChartValues<int> { totalClosed },
                        DataLabels = true
                    }
                };

                // 2. ГРАФИКИ (Последние 7 дней)
                // Используем UtcNow.Date, так как при создании ты юзаешь UtcNow
                var todayUtc = DateTime.UtcNow.Date;
                var lastWeek = Enumerable.Range(0, 7)
                    .Select(i => todayUtc.AddDays(-i))
                    .OrderBy(d => d)
                    .ToList();

                TimeLabels = lastWeek.Select(d => d.ToString("dd.MM")).ToList();

                var createdCounts = new ChartValues<int>();
                var completedCounts = new ChartValues<int>();

                foreach (var date in lastWeek)
                {
                    // Считаем заявки за этот день (сравниваем только Date)
                    createdCounts.Add(allRequests.Count(r => r.DateCreate.Date == date.Date));
                    completedCounts.Add(allRequests.Count(r => r.DateComplete.HasValue && r.DateComplete.Value.Date == date.Date));
                }

                CreatedRequestsSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Создано",
                        Values = createdCounts,
                        PointGeometrySize = 10
                    }
                };

                CompletedRequestsSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Закрыто",
                        Values = completedCounts,
                        Stroke = Brushes.Green,
                        PointGeometrySize = 10
                    }
                };
            }
        }
    }
}