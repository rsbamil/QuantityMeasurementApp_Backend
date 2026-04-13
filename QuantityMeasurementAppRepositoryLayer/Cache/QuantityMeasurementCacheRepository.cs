using QuantityMeasurementAppModelLayer.Models;
using QuantityMeasurementAppRepositoryLayer.Interface;

namespace QuantityMeasurementAppRepositoryLayer.Cache
{
    /// <summary>
    /// In-memory repository for testing/development without a database.
    /// </summary>
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        private readonly List<QuantityMeasurementEntity> _storage = new();
        private readonly List<UserEntity> _users = new();
        private int _idCounter = 1;
        private int _userIdCounter = 1;

        public void Save(QuantityMeasurementEntity entity)
        {
            entity.Id = _idCounter++;
            _storage.Add(entity);
        }

        public void SaveUser(UserEntity user)
        {
            user.Id = _userIdCounter++;
            _users.Add(user);
        }

        public UserEntity? GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }

        public List<QuantityMeasurementEntity> GetAll()
        {
            return _storage;
        }
    }
}