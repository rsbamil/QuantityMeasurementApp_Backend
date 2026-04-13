using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Enums;
using QuantityMeasurementApp.Models;
using System;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC11 test suite for volume measurement support using the generic Quantity<U> model.
    ///
    /// PURPOSE:
    /// Validates that the UC10 generic design scales to a third category (Volume)
    /// without modifying Quantity<U> core logic.
    ///
    /// COVERAGE:
    /// - Volume unit conversion factor validation
    /// - Base-unit conversion correctness
    /// - Equality across same and different units
    /// - Conversion across all volume units
    /// - Addition across same and different units
    /// - Explicit target-unit addition
    /// - Edge cases: zero, negative, small, large values
    /// - Architectural consistency with the UC10 generic design
    ///
    /// IMPORTANT DOMAIN NOTE:
    /// - Base unit for volume is Litre.
    /// - 1 Gallon ≈ 3.78541 Litres
    /// - 1 Litre ≈ 0.264172 Gallons
    /// </summary>
    [TestClass]
    public class QuantityVolumeTests
    {
        /// <summary>
        /// Standard floating-point comparison tolerance used across volume tests.
        ///
        /// WHY THIS IS NEEDED:
        /// Unit conversions involving decimal factors such as gallons may produce
        /// small floating-point rounding differences. Epsilon-based assertions
        /// ensure mathematically equivalent values are treated as equal.
        /// </summary>
        private const double EPSILON = 1e-5;

        // =========================================================
        // 1. VolumeUnit Enum / Extension Method Tests
        // =========================================================

        /// <summary>
        /// Verifies that Litre exposes the correct conversion factor.
        ///
        /// BUSINESS RULE:
        /// Litre is the base unit for the volume category, therefore its
        /// conversion factor must always be 1.0.
        ///
        /// FAILURE IMPACT:
        /// If this fails, all downstream litre-based conversions and additions
        /// become incorrect across the system.
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_LitreConstant()
        {
            Assert.AreEqual(1.0, VolumeUnit.Litre.GetConversionFactor(), EPSILON);
        }

        /// <summary>
        /// Verifies that Millilitre exposes the correct conversion factor.
        ///
        /// BUSINESS RULE:
        /// 1 Millilitre = 0.001 Litres.
        ///
        /// FAILURE IMPACT:
        /// Incorrect metric conversion would break equality, conversion,
        /// and addition between litre and millilitre quantities.
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_MillilitreConstant()
        {
            Assert.AreEqual(0.001, VolumeUnit.Millilitre.GetConversionFactor(), EPSILON);
        }

        /// <summary>
        /// Verifies that Gallon exposes the correct conversion factor.
        ///
        /// BUSINESS RULE:
        /// 1 Gallon ≈ 3.78541 Litres.
        ///
        /// FAILURE IMPACT:
        /// Any gallon-related conversion, comparison, or arithmetic would
        /// become inaccurate if the factor is incorrect.
        /// </summary>
        [TestMethod]
        public void testVolumeUnitEnum_GallonConstant()
        {
            Assert.AreEqual(3.78541, VolumeUnit.Gallon.GetConversionFactor(), EPSILON);
        }

        /// <summary>
        /// Verifies conversion to base unit when the source unit is already Litre.
        ///
        /// EXPECTATION:
        /// Converting a litre value to base unit must return the same numeric value.
        ///
        /// DESIGN VALIDATION:
        /// Confirms identity conversion behavior for the base unit.
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_LitreToLitre()
        {
            Assert.AreEqual(5.0, VolumeUnit.Litre.ConvertToBaseUnit(5.0), EPSILON);
        }

        /// <summary>
        /// Verifies conversion from Millilitre to base unit Litre.
        ///
        /// BUSINESS RULE:
        /// 1000 mL = 1 L.
        ///
        /// DESIGN VALIDATION:
        /// Confirms normalization through the base unit works for metric
        /// sub-units of volume.
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_MillilitreToLitre()
        {
            Assert.AreEqual(1.0, VolumeUnit.Millilitre.ConvertToBaseUnit(1000.0), EPSILON);
        }

        /// <summary>
        /// Verifies conversion from Gallon to base unit Litre.
        ///
        /// BUSINESS RULE:
        /// 1 Gallon ≈ 3.78541 L.
        ///
        /// FAILURE IMPACT:
        /// If base normalization is wrong, equality and arithmetic involving
        /// gallons will propagate incorrect results.
        /// </summary>
        [TestMethod]
        public void testConvertToBaseUnit_GallonToLitre()
        {
            Assert.AreEqual(3.78541, VolumeUnit.Gallon.ConvertToBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Verifies conversion from base unit back to Litre.
        ///
        /// EXPECTATION:
        /// Since Litre is the base unit, converting from base must preserve value.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToLitre()
        {
            Assert.AreEqual(2.0, VolumeUnit.Litre.ConvertFromBaseUnit(2.0), EPSILON);
        }

        /// <summary>
        /// Verifies conversion from base unit Litre to Millilitre.
        ///
        /// BUSINESS RULE:
        /// 1 L = 1000 mL.
        ///
        /// DESIGN VALIDATION:
        /// Confirms expansion from normalized base value into a smaller unit.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToMillilitre()
        {
            Assert.AreEqual(1000.0, VolumeUnit.Millilitre.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Verifies conversion from base unit Litre to Gallon.
        ///
        /// BUSINESS RULE:
        /// 3.78541 L ≈ 1 Gallon.
        ///
        /// DESIGN VALIDATION:
        /// Confirms reverse conversion works accurately for non-metric units.
        /// </summary>
        [TestMethod]
        public void testConvertFromBaseUnit_LitreToGallon()
        {
            Assert.AreEqual(1.0, VolumeUnit.Gallon.ConvertFromBaseUnit(3.78541), EPSILON);
        }

        /// <summary>
        /// Verifies that each volume unit exposes a readable display name.
        ///
        /// PURPOSE:
        /// Human-readable names are useful for diagnostics, logging,
        /// console output, and demonstration layers.
        /// </summary>
        [TestMethod]
        public void testVolumeUnit_GetUnitName()
        {
            Assert.AreEqual("Litre", VolumeUnit.Litre.GetUnitName());
            Assert.AreEqual("Millilitre", VolumeUnit.Millilitre.GetUnitName());
            Assert.AreEqual("Gallon", VolumeUnit.Gallon.GetUnitName());
        }

        // =========================================================
        // 2. Equality Tests
        // =========================================================

        /// <summary>
        /// Verifies equality for two litre quantities with the same value.
        ///
        /// PURPOSE:
        /// Establishes the baseline equality behavior within the same unit.
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToLitre_SameValue()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies inequality for two litre quantities with different values.
        ///
        /// PURPOSE:
        /// Confirms that equality logic does not produce false positives
        /// when both quantities are in the same unit but differ numerically.
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToLitre_DifferentValue()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

            Assert.IsFalse(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies equality for two millilitre quantities with identical values.
        ///
        /// PURPOSE:
        /// Confirms same-unit equality for the metric sub-unit case.
        /// </summary>
        [TestMethod]
        public void testEquality_MillilitreToMillilitre_SameValue()
        {
            var v1 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);
            var v2 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies equality for two gallon quantities with identical values.
        ///
        /// PURPOSE:
        /// Confirms same-unit equality for the imperial-style unit case.
        /// </summary>
        [TestMethod]
        public void testEquality_GallonToGallon_SameValue()
        {
            var v1 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Gallon);
            var v2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Gallon);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies cross-unit equality between Litre and Millilitre.
        ///
        /// BUSINESS RULE:
        /// 1 L = 1000 mL.
        ///
        /// DESIGN VALIDATION:
        /// Confirms Quantity<U>.Equals() correctly normalizes through
        /// the base unit before comparison.
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToMillilitre_EquivalentValue()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            Assert.IsTrue(litre.Equals(ml));
        }

        /// <summary>
        /// Verifies symmetric cross-unit equality from Millilitre to Litre.
        ///
        /// PURPOSE:
        /// Confirms the equality contract is symmetric:
        /// if A equals B, then B must equal A.
        /// </summary>
        [TestMethod]
        public void testEquality_MillilitreToLitre_EquivalentValue()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            Assert.IsTrue(ml.Equals(litre));
        }

        /// <summary>
        /// Verifies cross-unit equality between Litre and Gallon using
        /// a mathematically equivalent gallon value.
        ///
        /// BUSINESS RULE:
        /// 1 L ≈ 0.264172052 Gallon.
        ///
        /// DESIGN VALIDATION:
        /// Confirms floating-point tolerance is correctly handled during
        /// cross-system conversion comparison.
        /// </summary>
        [TestMethod]
        public void testEquality_LitreToGallon_EquivalentValue()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var gallonEquivalent = new Quantity<VolumeUnit>(0.264172052, VolumeUnit.Gallon);

            Assert.IsTrue(litre.Equals(gallonEquivalent));
        }

        /// <summary>
        /// Verifies cross-unit equality between Gallon and Litre.
        ///
        /// BUSINESS RULE:
        /// 1 Gallon ≈ 3.78541 L.
        ///
        /// PURPOSE:
        /// Confirms the reverse comparison direction also succeeds.
        /// </summary>
        [TestMethod]
        public void testEquality_GallonToLitre_EquivalentValue()
        {
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
            var litreEquivalent = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

            Assert.IsTrue(gallon.Equals(litreEquivalent));
        }

        /// <summary>
        /// Verifies cross-unit equality between Millilitre and Gallon.
        ///
        /// PURPOSE:
        /// Confirms volume comparisons work correctly even when both operands
        /// are non-base units and require normalization through litres.
        /// </summary>
        [TestMethod]
        public void testEquality_MillilitreToGallon_EquivalentValue()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var gallonEquivalent = new Quantity<VolumeUnit>(0.264172052, VolumeUnit.Gallon);

            Assert.IsTrue(ml.Equals(gallonEquivalent));
        }

        /// <summary>
        /// Verifies reflexive equality.
        ///
        /// EQUALITY CONTRACT:
        /// Any object must be equal to itself.
        /// </summary>
        [TestMethod]
        public void testEquality_SameReference()
        {
            var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            Assert.IsTrue(volume.Equals(volume));
        }

        /// <summary>
        /// Verifies comparison against null returns false.
        ///
        /// EQUALITY CONTRACT:
        /// A valid quantity instance must never be equal to null.
        /// </summary>
        [TestMethod]
        public void testEquality_NullComparison()
        {
            var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            Assert.IsFalse(volume.Equals(null));
        }

        /// <summary>
        /// Verifies zero-value quantities are equal across compatible units.
        ///
        /// PURPOSE:
        /// Confirms that normalization logic handles zero consistently,
        /// regardless of unit type.
        /// </summary>
        [TestMethod]
        public void testEquality_ZeroValue()
        {
            var v1 = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies negative quantities compare correctly across units.
        ///
        /// PURPOSE:
        /// Ensures sign preservation during normalization and equality checks.
        /// </summary>
        [TestMethod]
        public void testEquality_NegativeVolume()
        {
            var v1 = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(-1000.0, VolumeUnit.Millilitre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies equality remains accurate for large values.
        ///
        /// PURPOSE:
        /// Confirms the generic design remains reliable for high-magnitude
        /// numeric inputs without precision breakdown in standard scenarios.
        /// </summary>
        [TestMethod]
        public void testEquality_LargeVolumeValue()
        {
            var v1 = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.Millilitre);
            var v2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Litre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies equality remains accurate for very small values.
        ///
        /// PURPOSE:
        /// Confirms precision handling for low-magnitude quantities.
        /// </summary>
        [TestMethod]
        public void testEquality_SmallVolumeValue()
        {
            var v1 = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Millilitre);

            Assert.IsTrue(v1.Equals(v2));
        }

        /// <summary>
        /// Verifies transitive equality across three equivalent representations.
        ///
        /// EQUALITY CONTRACT:
        /// If A == B and B == C, then A must equal C.
        ///
        /// PURPOSE:
        /// Confirms conversion normalization preserves mathematical consistency.
        /// </summary>
        [TestMethod]
        public void testEquality_TransitiveProperty()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var c = new Quantity<VolumeUnit>(0.264172052, VolumeUnit.Gallon);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));
        }

        /// <summary>
        /// Verifies the symmetry property of equality.
        ///
        /// EQUALITY CONTRACT:
        /// If A equals B, then B must equal A.
        /// </summary>
        [TestMethod]
        public void testEquality_SymmetricProperty()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            Assert.AreEqual(a.Equals(b), b.Equals(a));
        }

        /// <summary>
        /// Verifies that volume cannot be considered equal to length.
        ///
        /// ARCHITECTURAL RULE:
        /// Measurement categories are isolated and non-interoperable.
        ///
        /// PURPOSE:
        /// Confirms runtime category safety in equality operations.
        /// </summary>
        [TestMethod]
        public void testEquality_VolumeVsLength_Incompatible()
        {
            var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);

            Assert.IsFalse(volume.Equals(length));
        }

        /// <summary>
        /// Verifies that volume cannot be considered equal to weight.
        ///
        /// ARCHITECTURAL RULE:
        /// Volume, length, and weight are separate categories and must not mix.
        /// </summary>
        [TestMethod]
        public void testEquality_VolumeVsWeight_Incompatible()
        {
            var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(volume.Equals(weight));
        }

        // =========================================================
        // 3. Conversion Tests
        // =========================================================

        /// <summary>
        /// Verifies conversion from Litre to Millilitre.
        ///
        /// BUSINESS RULE:
        /// 1 L = 1000 mL.
        ///
        /// PURPOSE:
        /// Confirms generic conversion logic works for a metric up-scaling case.
        /// </summary>
        [TestMethod]
        public void testConversion_LitreToMillilitre()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var converted = litre.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(1000.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion from Millilitre to Litre.
        ///
        /// BUSINESS RULE:
        /// 1000 mL = 1 L.
        ///
        /// PURPOSE:
        /// Confirms generic conversion logic works for metric normalization.
        /// </summary>
        [TestMethod]
        public void testConversion_MillilitreToLitre()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var converted = ml.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(1.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion from Gallon to Litre.
        ///
        /// BUSINESS RULE:
        /// 1 Gallon ≈ 3.78541 L.
        ///
        /// PURPOSE:
        /// Confirms non-metric conversion support through the generic design.
        /// </summary>
        [TestMethod]
        public void testConversion_GallonToLitre()
        {
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
            var converted = gallon.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(3.78541, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion from Litre to Gallon.
        ///
        /// PURPOSE:
        /// Confirms reverse imperial conversion is accurate within tolerance.
        /// </summary>
        [TestMethod]
        public void testConversion_LitreToGallon()
        {
            var litre = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
            var converted = litre.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(1.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Gallon, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion from Millilitre to Gallon.
        ///
        /// PURPOSE:
        /// Confirms generic conversion can move between two non-base units
        /// by correctly normalizing through the base unit.
        /// </summary>
        [TestMethod]
        public void testConversion_MillilitreToGallon()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var converted = ml.ConvertTo(VolumeUnit.Gallon);

            Assert.AreEqual(0.264172052, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Gallon, converted.GetUnit());
        }

        /// <summary>
        /// Verifies that conversion to the same unit is identity-preserving.
        ///
        /// PURPOSE:
        /// Ensures unnecessary unit changes do not alter value or unit.
        /// </summary>
        [TestMethod]
        public void testConversion_SameUnit()
        {
            var litre = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var converted = litre.ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(5.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion behavior for zero.
        ///
        /// PURPOSE:
        /// Confirms zero is preserved across unit conversion and does not
        /// introduce any arithmetic anomalies.
        /// </summary>
        [TestMethod]
        public void testConversion_ZeroValue()
        {
            var litre = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);
            var converted = litre.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(0.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies conversion preserves negative sign.
        ///
        /// PURPOSE:
        /// Confirms conversion logic behaves correctly for negative quantities.
        /// </summary>
        [TestMethod]
        public void testConversion_NegativeValue()
        {
            var litre = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre);
            var converted = litre.ConvertTo(VolumeUnit.Millilitre);

            Assert.AreEqual(-1000.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, converted.GetUnit());
        }

        /// <summary>
        /// Verifies round-trip conversion preserves the original value.
        ///
        /// PURPOSE:
        /// Converts Litre -> Millilitre -> Litre and confirms the final result
        /// is equivalent to the original within floating-point tolerance.
        /// </summary>
        [TestMethod]
        public void testConversion_RoundTrip()
        {
            var original = new Quantity<VolumeUnit>(1.5, VolumeUnit.Litre);
            var converted = original.ConvertTo(VolumeUnit.Millilitre)
                                    .ConvertTo(VolumeUnit.Litre);

            Assert.AreEqual(1.5, converted.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, converted.GetUnit());
        }

        // =========================================================
        // 4. Addition Tests
        // =========================================================

        /// <summary>
        /// Verifies addition of two litre quantities.
        ///
        /// PURPOSE:
        /// Establishes the baseline arithmetic behavior for same-unit addition.
        /// </summary>
        [TestMethod]
        public void testAddition_SameUnit_LitrePlusLitre()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(3.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition of two millilitre quantities.
        ///
        /// PURPOSE:
        /// Confirms same-unit arithmetic works correctly for sub-units too.
        /// </summary>
        [TestMethod]
        public void testAddition_SameUnit_MillilitrePlusMillilitre()
        {
            var v1 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);
            var v2 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(1000.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition across Litre and Millilitre with implicit target unit.
        ///
        /// BUSINESS RULE:
        /// Result should default to the unit of the first operand.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_LitrePlusMillilitre()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            var result = litre.Add(ml);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition across Millilitre and Litre with implicit target unit.
        ///
        /// BUSINESS RULE:
        /// Result should be expressed in the first operand's unit, here Millilitre.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_MillilitrePlusLitre()
        {
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

            var result = ml.Add(litre);

            Assert.AreEqual(2000.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition across Gallon and Litre with implicit target unit.
        ///
        /// PURPOSE:
        /// Confirms generic add() works across metric and imperial units.
        /// </summary>
        [TestMethod]
        public void testAddition_CrossUnit_GallonPlusLitre()
        {
            var gallon = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
            var litreEquivalent = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

            var result = gallon.Add(litreEquivalent);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Gallon, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition with an explicitly requested Litre target unit.
        ///
        /// PURPOSE:
        /// Confirms explicit target-unit overload overrides default first-operand behavior.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Litre()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            var result = litre.Add(ml, VolumeUnit.Litre);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition with an explicitly requested Millilitre target unit.
        ///
        /// PURPOSE:
        /// Confirms explicit result-unit selection is honored by the generic add logic.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Millilitre()
        {
            var litre = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var ml = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            var result = litre.Add(ml, VolumeUnit.Millilitre);

            Assert.AreEqual(2000.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition with an explicitly requested Gallon target unit.
        ///
        /// PURPOSE:
        /// Confirms output can be expressed in any supported volume unit after summation.
        /// </summary>
        [TestMethod]
        public void testAddition_ExplicitTargetUnit_Gallon()
        {
            var v1 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

            var result = v1.Add(v2, VolumeUnit.Gallon);

            Assert.AreEqual(2.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Gallon, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition is commutative when expressed in the same explicit target unit.
        ///
        /// MATHEMATICAL PROPERTY:
        /// A + B must equal B + A.
        ///
        /// NOTE:
        /// Explicit target unit is used here so both results are directly comparable.
        /// </summary>
        [TestMethod]
        public void testAddition_Commutativity_WithExplicitTargetUnit()
        {
            var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            var result1 = a.Add(b, VolumeUnit.Litre);
            var result2 = b.Add(a, VolumeUnit.Litre);

            Assert.AreEqual(result1.GetValue(), result2.GetValue(), EPSILON);
            Assert.AreEqual(result1.GetUnit(), result2.GetUnit());
        }

        /// <summary>
        /// Verifies zero acts as an additive identity.
        ///
        /// MATHEMATICAL PROPERTY:
        /// A + 0 = A.
        /// </summary>
        [TestMethod]
        public void testAddition_WithZero()
        {
            var v1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var zero = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre);

            var result = v1.Add(zero);

            Assert.AreEqual(5.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition works correctly when one operand is negative.
        ///
        /// PURPOSE:
        /// Confirms arithmetic correctness for signed values.
        /// </summary>
        [TestMethod]
        public void testAddition_NegativeValues()
        {
            var v1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(-2000.0, VolumeUnit.Millilitre);

            var result = v1.Add(v2);

            Assert.AreEqual(3.0, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition remains stable for large values.
        ///
        /// PURPOSE:
        /// Ensures arithmetic scales correctly for high-magnitude quantities.
        /// </summary>
        [TestMethod]
        public void testAddition_LargeValues()
        {
            var v1 = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(2e6, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        /// <summary>
        /// Verifies addition remains accurate for small fractional values.
        ///
        /// PURPOSE:
        /// Confirms arithmetic precision for low-magnitude volume quantities.
        /// </summary>
        [TestMethod]
        public void testAddition_SmallValues()
        {
            var v1 = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(0.002, VolumeUnit.Litre);

            var result = v1.Add(v2);

            Assert.AreEqual(0.003, result.GetValue(), EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.GetUnit());
        }

        // =========================================================
        // 5. Architectural / Integration Confidence Tests
        // =========================================================

        /// <summary>
        /// Verifies that equality, conversion, and addition behave consistently
        /// for the new VolumeUnit category through the generic Quantity<U> API.
        ///
        /// ARCHITECTURAL PURPOSE:
        /// Confirms no special-case logic is needed in Quantity<U> for volume support.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_VolumeOperations_Consistency()
        {
            var v1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var v2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

            var equality = v1.Equals(v2);
            var conversion = v1.ConvertTo(VolumeUnit.Millilitre);
            var addition = v1.Add(v2, VolumeUnit.Litre);

            Assert.IsTrue(equality);
            Assert.AreEqual(1000.0, conversion.GetValue(), EPSILON);
            Assert.AreEqual(2.0, addition.GetValue(), EPSILON);
        }

        /// <summary>
        /// Verifies volume integrates into the existing architecture without
        /// affecting type isolation between categories.
        ///
        /// ARCHITECTURAL PURPOSE:
        /// - Volume works with the existing generic Quantity<U> pipeline
        /// - Volume remains incompatible with Length and Weight
        /// - No changes are required to existing generic logic
        /// </summary>
        [TestMethod]
        public void testScalability_VolumeIntegration()
        {
            var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(volume.Equals(length));
            Assert.IsFalse(volume.Equals(weight));

            var converted = volume.ConvertTo(VolumeUnit.Millilitre);
            var sum = volume.Add(new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre));

            Assert.AreEqual(1000.0, converted.GetValue(), EPSILON);
            Assert.AreEqual(2.0, sum.GetValue(), EPSILON);
        }
    }
}