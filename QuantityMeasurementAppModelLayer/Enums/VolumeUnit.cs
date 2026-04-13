using System;

namespace QuantityMeasurementAppModelLayer.Enums
{
    /// <summary>
    /// UC11
    /// Represents supported volume measurement units.
    ///
    /// DESIGN NOTE:
    /// In the C# implementation, enum values are kept simple and immutable.
    /// Conversion behavior is provided through extension methods so that the
    /// public usage remains clean and expressive:
    ///
    ///     VolumeUnit.Litre.GetConversionFactor();
    ///     VolumeUnit.Gallon.ConvertToBaseUnit(2.0);
    ///
    /// BASE UNIT:
    /// - Litre is the base unit for all volume conversions.
    ///
    /// CONVERSION FACTORS:
    /// - 1 Litre      = 1.0 L
    /// - 1 Millilitre = 0.001 L
    /// - 1 Gallon     = 3.78541 L
    /// </summary>
    public enum VolumeUnit
    {
        Litre,
        Millilitre,
        Gallon
    }

}