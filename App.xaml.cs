using GymPos.Data.DbData;
using GymPos.Repository;
using GymPos.Services;
using GymPos.ViewModels;
using GymPos.ViewModels.Asistencias;
using GymPos.ViewModels.CajaVM;
using GymPos.ViewModels.CarritoVM;
using GymPos.ViewModels.ClienteVM;
using GymPos.ViewModels.MembresiasVM;
using GymPos.ViewModels.ProductoVM;
using GymPos.ViewModels.SuscripcionesVM;
using GymPos.ViewModels.VentaVM;
using GymPos.Views.UsuarioPage;
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
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IServiceAsistencia, ServiceAsistencia>();
        services.AddTransient<IServiceCliente, ServiceCliente>();
        services.AddTransient<IProductoService, ProductoService>();
        services.AddTransient<ICategoriaService, CategoriaService>();
        services.AddTransient<IServiceCaja, ServiceCaja>();
        services.AddTransient<IServiceVenta, ServiceVenta>();
        services.AddSingleton<ICajaEventService, CajaEventService>();
        services.AddDbContext<GymPosContext>(options =>
        {
            options.UseSqlServer("Server=localhost;Database=GymDB;Trusted_Connection=True;TrustServerCertificate=True;");
        });
        // Repository
        services.AddTransient<IRepositoryCliente, RepositoryCliente>();
        services.AddTransient<IRepositorySuscripcion, RepositorySuscripcion>();
        services.AddTransient<IRepositoryMembresia, RepositoryMembresia>();
        services.AddTransient<IRepositoryAsistencia, RepositoryAsistencia>();
        services.AddTransient<IServiceSuscripcion, ServiceSuscripcion>();
        services.AddTransient<IRepositoryProducto, RepositoryProducto>();
        services.AddSingleton<IRepositoryVenta, RepositoryVenta>();
        services.AddSingleton<IRepositoryCaja, RepositoryCaja>();
        services.AddSingleton<IRepositoryPago, RepositoryPago>();
        services.AddSingleton<IRepositoryCategoria, RepositoryCategoria>();
        services.AddTransient<IRepositoryProducto, RepositoryProducto>();
        services.AddSingleton<IRepositoryMovimientoCaja, RepositoryMovimientoCaja>();
        services.AddSingleton<IRepositoryProducto, RepositoryProducto>();
        services.AddSingleton<IRepositoryUsuario, RepositoryUsuario>();

        // ViewModels
        services.AddTransient<ListClienteViewModel>();
        services.AddSingleton<EditClienteViewModel>();
        services.AddTransient<ListMembresiaViewModel>();
        services.AddTransient<CreateSuscripcionViewModel>();
        services.AddTransient<ListSuscripcionViewModel>();
        services.AddTransient<AsistenciaViewModel>();
        services.AddSingleton<CreateClienteViewModel>();
        services.AddSingleton<ResumenCajaViewModel>();
        services.AddTransient<CreateVentaViewModel>();
        services.AddTransient<ListVentaViewModel>();
        services.AddTransient<VentaViewModel>();
        services.AddTransient<EditProductoViewModel>();
        services.AddTransient<ListUsuarioPage>();
        // Productos y carrito
        services.AddTransient<ListProductoViewModel>();
        services.AddSingleton<CarritoViewModel>();
        //services.AddTransient<LoginViewModel>();

        Services = services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        // Mostrar ventana de login siempre al iniciar
        var loginWindow = new Views.LoginWindow();
        _window = loginWindow;
        _window.Activate();
    }
}
