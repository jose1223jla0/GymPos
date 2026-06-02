using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ClienteVM;

public partial class ListClienteViewModel : ObservableObject
{
    public ObservableCollection<Cliente> ClientesPagina { get; } = new();
    public event Action<Cliente>? OpenDialogRequest;
    private readonly IRepositoryCliente _repositoryCliente;
    [ObservableProperty]
    private bool isLoading = false;

    private const int PageSize = 20;
    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int totalPages;

    private bool _loadingPage;
    public ListClienteViewModel(IRepositoryCliente repositoryCliente)
    {
        _repositoryCliente = repositoryCliente;
    }

    public async Task LoadClientes()
    {
        try
        {
            IsLoading = true;
            var total = await _repositoryCliente.CountClientes();
            TotalPages = total / PageSize;
            CurrentPage = 1;
            await LoadPage(CurrentPage);
        }
        finally
        {
            IsLoading = false;
        }
    }

   
    private async Task LoadPage(int page)
    {
        if (_loadingPage) return;

        _loadingPage = true;
        try
        {
            var clientes = await _repositoryCliente.GetPaged(page, PageSize);
            ClientesPagina.Clear();
            foreach (var c in clientes)
            {
                ClientesPagina.Add(c);
            }
        }
        finally
        {
            _loadingPage = false;
        }
    }

    [RelayCommand]
    public async Task NextPage()
    {
        if (CurrentPage >= TotalPages)
        {
            return;
        }
        CurrentPage++;
        await LoadPage(CurrentPage);
    }

    [RelayCommand]
    public async Task PreviousPage()
    {
        if (CurrentPage <= 1)
        {
            return;
        }

        CurrentPage--;
        await LoadPage(CurrentPage);
    }

    [RelayCommand]
    private void Edit(Cliente cliente)
    {
        OpenDialogRequest?.Invoke(cliente);
    }
}
