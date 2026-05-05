using GymPos.Data.DbData;
using GymPos.Repository;
using GymPos.Services;
using GymPos.ViewModels.Asistencias;
using GymPos.ViewModels.Clientes;
using GymPos.ViewModels.Membresias;
using GymPos.ViewModels.Suscripciones;
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
        services.AddTransient<IRepositorySuscripcion, RepositorySuscripcion>();
        services.AddTransient<IRepositoryMembresia, RepositoryMembresia>();
        services.AddTransient<IRepositoryAsistencia, RepositoryAsistencia>();
        services.AddTransient<IServiceCliente, ServiceCliente>();
        services.AddTransient<IServiceSuscripcion, ServiceSuscripcion>();

        // ViewModels
        services.AddTransient<ClienteViewModel>();
        services.AddTransient<AddClienteViewModel>();
        services.AddTransient<MembresiaViewModel>();
        services.AddTransient<CreateSuscripcionViewModel>();
        services.AddTransient<ListSuscripcionViewModel>();
        services.AddTransient<AsistenciaViewModel>();
        //services.AddTransient<LoginViewModel>();



        Services = services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
