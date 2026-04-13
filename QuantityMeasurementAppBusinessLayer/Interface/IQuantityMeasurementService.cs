using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Models;

namespace QuantityMeasurementAppBusinessLayer.Interface
{
    public interface IQuantityMeasurementService
    {
        bool Compare(QuantityDTO first, QuantityDTO second);
        QuantityDTO Add(QuantityDTO first, QuantityDTO second);
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second);
        QuantityDTO Division(QuantityDTO first, QuantityDTO second);
        QuantityDTO Convert(QuantityDTO quantity , string targetUnit);
        List<QuantityMeasurementEntity> GetHistory();
    }
}