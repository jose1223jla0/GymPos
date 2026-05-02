using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GymPos.ViewModels.Asistencias;

public partial class AsistenciaViewModel : ObservableObject
{
    public ObservableCollection<Asistencia> Asistencias { get; set; } = new();
    public AsistenciaViewModel()
    {
        
    }
}



