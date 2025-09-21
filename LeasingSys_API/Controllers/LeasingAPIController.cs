using System;
using System.Collections.Generic;
using System.Linq;
using LeasingSys_API.Data;
using LeasingSys_API.Models;
using LeasingSys_API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LeasingSys_API.Controllers;

// [Route("api/[controller]")]
[Route("api/leasingAPI")]
[ApiController]
public class LeasingAPIController : ControllerBase // 继承 Controller 则会额外支持 MVC 特性
{
    private readonly ApplicationDbContext _db;

    public LeasingAPIController(ApplicationDbContext db)
    {
        this._db = db;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<LeasingDTO>> GetLeasing()
    {
        // ActionResult 类型可以灵活控制 Ok(data)、NotFound()、BadRequest()、CreatedAtRoute().
        return Ok(this._db.Leasing.ToList());
    }

    [HttpGet("{id:int}", Name = "GetLeasing")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LeasingDTO))] // 可通过 ActionResult<T> 隐式推断
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // 避免 SwaggerUI 未记录此状态码
    public ActionResult<LeasingDTO> GetLeasing(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var leasingDto = this._db.Leasing.FirstOrDefault(u => u.Id == id);
        if (leasingDto == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(leasingDto);
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<LeasingDTO> CreateLeasing([FromBody] LeasingDTO leasingDto) // [FromBody] -> 数据来自请求体
    {
        // 如果请求的 Body 有问题，导致 leasingDto 可能会变成 null，
        // 那么请求根本就不会进入到您的 Action 方法内部，而是提前就被框架拦截并返回 400 错误.
        // 因此不需要判断 if (leasingDto is null) {}
        // 甚至因为框架帮我们处理了空请求体后, 这个方法内部都不会运行.
        // if (!ModelState.IsValid)
        // {
        //     return Ok(leasingDto);
        // }
        if (this._db.Leasing.FirstOrDefault(u => u.Name.ToLower() == leasingDto.Name.ToLower()) != null)
        {
            // 报错的时候会显示:
            // {
            //     "Key -> NameIsUnique": ["ErrorMessage -> Name already exists"]
            // }
            ModelState.AddModelError("Key -> NameIsUnique", "ErrorMessage -> Name already exists");
            return BadRequest(ModelState);
        }

        if (leasingDto.Id < 0)
        {
            return BadRequest();
        }

        if (leasingDto.Id > 0)
        {
            return StatusCode(StatusCodes.Status400BadRequest, leasingDto);
        }
        // Id 交由 EFCore 管理.
        //leasingDto.Id = this._db.Leasing.OrderByDescending(u => u.Id).FirstOrDefault()?.Id + 1 ?? 0;

        Leasing model = new Leasing() { };
        model.Amenity = leasingDto.Amenity;
        model.Details = leasingDto.Details;
        model.ImageUrl = leasingDto.ImageUrl;
        model.Name = leasingDto.Name;
        model.Occupancy = leasingDto.Occupancy;
        model.Rate = leasingDto.Rate;
        model.Sqft = leasingDto.Sqft;

        this._db.Leasing.Add(model);
        this._db.SaveChanges();

        // CreatedAtRoute 会设置 201 状态码 
        leasingDto.Id = model.Id;
        return CreatedAtRoute("GetLeasing", new { id = leasingDto.Id }, leasingDto);
    }

    // Name = "DeleteLeasing" 使得 CreatedAtRoute 或 RedirectToRoute 方法安全地生成 URL.
    [HttpDelete("{id:int}", Name = "DeleteLeasing")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult DeleteLeasing(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var leasingDto = this._db.Leasing.FirstOrDefault(u => u.Id == id);
        if (leasingDto == null)
        {
            return NotFound();
        }

        this._db.Leasing.Remove(leasingDto);
        this._db.SaveChanges();
        return NoContent();
    }

    // [HttpPut("{id:int}", Name = "UpdateLeasing")]
    // [ProducesResponseType(StatusCodes.Status400BadRequest)]
    // [ProducesResponseType(StatusCodes.Status204NoContent)]
    // public IActionResult UpdateLeasing(
    //     [FromRoute(Name = "id")] int productId,
    //     [FromBody] LeasingDTO leasingDto) // 参数名和占位符名不一致, 需要 [FromRoute(Name = ...
    // {
    //     if (productId <= 0 || productId != leasingDto.Id)
    //     {
    //         return BadRequest();
    //     }
    //
    //     var leasingToUpdate = LeasingOffice.LeasingList.FirstOrDefault(u => u.Id == productId);
    //     // 全部替换一次会很消耗计算资源. 应该使用 JSON patch.
    //     leasingToUpdate.Name = leasingDto.Name;
    //     leasingToUpdate.Occupancy = 0;
    //     leasingToUpdate.Square = 0;
    //
    //     return NoContent();
    // }

    [HttpPatch("{id:int}", Name = "UpdatePartialLeasing")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult UpdateLeasing(
        [FromRoute(Name = "id")] int productId,
        [FromBody] JsonPatchDocument<LeasingDTO>? patchLeasingDto)
    {
        if (productId <= 0 || patchLeasingDto is null)
        {
            return BadRequest();
        }

        // 1. 从数据库获取【正在被跟踪】的实体
        var leasingFromDb = this._db.Leasing.FirstOrDefault(u => u.Id == productId);
        if (leasingFromDb is null)
        {
            return NotFound();
        }

        // 2. 创建一个临时的 DTO，它的数据来自原始实体
        //    这是应用补丁所必需的步骤
        LeasingDTO leasingDto = new LeasingDTO()
        {
            Id = leasingFromDb.Id, // 别忘了复制 Id
            Amenity = leasingFromDb.Amenity,
            Details = leasingFromDb.Details,
            ImageUrl = leasingFromDb.ImageUrl,
            Name = leasingFromDb.Name,
            Occupancy = leasingFromDb.Occupancy,
            Rate = leasingFromDb.Rate,
            Sqft = leasingFromDb.Sqft
        };

        // 3. 将补丁应用到【临时的 DTO】上
        patchLeasingDto.ApplyTo(leasingDto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // 4. 将 DTO 中被修改后的值，手动同步回【原始的、被跟踪的实体】
        //    EF Core 会自动检测到这些属性的变化
        leasingFromDb.Name = leasingDto.Name;
        leasingFromDb.Details = leasingDto.Details;
        leasingFromDb.Rate = leasingDto.Rate;
        leasingFromDb.Sqft = leasingDto.Sqft;
        leasingFromDb.Occupancy = leasingDto.Occupancy;
        leasingFromDb.ImageUrl = leasingDto.ImageUrl;
        leasingFromDb.Amenity = leasingDto.Amenity;
        leasingFromDb.UpdatedDate = DateTime.Now; // 如果需要，更新修改时间

        // 5. 保存更改。EF Core 知道要更新哪个实体以及哪些字段被修改了。
        this._db.SaveChanges();

        return NoContent();
    }
}