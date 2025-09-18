using LeasingSys_API.Data;
using LeasingSys_API.Models;
using LeasingSys_API.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LeasingSys_API.Controllers;

// [Route("api/[controller]")]
[Route("api/leasingAPI")]
[ApiController]
public class LeasingAPIController : ControllerBase // 继承 Controller 则会额外支持 MVC 特性
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<LeasingDTO>> GetLeasing()
    {
        // ActionResult 类型可以灵活控制 Ok(data)、NotFound()、BadRequest()、CreatedAtRoute().
        return Ok(LeasingOffice.LeasingList);
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

        var leasingDto = LeasingOffice.LeasingList.FirstOrDefault(u => u.Id == id);
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
        if (LeasingOffice.LeasingList.FirstOrDefault(u => u.Name.ToLower() == leasingDto.Name.ToLower()) != null)
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

        leasingDto.Id = LeasingOffice.LeasingList.OrderByDescending(u => u.Id).FirstOrDefault()?.Id + 1 ?? 0;
        LeasingOffice.LeasingList.Add(leasingDto);
        // return Ok(leasingDto);

        // CreatedAtRoute 会设置 201 状态码 
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

        var leasingDto = LeasingOffice.LeasingList.FirstOrDefault(u => u.Id == id);
        if (leasingDto == null)
        {
            return NotFound();
        }

        LeasingOffice.LeasingList.Remove(leasingDto);
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

        var leasingToUpdate = LeasingOffice.LeasingList.FirstOrDefault(u => u.Id == productId);
        if (leasingToUpdate is null)
        {
            return NotFound();
        }

        // [
        //     {
        //         "op": "replace",
        //         "path": "/YourPropertyName",
        //         "value": "Your New Value"
        //     }
        // ]
        patchLeasingDto.ApplyTo(leasingToUpdate, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();
    }
}