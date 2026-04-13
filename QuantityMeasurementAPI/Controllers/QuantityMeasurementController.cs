using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppBusinessLayer.Service;
using QuantityMeasurementAppRepositoryLayer.Database;
using QuantityMeasurementAppModelLayer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using QuantityMeasurementAppBusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
namespace QuantityMeasurementAPI.Controllers
{
    [Route("/api")]
    [ApiController]
    public class QuantityMeasurementAPIController : ControllerBase
    {
        private IQuantityMeasurementService service;
        public QuantityMeasurementAPIController(IQuantityMeasurementService service)
        {
            this.service = service;
        }
        [HttpPost("compare")]
        public IActionResult Compare([FromBody] QuantityInputDTO input)
        {
            if (input.Quantity1 == null || input.Quantity2 == null || input.Quantity1.Unit == null || input.Quantity2.Unit == null)
            {
                return BadRequest("Invalid input. Please provide valid quantity and units.");
            }
            var result = service.Compare(input.Quantity1, input.Quantity2);
            return Ok(result);
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] QuantityInputDTO input)
        {
            if (input.Quantity1 == null || input.Quantity2 == null || input.Quantity1.Unit == null || input.Quantity2.Unit == null)
            {
                return BadRequest("Invalid input. Please provide valid quantity and units.");
            }
            var result = service.Add(input.Quantity1, input.Quantity2);
            return Ok(result);
        }

        [HttpPost("subtract")]
        public IActionResult Subtract([FromBody] QuantityInputDTO input)
        {
            if (input.Quantity1 == null || input.Quantity2 == null || input.Quantity1.Unit == null || input.Quantity2.Unit == null)
            {
                return BadRequest("Invalid input. Please provide valid quantity and units.");
            }
            var result = service.Subtract(input.Quantity1, input.Quantity2);
            return Ok(result);
        }

        [HttpPost("divide")]
        public IActionResult Divide([FromBody] QuantityInputDTO input)
        {
            if (input.Quantity1 == null || input.Quantity2 == null || input.Quantity1.Unit == null || input.Quantity2.Unit == null)
            {
                return BadRequest("Invalid input. Please provide valid quantity and units.");
            }
            var result = service.Division(input.Quantity1, input.Quantity2);
            return Ok(result);
        }

        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            var history = service.GetHistory();
            return Ok(history);
        }

        [HttpPost("convert")]
        public IActionResult Convert([FromBody] ConvertDTO input)
        {
            if(input.Quantity1==null || string.IsNullOrWhiteSpace(input.Quantity1.Unit))
            {
                return BadRequest(new {message = "Invalid Input"});
            }
            if (string.IsNullOrWhiteSpace(input.TargetUnit))
            {
                return BadRequest(new {message = "Target unit is required."});
            }

            var result = service.Convert(input.Quantity1,input.TargetUnit);
            return Ok(result);
        }
    }
}