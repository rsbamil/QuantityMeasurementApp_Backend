using QuantityMeasurementAppModelLayer.Models;

namespace QuantityMeasurementAppRepositoryLayer.Interface
{
    public interface IQuantityMeasurementRepository
    {
        void Save(QuantityMeasurementEntity entity);
        void SaveUser(UserEntity user);
        UserEntity GetUserByEmail(string email);
        List<QuantityMeasurementEntity> GetAll();
    }
}