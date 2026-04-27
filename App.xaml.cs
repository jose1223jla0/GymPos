using GymPos.Data.DbData;
using GymPos.Repository;
using GymPos.Services;
using GymPos.ViewModels.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace GymPos;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }
    private Window? _window;
    public App()
    {
        InitializeComponent();
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        // Servicios
        services.AddSingleton<INavigationService, NavegationService>();
        services.AddDbContext<GymPosContext>(options =>
        {
            options.UseSqlServer("Server=localhost;Database=GymDB;Trusted_Connection=True;TrustServerCertificate=True;");
        });
        // Repository
        services.AddTransient<IRepositoryCliente, RepositoryCliente>();


        // ViewModels
        services.AddTransient<ClienteViewModel>();
        //services.AddTransient<LoginViewModel>();



        Services = services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
