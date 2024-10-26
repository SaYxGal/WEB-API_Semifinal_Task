﻿using HospitalService.Attributes;
using HospitalService.Models;
using HospitalService.Models.Hospitals.DTO;
using HospitalService.Services.Hospitals;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HospitalService.Controllers;
[Route("api/[controller]s")]
[ApiController]
public class HospitalController : ControllerBase
{
    private readonly IHospitalService _hospitalService;

    public HospitalController(IHospitalService hospitalService)
    {
        _hospitalService = hospitalService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get([FromRoute][Required] int id)
    {
        return Ok(await _hospitalService.GetById(id));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetHospitals([FromQuery][Required] int from, [FromQuery][Required] int count)
    {
        return Ok(await _hospitalService.GetHospitals(from, count));
    }

    [HttpGet("{id}/Rooms")]
    [Authorize]
    public async Task<IActionResult> GetHospitalRooms([FromRoute][Required] int id)
    {
        return Ok(await _hospitalService.GetHospitalRooms(id));
    }

    [HttpPost]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> Create([FromBody][Required] CreateHospitalDTO dto)
    {
        await _hospitalService.Create(dto);

        return Created();
    }

    [HttpPut("{id}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> Update([FromRoute][Required] int id, [FromBody][Required] UpdateHospitalDTO dto)
    {
        await _hospitalService.Update(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(UserRole.Admin)]
    public async Task<IActionResult> Delete([FromRoute][Required] int id)
    {
        await _hospitalService.Delete(id);

        return NoContent();
    }
}
