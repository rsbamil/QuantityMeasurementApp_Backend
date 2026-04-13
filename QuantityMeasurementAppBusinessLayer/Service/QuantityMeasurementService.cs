using QuantityMeasurementAppBusinessLayer.Exception;
using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Enums;
using QuantityMeasurementAppModelLayer.Models;
using QuantityMeasurementAppRepositoryLayer.Interface;

namespace QuantityMeasurementAppBusinessLayer.Service
{
    public class QuantityMeasurementService : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;

        // Repository is now properly injected via DI (no more manual new())
        public QuantityMeasurementService(IQuantityMeasurementRepository repository)
        {
            _repository = repository;
        }

        public bool Compare(QuantityDTO first, QuantityDTO second)
        {
            Validate(first);
            Validate(second);

            if (!first.Category!.Equals(second.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Cannot compare different categories.");

            double firstBase = ConvertToBase(first);
            double secondBase = ConvertToBase(second);
            double result = firstBase.CompareTo(secondBase);

            SaveHistory(first, second, "compare", result);

            return firstBase.CompareTo(secondBase) == 0;
        }

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
        {
            Validate(first);
            Validate(second);

            if (!first.Category!.Equals(second.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Cannot add different categories.");

            if (first.Category.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Addition of temperature is not supported.");

            double firstBase = ConvertToBase(first);
            double secondBase = ConvertToBase(second);
            double result = firstBase + secondBase;

            SaveHistory(first, second, "addition", result);

            return new QuantityDTO
            {
                Value = result,
                Unit = GetBaseUnit(first.Category),
                Category = first.Category
            };
        }

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
        {
            Validate(first);
            Validate(second);

            if (!first.Category!.Equals(second.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Cannot subtract different categories.");

            if (first.Category.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Subtraction of temperature is not supported.");

            double firstBase = ConvertToBase(first);
            double secondBase = ConvertToBase(second);
            double result = firstBase - secondBase;

            SaveHistory(first, second, "subtract", result);

            return new QuantityDTO
            {
                Value = result,
                Unit = GetBaseUnit(first.Category),
                Category = first.Category
            };
        }

        public QuantityDTO Division(QuantityDTO first, QuantityDTO second)
        {
            Validate(first);
            Validate(second);

            if (!first.Category!.Equals(second.Category, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Cannot divide different categories.");

            if (first.Category.Equals("Temperature", StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException("Division of temperature is not supported.");

            double firstBase = ConvertToBase(first);
            double secondBase = ConvertToBase(second);

            if (secondBase == 0)
                throw new QuantityMeasurementException("Cannot divide by zero.");

            double result = firstBase / secondBase;

            SaveHistory(first, second, "division", result);

            return new QuantityDTO
            {
                Value = result,
                Unit = GetBaseUnit(first.Category),
                Category = first.Category
            };
        }

        public QuantityDTO Convert(QuantityDTO quantity , string targetUnit)
        {
            Validate(quantity);

            if (string.IsNullOrWhiteSpace(targetUnit))
                throw new QuantityMeasurementException("Target unit is required.");

            double baseValue = ConvertToBase(quantity);
            double convertedValue = ConvertFromBase(baseValue, quantity.Category!, targetUnit);

            _repository.Save(new QuantityMeasurementEntity
            {
                Value1 = quantity.Value,
                Value2 = 0,
                Unit1 = quantity.Unit ?? string.Empty,
                Unit2 = targetUnit,
                Category = quantity.Category ?? string.Empty,
                Operation = "convert",
                Result = convertedValue
            });

            return new QuantityDTO
            {
                Value = convertedValue,
                Unit = targetUnit,
                Category = quantity.Category
            };
        }

        public List<QuantityMeasurementEntity> GetHistory()
        {
            return _repository.GetAll();
        }

        // ─── Private Helpers ───────────────────────────────────────────────

        private void SaveHistory(QuantityDTO dto1, QuantityDTO dto2, string operation, double result)
        {
            _repository.Save(new QuantityMeasurementEntity
            {
                Value1 = dto1.Value,
                Value2 = dto2.Value,
                Unit1 = dto1.Unit ?? string.Empty,
                Unit2 = dto2.Unit ?? string.Empty,
                Category = dto1.Category ?? string.Empty,
                Operation = operation,
                Result = result
            });
        }

        private static void Validate(QuantityDTO dto)
        {
            if (dto == null)
                throw new QuantityMeasurementException("Quantity cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new QuantityMeasurementException("Category is required.");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new QuantityMeasurementException("Unit is required.");
        }

        private static double ConvertToBase(QuantityDTO dto)
        {
            switch (dto.Category!.ToLower())
            {
                case "length":
                    return Enum.Parse<LengthUnit>(dto.Unit!, true) switch
                    {
                        LengthUnit.Inch        => dto.Value,
                        LengthUnit.Feet        => dto.Value * 12,
                        LengthUnit.Yard        => dto.Value * 36,
                        LengthUnit.Centimeter  => dto.Value * 0.393701,
                        _                      => throw new QuantityMeasurementException("Invalid Length Unit")
                    };

                case "weight":
                    return Enum.Parse<WeightUnit>(dto.Unit!, true) switch
                    {
                        WeightUnit.Gram      => dto.Value,
                        WeightUnit.Kilogram  => dto.Value * 1000,
                        WeightUnit.Pound     => dto.Value * 453.592,
                        _                    => throw new QuantityMeasurementException("Invalid Weight Unit")
                    };

                case "volume":
                    return Enum.Parse<VolumeUnit>(dto.Unit!, true) switch
                    {
                        VolumeUnit.Millilitre => dto.Value,
                        VolumeUnit.Litre      => dto.Value * 1000,
                        VolumeUnit.Gallon     => dto.Value * 3785.41,
                        _                     => throw new QuantityMeasurementException("Invalid Volume Unit")
                    };

                case "temperature":
                    return Enum.Parse<TemperatureUnit>(dto.Unit!, true) switch
                    {
                        TemperatureUnit.Celsius    => dto.Value,
                        TemperatureUnit.Fahrenheit => (dto.Value - 32) * 5 / 9,
                        TemperatureUnit.Kelvin     => dto.Value - 273.15,
                        _                          => throw new QuantityMeasurementException("Invalid Temperature Unit")
                    };

                default:
                    throw new QuantityMeasurementException("Invalid Category");
            }
        }

        public static double ConvertFromBase(double baseValue, string category, string targetUnit)
{
    switch (category.ToLower())
    {
        case "length":
            return Enum.Parse<LengthUnit>(targetUnit, true) switch
            {
                LengthUnit.Inch       => baseValue,
                LengthUnit.Feet       => baseValue / 12,
                LengthUnit.Yard       => baseValue / 36,
                LengthUnit.Centimeter => baseValue / 0.393701,
                _ => throw new QuantityMeasurementException("Invalid Length Unit")
            };

        case "weight":
            return Enum.Parse<WeightUnit>(targetUnit, true) switch
            {
                WeightUnit.Gram     => baseValue,
                WeightUnit.Kilogram => baseValue / 1000,
                WeightUnit.Pound    => baseValue / 453.592,
                _ => throw new QuantityMeasurementException("Invalid Weight Unit")
            };

        case "volume":
            return Enum.Parse<VolumeUnit>(targetUnit, true) switch
            {
                VolumeUnit.Millilitre => baseValue,
                VolumeUnit.Litre      => baseValue / 1000,
                VolumeUnit.Gallon     => baseValue / 3785.41,
                _ => throw new QuantityMeasurementException("Invalid Volume Unit")
            };

        case "temperature":
            return Enum.Parse<TemperatureUnit>(targetUnit, true) switch
            {
                TemperatureUnit.Celsius    => baseValue,
                TemperatureUnit.Fahrenheit => (baseValue * 9 / 5) + 32,
                TemperatureUnit.Kelvin     => baseValue + 273.15,
                _ => throw new QuantityMeasurementException("Invalid Temperature Unit")
            };

        default:
            throw new QuantityMeasurementException("Invalid Category");
    }
}

        private static string GetBaseUnit(string category) =>
            category.Trim().ToLower() switch
            {
                "length"      => "Inch",
                "weight"      => "Gram",
                "volume"      => "Millilitre",
                "temperature" => "Celsius",
                _             => "Unknown"
            };
    }
}