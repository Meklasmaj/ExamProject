using System.Windows;
using GymnasieProjekt.Core.Repositories;
using GymnasieProjekt.WPF.ViewModels;
using GymnasieProjekt.WPF.Views;
using Microsoft.Extensions.Configuration;

namespace GymnasieProjekt.WPF;

public partial class App : Application
{
    // The only place that knows both concrete repositories and views.
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString("GymnasieProjektDB")
            ?? throw new InvalidOperationException("Connection string 'GymnasieProjektDB' mangler i appsettings.json");

        IStudentRepository studentRepository = new StudentRepository(connectionString);
        var mainViewModel = new MainViewModel(studentRepository);

        var mainWindow = new MainWindow { DataContext = mainViewModel };
        mainWindow.Show();
    }
}
