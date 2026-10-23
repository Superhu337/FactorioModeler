using FactorioModeler.Models;
using FactorioModeler.Scripts;
using FactorioModeler.Services;
using FactorioModeler.ViewModels;
using FactorioModeler.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FactorioModeler
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Регистрируем все сервисы в одном месте
            var services = new ServiceCollection();

            services.AddSingleton<IDataService, DataCollectorService>();
            services.AddSingleton<IWindowManager, WindowManagerService>();
            services.AddSingleton<IEventBus, EventBusService>();
            services.AddSingleton<ICardDataExtractor, CardDataExtractorService>();

            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<SearchPopUp>();
            services.AddTransient<SearchPopUpViewModel>();
            // [НОВЫЕ ОКНА ЗДЕСЬ]

            _serviceProvider = services.BuildServiceProvider();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
        }
    }
}
