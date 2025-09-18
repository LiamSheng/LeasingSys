using System.ComponentModel.DataAnnotations;

namespace LeasingSys_API.Models.DTO;

public class LeasingDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Occupancy { get; set; }
    public int Square { get; set; }
}