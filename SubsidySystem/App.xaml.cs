using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubsidySystem.Models;
using SubsidySystem.Repositories;
using SubsidySystem.Services;
using SubsidySystem.ViewModels;
using SubsidySystem.Views;
using System;
using System.IO;
using System.Windows;

namespace SubsidySystem
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            // Контекст базы данных
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Репозитории
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();

            // Сервисы
            services.AddScoped<ISubsidyCalculationService, SubsidyCalculationService>();

            // ViewModels
            services.AddTransient<CitizenRegistrationViewModel>();
            services.AddTransient<FamilyMemberViewModel>();
            services.AddTransient<IncomeViewModel>();
            services.AddTransient<SubsidyCalculationViewModel>();
            services.AddTransient<ReportViewModel>();
            services.AddTransient<ApproveRegistriesViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<StandardsViewModel>();
            services.AddTransient<DictionariesViewModel>();

            // Окна
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<CitizenRegistrationWindow>();
            services.AddTransient<FamilyMemberWindow>();
            services.AddTransient<IncomeWindow>();
            services.AddTransient<SubsidyCalculationWindow>();
            services.AddTransient<ReportWindow>();
            services.AddTransient<ApproveRegistriesWindow>();
            services.AddTransient<UsersWindow>();
            services.AddTransient<StandardsWindow>();
            services.AddTransient<DictionariesWindow>();

            ServiceProvider = services.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }
    }
}