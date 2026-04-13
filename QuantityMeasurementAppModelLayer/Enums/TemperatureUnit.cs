using System;

namespace QuantityMeasurementAppModelLayer.Enums
{
    /// <summary>
    /// Supported temperature units.
    /// Base unit: Celsius
    ///
    /// UC14 RULE:
    /// Temperature supports:
    /// - Equality comparison
    /// - Conversion
    ///
    /// Temperature does NOT support:
    /// - Addition
    /// - Subtraction
    /// - Division
    ///
    /// NOTE:
    /// Temperature conversion is non-linear.
    /// </summary>
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }

}