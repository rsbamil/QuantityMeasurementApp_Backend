using System.ComponentModel.DataAnnotations;
namespace QuantityMeasurementAppModelLayer.DTOs
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}