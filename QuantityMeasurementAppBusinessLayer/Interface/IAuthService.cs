using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Models;

namespace QuantityMeasurementAppBusinessLayer.Interface
{
    public interface IAuthService
    {
        void SaveUsers(RegisterDTO user);
        string VerifyUser(LoginDTO login);
        string GenerateJwtToken(UserEntity user);
    }
}