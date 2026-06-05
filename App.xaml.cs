using GymPos.Data.DbData;
using GymPos.Models;
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
using GymPos.ViewModels.UsuarioVM;
using GymPos.ViewModels.VentaVM;
using GymPos.Views.UsuarioPage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Linq;

namespace GymPos;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }
    public static Window? MainAppWindow { get; set; }
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
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = System.IO.Path.Combine(folder, "GymPos");
            System.IO.Directory.CreateDirectory(dir);
            var dbPath = System.IO.Path.Combine(dir, "gympos.db");
            options.UseSqlite($"Data Source={dbPath}");
        });
        // Repository
        services.AddTransient<IRepositoryCliente, RepositoryCliente>();
        services.AddTransient<IRepositorySuscripcion, RepositorySuscripcion>();
        services.AddTransient<IRepositoryMembresia, RepositoryMembresia>();
        services.AddTransient<IRepositoryAsistencia, RepositoryAsistencia>();
        services.AddTransient<IRepositoryVenta, RepositoryVenta>();
        services.AddTransient<IServiceSuscripcion, ServiceSuscripcion>();
        services.AddTransient<IRepositoryProducto, RepositoryProducto>();
        services.AddTransient<IRepositoryCaja, RepositoryCaja>();
        services.AddTransient<IRepositoryPago, RepositoryPago>();
        services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
        services.AddTransient<IRepositoryMovimientoCaja, RepositoryMovimientoCaja>();
        services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
        services.AddSingleton<IAuthService, AuthService>();

        // ViewModels
        services.AddSingleton<DashboardViewModel>();
        services.AddTransient<ListClienteViewModel>();
        services.AddSingleton<EditClienteViewModel>();
        services.AddTransient<ListMembresiaViewModel>();
        services.AddSingleton<CreateMembresiaViewModel>();
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
        services.AddSingleton<EditMembresiaViewModel>();
        // ViewModels de usuario
        services.AddTransient<ListUsuarioViewModel>();
        services.AddTransient<CreateUsuarioViewModel>();
        services.AddTransient<EditUsuarioViewModel>();
        // Productos y carrito
        services.AddTransient<ListProductoViewModel>();
        services.AddSingleton<CarritoViewModel>();
        //services.AddTransient<LoginViewModel>();

        Services = services.BuildServiceProvider();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        using (var scope = Services!.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GymPosContext>();
            context.Database.EnsureCreated();

            if (!context.Usuarios.Any(u => u.Rol == Rol.SuperAdmin))
            {
                context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = "Jose Luis",
                    ApellidosUsuario = "Andrade Oscco",
                    UsernameDni = "70437176",
                    Password = BCrypt.Net.BCrypt.HashPassword("print(jose)"),
                    EstadoUsuario = true,
                    Rol = Rol.SuperAdmin
                });
            }
            var categoriasPredefinidas = new[]
            {
                "Suplementos",
                "Bebidas",
                "Proteínas",
                "Accesorios Deportivos",
                "Ropa Deportiva",
                "Higiene Personal",
                "Snacks Saludables",
                "Equipamiento Fitness"
            };

            foreach (var nombre in categoriasPredefinidas)
            {
                if (!context.Categorias.Any(c => c.NombreCategoria == nombre))
                {
                    context.Categorias.Add(new Categoria
                    {
                        NombreCategoria = nombre
                    });
                }
            }
            // Guardar cambios (usuarios y categorías)
            context.SaveChanges();
        }
        var loginWindow = new Views.LoginWindow();
        _window = loginWindow;
        _window.Activate();
    }
}
