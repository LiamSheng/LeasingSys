using LeasingSys_API.Data;
using LeasingSys_API.Models;
using LeasingSys_API.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace LeasingSys_API.Controllers;

// [Route("api/[controller]")]
[Route("api/leasingAPI")]
[ApiController]
public class LeasingAPIController : ControllerBase // 继承 Controller 则会额外支持 MVC 特性.
{
    [HttpGet]
    public IEnumerable<LeasingDTO> GetLeasing()
    {
        return LeasingOffice.LeasingList;
    }

    [HttpGet("id:int")]
    public LeasingDTO GetLeasing(int id)
    {
        return LeasingOffice.LeasingList.FirstOrDefault(u => u.Id == id);
    }
}