using System;

namespace QuantityMeasurementAppModelLayer.Enums
{
    /// <summary>
    /// Represents supported units for length measurement.
    ///
    /// BASE UNIT:
    /// Feet is treated as the base unit for all length conversions.
    ///
    /// SUPPORTED UNITS:
    /// - Feet
    /// - Inch
    /// - Yard
    /// - Centimeter
    ///
    /// DESIGN NOTE:
    /// Conversion responsibility is centralized through extension methods,
    /// which keeps the enum lightweight while preserving clean separation
    /// of concerns.
    /// </summary>
    public enum LengthUnit
    {
        /// <summary>
        /// Represents measurement in feet.
        /// Base unit for length category.
        /// </summary>
        Feet,

        /// <summary>
        /// Represents measurement in inches.
        /// 1 Inch = 1/12 Feet.
        /// </summary>
        Inch,

        /// <summary>
        /// Represents measurement in yards.
        /// 1 Yard = 3 Feet.
        /// </summary>
        Yard,

        /// <summary>
        /// Represents measurement in centimeters.
        /// 1 Centimeter = 1 / 30.48 Feet.
        /// </summary>
        Centimeter
    }

}