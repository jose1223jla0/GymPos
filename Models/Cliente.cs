namespace GymPos.Models;

public class Cliente
{
    public int Id { get; set; }
    public required string Dni { get; set; }
    public required string Nombres { get; set; }
    public required string Apellidos { get; set; }
}
