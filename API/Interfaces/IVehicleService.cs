using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IVehicleService
    {
        Task<ActionResult<VehicleDto>> CalculatePremium(VehicleDto vehicleDto);
        Task<ActionResult<VehicleDBDto>> Add(VehicleDBDto addVDto);
        byte[] GenerateDoc(VehicleDto vehicleDto);
    }
}
