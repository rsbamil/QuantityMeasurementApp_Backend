using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Enums;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// UC13 comprehensive regression and refactoring validation test suite
    /// for centralized arithmetic logic in Quantity<U>.
    ///
    /// PURPOSE:
    /// UC13 refactors arithmetic internals to remove duplication across:
    /// - Add
    /// - Subtract
    /// - Divide
    ///
    /// This suite verifies that:
    /// 1. Public behavior from UC12 remains unchanged
    /// 2. Validation behavior is consistent across operations
    /// 3. Arithmetic continues to work for all supported categories
    /// 4. Immutability remains preserved
    /// 5. Refactoring did not introduce regressions
    ///
    /// IMPORTANT TESTING NOTE:
    /// Private helper methods introduced in UC13 are intentionally not tested directly.
    /// Instead, they are validated through public API behavior, which is the preferred
    /// and maintainable industrial testing strategy.
    /// </summary>
    [TestClass]
    public class QuantityArithmeticTests
    {
        /// <summary>
        /// Floating-point tolerance used across all tests.
        /// </summary>
        private const double EPSILON = 1e-6;

        // =========================================================
        // BEHAVIOR PRESERVATION - ADDITION
        // =========================================================

        /// <summary>
        /// Verifies that addition behavior from UC12 remains unchanged
        /// after internal UC13 refactoring.
        ///
        /// SCENARIO:
        /// 1 Foot + 12 Inches = 2 Feet
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved_ImplicitTarget_Length()
        {
            var q1 = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            var result = q1.Add(q2);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        /// <summary>
        /// Verifies that explicit target-unit addition remains unchanged
        /// after UC13 refactoring.
        ///
        /// SCENARIO:
        /// 10 Kilogram + 5000 Gram = 15000 Gram
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved_ExplicitTarget_Weight()
        {
            var q1 = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram);
            var q2 = new Quantity<WeightUnit>(5000.0, WeightUnit.Gram);

            var result = q1.Add(q2, WeightUnit.Gram);

            Assert.AreEqual(15000.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.Gram, result.Unit);
        }

        /// <summary>
        /// Verifies that volume addition continues to work through the same
        /// centralized arithmetic path.
        /// </summary>
        [TestMethod]
        public void testAdd_UC12_BehaviorPreserved_Volume()
        {
            var q1 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);
            var q2 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);

            var result = q1.Add(q2);

            Assert.AreEqual(2.5, result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
        }

        // =========================================================
        // BEHAVIOR PRESERVATION - SUBTRACTION
        // =========================================================

        /// <summary>
        /// Verifies that implicit-target subtraction remains unchanged
        /// after UC13 refactoring.
        ///
        /// SCENARIO:
        /// 10 Feet - 6 Inches = 9.5 Feet
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_ImplicitTarget_Length()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(6.0, LengthUnit.Inch);

            var result = q1.Subtract(q2);

            Assert.AreEqual(9.5, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        /// <summary>
        /// Verifies that explicit-target subtraction remains unchanged
        /// after UC13 refactoring.
        ///
        /// SCENARIO:
        /// 5 Litre - 2 Litre = 3000 Millilitre
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_ExplicitTarget_Volume()
        {
            var q1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var q2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

            var result = q1.Subtract(q2, VolumeUnit.Millilitre);

            Assert.AreEqual(3000.0, result.Value, EPSILON);
            Assert.AreEqual(VolumeUnit.Millilitre, result.Unit);
        }

        /// <summary>
        /// Verifies negative subtraction behavior remains preserved.
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_NegativeResult()
        {
            var q1 = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);

            var result = q1.Subtract(q2);

            Assert.AreEqual(-5.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        /// <summary>
        /// Verifies zero-result subtraction remains preserved.
        /// </summary>
        [TestMethod]
        public void testSubtract_UC12_BehaviorPreserved_ZeroResult()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(120.0, LengthUnit.Inch);

            var result = q1.Subtract(q2);

            Assert.AreEqual(0.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        // =========================================================
        // BEHAVIOR PRESERVATION - DIVISION
        // =========================================================

        /// <summary>
        /// Verifies same-unit division behavior remains unchanged.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_SameUnit()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

            double result = q1.Divide(q2);

            Assert.AreEqual(5.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies cross-unit division behavior remains unchanged.
        ///
        /// SCENARIO:
        /// 24 Inches / 2 Feet = 1
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_CrossUnit()
        {
            var q1 = new Quantity<LengthUnit>(24.0, LengthUnit.Inch);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

            double result = q1.Divide(q2);

            Assert.AreEqual(1.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies fractional ratio remains unchanged after refactoring.
        /// </summary>
        [TestMethod]
        public void testDivide_UC12_BehaviorPreserved_FractionalRatio()
        {
            var q1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var q2 = new Quantity<VolumeUnit>(10.0, VolumeUnit.Litre);

            double result = q1.Divide(q2);

            Assert.AreEqual(0.5, result, EPSILON);
        }
        // =========================================================
        // TARGET UNIT BEHAVIOR
        // =========================================================

        /// <summary>
        /// Verifies implicit target-unit behavior remains correct for addition.
        ///
        /// EXPECTATION:
        /// Result uses first operand's unit when target unit is not explicitly supplied.
        /// </summary>
        [TestMethod]
        public void testImplicitTargetUnit_Add_UsesFirstOperandUnit()
        {
            var q1 = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram);
            var q2 = new Quantity<WeightUnit>(500.0, WeightUnit.Gram);

            var result = q1.Add(q2);

            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
        }

        /// <summary>
        /// Verifies implicit target-unit behavior remains correct for subtraction.
        /// </summary>
        [TestMethod]
        public void testImplicitTargetUnit_Subtract_UsesFirstOperandUnit()
        {
            var q1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var q2 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);

            var result = q1.Subtract(q2);

            Assert.AreEqual(VolumeUnit.Litre, result.Unit);
        }

        /// <summary>
        /// Verifies explicit target unit overrides implicit unit selection.
        /// </summary>
        [TestMethod]
        public void testExplicitTargetUnit_AddSubtract_OverridesFirstOperandUnit()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(6.0, LengthUnit.Inch);

            var addResult = q1.Add(q2, LengthUnit.Inch);
            var subtractResult = q1.Subtract(q2, LengthUnit.Inch);

            Assert.AreEqual(LengthUnit.Inch, addResult.Unit);
            Assert.AreEqual(LengthUnit.Inch, subtractResult.Unit);
        }

        // =========================================================
        // ROUNDING CONSISTENCY
        // =========================================================

        /// <summary>
        /// Verifies centralized arithmetic preserves two-decimal rounding
        /// for addition and subtraction results.
        /// </summary>
        [TestMethod]
        public void testRounding_AddSubtract_TwoDecimalPlaces()
        {
            var q1 = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.Inch);

            var addResult = q1.Add(q2, LengthUnit.Feet);
            var subtractResult = q1.Subtract(q2, LengthUnit.Feet);

            // Addition behavior is preserved from previous use cases:
            // it should not force rounding.
            Assert.AreEqual(1.1666666666666667, addResult.Value, 1e-9);

            // Subtraction in UC12/UC13 is rounded to two decimal places.
            Assert.AreEqual(0.83, subtractResult.Value, EPSILON);
        }

        /// <summary>
        /// Verifies division remains unrounded and returns raw scalar precision.
        /// </summary>
        [TestMethod]
        public void testRounding_Divide_NoRounding()
        {
            var q1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var q2 = new Quantity<WeightUnit>(3.0, WeightUnit.Gram);

            double result = q1.Divide(q2);

            Assert.AreEqual(333.3333333333333, result, 1e-9);
        }

        // =========================================================
        // IMMUTABILITY PRESERVATION
        // =========================================================

        /// <summary>
        /// Verifies original quantities remain unchanged after addition.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterAdd_ViaCentralizedHelper()
        {
            var original1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var original2 = new Quantity<LengthUnit>(6.0, LengthUnit.Inch);

            var result = original1.Add(original2);

            Assert.AreEqual(10.0, original1.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, original1.Unit);

            Assert.AreEqual(6.0, original2.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Inch, original2.Unit);

            Assert.AreNotSame(original1, result);
        }

        /// <summary>
        /// Verifies original quantities remain unchanged after subtraction.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterSubtract_ViaCentralizedHelper()
        {
            var original1 = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram);
            var original2 = new Quantity<WeightUnit>(5000.0, WeightUnit.Gram);

            var result = original1.Subtract(original2);

            Assert.AreEqual(10.0, original1.Value, EPSILON);
            Assert.AreEqual(5000.0, original2.Value, EPSILON);
            Assert.AreNotSame(original1, result);
        }

        /// <summary>
        /// Verifies original quantities remain unchanged after division.
        /// </summary>
        [TestMethod]
        public void testImmutability_AfterDivide_ViaCentralizedHelper()
        {
            var original1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
            var original2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

            double ratio = original1.Divide(original2);

            Assert.AreEqual(5.0, original1.Value, EPSILON);
            Assert.AreEqual(2.0, original2.Value, EPSILON);
            Assert.AreEqual(2.5, ratio, EPSILON);
        }

        // =========================================================
        // NON-COMMUTATIVE PROPERTY PRESERVATION
        // =========================================================

        /// <summary>
        /// Verifies subtraction remains non-commutative after refactoring.
        /// </summary>
        [TestMethod]
        public void testSubtract_NonCommutative_Preserved()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);

            var result1 = a.Subtract(b);
            var result2 = b.Subtract(a);

            Assert.AreEqual(5.0, result1.Value, EPSILON);
            Assert.AreEqual(-5.0, result2.Value, EPSILON);
        }

        /// <summary>
        /// Verifies division remains non-commutative after refactoring.
        /// </summary>
        [TestMethod]
        public void testDivide_NonCommutative_Preserved()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);

            double result1 = a.Divide(b);
            double result2 = b.Divide(a);

            Assert.AreEqual(2.0, result1, EPSILON);
            Assert.AreEqual(0.5, result2, EPSILON);
        }

        // =========================================================
        // ALL CATEGORIES VALIDATION
        // =========================================================

        /// <summary>
        /// Verifies that all three arithmetic operations continue to work
        /// across all supported categories after UC13 refactoring.
        /// </summary>
        [TestMethod]
        public void testAllOperations_AcrossAllCategories()
        {
            // Length
            var lengthAdd = new Quantity<LengthUnit>(1.0, LengthUnit.Feet)
                .Add(new Quantity<LengthUnit>(12.0, LengthUnit.Inch));

            var lengthSubtract = new Quantity<LengthUnit>(10.0, LengthUnit.Feet)
                .Subtract(new Quantity<LengthUnit>(6.0, LengthUnit.Inch));

            double lengthDivide = new Quantity<LengthUnit>(24.0, LengthUnit.Inch)
                .Divide(new Quantity<LengthUnit>(2.0, LengthUnit.Feet));

            // Weight
            var weightAdd = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram)
                .Add(new Quantity<WeightUnit>(5000.0, WeightUnit.Gram));

            var weightSubtract = new Quantity<WeightUnit>(10.0, WeightUnit.Kilogram)
                .Subtract(new Quantity<WeightUnit>(5000.0, WeightUnit.Gram));

            double weightDivide = new Quantity<WeightUnit>(2000.0, WeightUnit.Gram)
                .Divide(new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram));

            // Volume
            var volumeAdd = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre)
                .Add(new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre));

            var volumeSubtract = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre)
                .Subtract(new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre));

            double volumeDivide = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre)
                .Divide(new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre));

            Assert.AreEqual(2.0, lengthAdd.Value, EPSILON);
            Assert.AreEqual(9.5, lengthSubtract.Value, EPSILON);
            Assert.AreEqual(1.0, lengthDivide, EPSILON);

            Assert.AreEqual(15.0, weightAdd.Value, EPSILON);
            Assert.AreEqual(5.0, weightSubtract.Value, EPSILON);
            Assert.AreEqual(2.0, weightDivide, EPSILON);

            Assert.AreEqual(2.5, volumeAdd.Value, EPSILON);
            Assert.AreEqual(4.5, volumeSubtract.Value, EPSILON);
            Assert.AreEqual(1.0, volumeDivide, EPSILON);
        }

        // =========================================================
        // INTEGRATION / CHAINING
        // =========================================================

        /// <summary>
        /// Verifies that chained operations continue to work after refactoring.
        ///
        /// SCENARIO:
        /// (10 ft + 2 ft - 4 ft) / 4 ft = 2
        /// </summary>
        [TestMethod]
        public void testArithmetic_Chain_Operations()
        {
            var q1 = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);
            var q3 = new Quantity<LengthUnit>(4.0, LengthUnit.Feet);
            var q4 = new Quantity<LengthUnit>(4.0, LengthUnit.Feet);

            double result = q1.Add(q2).Subtract(q3).Divide(q4);

            Assert.AreEqual(2.0, result, EPSILON);
        }

        /// <summary>
        /// Verifies inverse arithmetic relationship still works:
        /// A + B - B ~= A
        /// </summary>
        [TestMethod]
        public void testArithmetic_AddSubtract_InverseRelationship()
        {
            var a = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(6.0, LengthUnit.Inch);

            var result = a.Add(b).Subtract(b);

            Assert.AreEqual(a.Value, result.Value, EPSILON);
            Assert.AreEqual(a.Unit, result.Unit);
        }
    }
}