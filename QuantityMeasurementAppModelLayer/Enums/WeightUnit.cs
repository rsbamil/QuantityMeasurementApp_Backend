using System;

namespace QuantityMeasurementAppModelLayer.Enums
{
    /// <summary>
    /// Represents supported units for weight measurement.
    ///
    /// BASE UNIT:
    /// Kilogram is treated as the base unit for all weight conversions.
    ///
    /// SUPPORTED UNITS:
    /// - Kilogram
    /// - Gram
    /// - Pound
    ///
    /// DESIGN NOTE:
    /// Unit-specific conversion logic is encapsulated in extension methods
    /// to maintain separation of concerns and improve scalability.
    /// </summary>
    public enum WeightUnit
    {
        /// <summary>
        /// Represents measurement in kilograms.
        /// Base unit for weight category.
        /// </summary>
        Kilogram,

        /// <summary>
        /// Represents measurement in grams.
        /// 1 Gram = 0.001 Kilogram.
        /// </summary>
        Gram,

        /// <summary>
        /// Represents measurement in pounds.
        /// 1 Pound ≈ 0.453592 Kilogram.
        /// </summary>
        Pound
    }
}