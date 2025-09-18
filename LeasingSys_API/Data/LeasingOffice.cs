using LeasingSys_API.Models.DTO;

namespace LeasingSys_API.Data;

public static class LeasingOffice
{
    public static List<LeasingDTO> LeasingList = new List<LeasingDTO>
    {
        new LeasingDTO { Id = 1, Name = "Leasing1", Occupancy = 4, Square = 100 },
        new LeasingDTO { Id = 2, Name = "Leasing2", Occupancy = 4, Square = 100 },
        new LeasingDTO { Id = 3, Name = "Leasing3", Occupancy = 4, Square = 100 },
    };
}