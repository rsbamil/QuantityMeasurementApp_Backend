using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Enums;
using QuantityMeasurementApp.Models;
using System;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC14: Temperature Measurement Test Suite
    ///
    /// PURPOSE:
    /// This test class validates the behavior of the temperature
    /// measurement implementation introduced in UC14.
    ///
    /// Temperature differs from other measurement categories because:
    /// - It supports equality comparison
    /// - It supports unit conversion
    /// - It DOES NOT support arithmetic operations such as
    ///   addition, subtraction, or division.
    ///
    /// TEST COVERAGE:
    /// - Equality across different temperature units
    /// - Temperature conversion accuracy
    /// - Edge cases (absolute zero and -40°C == -40°F)
    /// - Cross-category comparison protection
    /// - Unit metadata methods (GetUnitName / SupportsArithmetic)
    ///
    /// DESIGN VALIDATION:
    /// These tests ensure the generic Quantity<U> architecture
    /// properly supports a new category with restricted arithmetic rules.
    /// </summary>
    [TestClass]
    public class QuantityTemperatureTests
    {
        /// <summary>
        /// Floating point tolerance used for equality comparisons.
        /// </summary>
        private const double EPSILON = 1e-5;

        /// <summary>
        /// Verifies equality between two temperatures
        /// expressed in the same unit.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToCelsius_SameValue()
        {
            var t1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var t2 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);

            Assert.IsTrue(t1.Equals(t2));
        }

        /// <summary>
        /// Verifies cross-unit equality between
        /// Celsius and Fahrenheit.
        ///
        /// 0°C = 32°F
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
        {
            var celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        /// <summary>
        /// Verifies cross-unit equality between
        /// Celsius and Kelvin.
        ///
        /// 0°C = 273.15 K
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_CelsiusToKelvin_0Celsius273Point15Kelvin()
        {
            var celsius = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
            var kelvin = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.Kelvin);

            Assert.IsTrue(celsius.Equals(kelvin));
        }

        /// <summary>
        /// Validates the well-known temperature equivalence point:
        ///
        /// -40°C == -40°F
        ///
        /// This test ensures that non-linear conversions
        /// are implemented correctly.
        /// </summary>
        [TestMethod]
        public void testTemperatureEquality_Negative40EqualPoint()
        {
            var celsius = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius);
            var fahrenheit = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit);

            Assert.IsTrue(celsius.Equals(fahrenheit));
        }

        /// <summary>
        /// Validates conversion from Celsius to Fahrenheit.
        ///
        /// 100°C = 212°F
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_CelsiusToFahrenheit()
        {
            var celsius = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var converted = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(212.0, converted.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.Fahrenheit, converted.Unit);
        }

        /// <summary>
        /// Validates conversion from Fahrenheit to Celsius.
        ///
        /// 32°F = 0°C
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_FahrenheitToCelsius()
        {
            var fahrenheit = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);
            var converted = fahrenheit.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(0.0, converted.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.Celsius, converted.Unit);
        }

        /// <summary>
        /// Validates conversion from Kelvin to Celsius.
        ///
        /// 273.15 K = 0°C
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_KelvinToCelsius()
        {
            var kelvin = new Quantity<TemperatureUnit>(273.15, TemperatureUnit.Kelvin);
            var converted = kelvin.ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(0.0, converted.Value, EPSILON);
            Assert.AreEqual(TemperatureUnit.Celsius, converted.Unit);
        }

        /// <summary>
        /// Validates temperature conversions around absolute zero.
        ///
        /// Absolute zero:
        /// -273.15°C
        /// 0 Kelvin
        /// -459.67°F
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_AbsoluteZero()
        {
            var celsius = new Quantity<TemperatureUnit>(-273.15, TemperatureUnit.Celsius);

            var kelvin = celsius.ConvertTo(TemperatureUnit.Kelvin);
            var fahrenheit = celsius.ConvertTo(TemperatureUnit.Fahrenheit);

            Assert.AreEqual(0.0, kelvin.Value, EPSILON);
            Assert.AreEqual(-459.67, fahrenheit.Value, 1e-2);
        }

        /// <summary>
        /// Ensures that converting to another unit and back
        /// preserves the original temperature value.
        ///
        /// This validates round-trip conversion stability.
        /// </summary>
        [TestMethod]
        public void testTemperatureConversion_RoundTrip_PreservesValue()
        {
            var original = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            var roundTrip = original
                            .ConvertTo(TemperatureUnit.Fahrenheit)
                            .ConvertTo(TemperatureUnit.Celsius);

            Assert.AreEqual(50.0, roundTrip.Value, EPSILON);
        }

        /// <summary>
        /// Ensures temperature quantities are never considered
        /// equal to length quantities even if numeric values match.
        ///
        /// This validates category isolation within the generic model.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsLengthIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var length = new Quantity<LengthUnit>(100.0, LengthUnit.Feet);

            Assert.IsFalse(temp.Equals(length));
        }

        /// <summary>
        /// Ensures temperature quantities are never considered
        /// equal to weight quantities.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsWeightIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);
            var weight = new Quantity<WeightUnit>(50.0, WeightUnit.Kilogram);

            Assert.IsFalse(temp.Equals(weight));
        }

        /// <summary>
        /// Ensures temperature quantities are never considered
        /// equal to volume quantities.
        /// </summary>
        [TestMethod]
        public void testTemperatureVsVolumeIncompatibility()
        {
            var temp = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);
            var volume = new Quantity<VolumeUnit>(25.0, VolumeUnit.Litre);

            Assert.IsFalse(temp.Equals(volume));
        }

        /// <summary>
        /// Validates that the temperature unit explicitly reports
        /// that arithmetic operations are not supported.
        ///
        /// This is a design constraint introduced in UC14.
        /// </summary>
        [TestMethod]
        public void testOperationSupportMethods_TemperatureUnitAddition()
        {
            Assert.IsFalse(TemperatureUnit.Celsius.SupportsArithmetic());
        }

        /// <summary>
        /// Ensures the human-readable unit names returned by
        /// GetUnitName() match expected values.
        ///
        /// This helps verify enum extension metadata behavior.
        /// </summary>
        [TestMethod]
        public void testTemperatureUnit_NameMethod()
        {
            Assert.AreEqual("Celsius", TemperatureUnit.Celsius.GetUnitName());
            Assert.AreEqual("Fahrenheit", TemperatureUnit.Fahrenheit.GetUnitName());
            Assert.AreEqual("Kelvin", TemperatureUnit.Kelvin.GetUnitName());
        }
    }
}