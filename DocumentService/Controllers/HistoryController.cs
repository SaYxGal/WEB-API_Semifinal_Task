using DocumentService.Attributes;
using DocumentService.Models;
using DocumentService.Models.History.DTO;
using DocumentService.Services.Histories;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DocumentService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly IHistoryService _historyService;

    public HistoryController(IHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpPost]
    [Authorize(UserRole.Admin, UserRole.Manager, UserRole.Doctor)]
    public async Task<IActionResult> Create([FromBody][Required] AddHistoryRecordDTO dto)
    {
        await _historyService.CreateHistoryRecord(dto);

        return Created();
    }

    [HttpPut("{id}")]
    [Authorize(UserRole.Admin, UserRole.Manager, UserRole.Doctor)]
    public async Task<IActionResult> Update([FromRoute][Required] int id, [FromBody][Required] UpdateHistoryRecordDTO dto)
    {
        await _historyService.UpdateHistoryRecord(id, dto);

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHistoryRecord([FromRoute][Required] int id)
    {
        return Ok(await _historyService.GetHistoryRecord(id));
    }

    [HttpGet("/Account/{id}")]
    public async Task<IActionResult> GetAccountHistory([FromRoute][Required] string id)
    {
        return Ok(await _historyService.GetAccountHistory(id));
    }
}
