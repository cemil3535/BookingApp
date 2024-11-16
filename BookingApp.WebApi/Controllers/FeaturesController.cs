﻿using BookingApp.Business.Operations.Feature;
using BookingApp.Business.Operations.Feature.Dtos;
using BookingApp.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BookingApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        public FeaturesController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> AddFeature(AddFeatureRequest request)
        {
            var addFeatureDto = new AddFeatureDto
            {
                Title = request.Title
            };
            // Controllerin gorevi verileri service netodlari kullanarak Business Katmanina gondermek. Asil is Business KAtmnainda gercklesiyor.

            var result = await _featureService.AddFeature(addFeatureDto);

            if (result.IsSucceed)
                return Ok();
            else
                return BadRequest(result.Message);
        }
    }
}
