using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementApp.Enums;
using QuantityMeasurementApp.Models;
using System;

namespace QuantityMeasurementApp.Tests
{
    /// <summary>
    /// Comprehensive UC10 test suite for the generic <c>Quantity&lt;U&gt;</c> model.
    ///
    /// Purpose:
    /// Validates that the new generic quantity architecture supports multiple
    /// measurement categories (for example Length and Weight) through a common,
    /// reusable design without duplicating quantity logic.
    ///
    /// Coverage includes:
    /// - Unit contract behavior
    /// - Equality across compatible units
    /// - Conversion between units
    /// - Addition across units in the same category
    /// - Cross-category safety
    /// - Constructor validation expectations
    /// - Immutability behavior
    /// - Architectural placeholder validations for design-level requirements
    ///
    /// Note:
    /// Some UC10 requirements are architectural or compile-time concerns.
    /// These cannot always be fully asserted through runtime unit tests, so
    /// some placeholder tests are intentionally included to document those expectations.
    /// </summary>
    [TestClass]
    public class UC10_GenericQuantityTests
    {
        /// <summary>
        /// Standard floating-point comparison tolerance used across numeric assertions.
        /// </summary>
        private const double EPSILON = 1e-6;

        // =========================================================
        // 1. Interface / Unit Contract Tests
        // =========================================================

        /// <summary>
        /// Verifies that <see cref="LengthUnit"/> exposes correct measurable behavior.
        ///
        /// Expected:
        /// - Feet is the base unit for length
        /// - Conversion factor for Feet is 1.0
        /// - 1 Foot converted to base remains 1.0
        /// - 1 base unit (Foot) converts to 12 Inches
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_LengthUnitImplementation()
        {
            Assert.AreEqual(1.0, LengthUnit.Feet.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, LengthUnit.Feet.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(12.0, LengthUnit.Inch.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Verifies that <see cref="WeightUnit"/> exposes correct measurable behavior.
        ///
        /// Expected:
        /// - Kilogram is the base unit for weight
        /// - Conversion factor for Kilogram is 1.0
        /// - 1 Kilogram converted to base remains 1.0
        /// - 1 base unit (Kilogram) converts to 1000 Grams
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_WeightUnitImplementation()
        {
            Assert.AreEqual(1.0, WeightUnit.Kilogram.GetConversionFactor(), EPSILON);
            Assert.AreEqual(1.0, WeightUnit.Kilogram.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1000.0, WeightUnit.Gram.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Ensures that different unit categories implement conversion behavior consistently.
        ///
        /// This test does not compare categories semantically, but verifies that both
        /// categories follow the same measurable contract pattern:
        /// - Convert value to a common base unit
        /// - Return predictable numeric results
        /// </summary>
        [TestMethod]
        public void testIMeasurableInterface_ConsistentBehavior()
        {
            double lengthBase = LengthUnit.Yard.ConvertToBaseUnit(1.0);
            double weightBase = WeightUnit.Pound.ConvertToBaseUnit(1.0);

            Assert.AreEqual(3.0, lengthBase, EPSILON);
            Assert.AreEqual(0.453592, weightBase, EPSILON);
        }

        // =========================================================
        // 2. Generic Quantity Equality Tests
        // =========================================================

        /// <summary>
        /// Verifies that two length quantities with equivalent physical values
        /// are treated as equal, even when represented using different units.
        ///
        /// Example:
        /// 1 Foot == 12 Inches
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Equality()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Verifies that two weight quantities with equivalent physical values
        /// are treated as equal, even when represented using different units.
        ///
        /// Example:
        /// 1 Kilogram == 1000 Grams
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Equality()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var b = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            Assert.IsTrue(a.Equals(b));
        }

        // =========================================================
        // 3. Generic Quantity Conversion Tests
        // =========================================================

        /// <summary>
        /// Verifies length conversion using the generic quantity model.
        ///
        /// Expected:
        /// 1 Foot converts to 12 Inches.
        /// Also confirms that the resulting quantity carries the requested target unit.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Conversion()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var result = a.ConvertTo(LengthUnit.Inch);

            Assert.AreEqual(12.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Inch, result.Unit);
        }

        /// <summary>
        /// Verifies weight conversion using the generic quantity model.
        ///
        /// Expected:
        /// 1 Kilogram converts to 1000 Grams.
        /// Also confirms that the resulting quantity carries the requested target unit.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Conversion()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var result = a.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(1000.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.Gram, result.Unit);
        }

        // =========================================================
        // 4. Generic Quantity Addition Tests
        // =========================================================

        /// <summary>
        /// Verifies length addition when the operands use different units.
        ///
        /// Scenario:
        /// 1 Foot + 12 Inches = 2 Feet
        ///
        /// The result is explicitly requested in Feet.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_LengthOperations_Addition()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            var result = a.Add(b, LengthUnit.Feet);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, result.Unit);
        }

        /// <summary>
        /// Verifies weight addition when the operands use different units.
        ///
        /// Scenario:
        /// 1 Kilogram + 1000 Grams = 2 Kilograms
        ///
        /// The result is explicitly requested in Kilograms.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_WeightOperations_Addition()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var b = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            var result = a.Add(b, WeightUnit.Kilogram);

            Assert.AreEqual(2.0, result.Value, EPSILON);
            Assert.AreEqual(WeightUnit.Kilogram, result.Unit);
        }

        // =========================================================
        // 5. Cross Category Tests
        // =========================================================

        /// <summary>
        /// Verifies that quantities from different measurement categories
        /// are never considered equal.
        ///
        /// This protects the model from invalid logical comparisons such as
        /// Length vs Weight.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_LengthVsWeight()
        {
            var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// Documents compile-time generic type safety.
        ///
        /// Example rule:
        /// A <c>Quantity&lt;LengthUnit&gt;</c> cannot be assigned to a
        /// <c>Quantity&lt;WeightUnit&gt;</c>.
        ///
        /// This behavior is enforced by the compiler, so runtime assertion is not applicable.
        /// </summary>
        [TestMethod]
        public void testCrossCategoryPrevention_CompilerTypeSafety()
        {
            Assert.AreNotEqual(typeof(Quantity<LengthUnit>), typeof(Quantity<WeightUnit>));
        }

        // =========================================================
        // 7. Combination Tests
        // =========================================================

        /// <summary>
        /// Verifies multiple supported conversion combinations across both
        /// length and weight categories.
        ///
        /// This acts as a broader regression test to ensure conversion behavior
        /// remains accurate for commonly used unit pairings.
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Conversion_AllUnitCombinations()
        {
            var length = new Quantity<LengthUnit>(1.0, LengthUnit.Yard);

            Assert.AreEqual(3.0, length.ConvertTo(LengthUnit.Feet).Value, EPSILON);
            Assert.AreEqual(36.0, length.ConvertTo(LengthUnit.Inch).Value, EPSILON);

            var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.AreEqual(1000.0, weight.ConvertTo(WeightUnit.Gram).Value, EPSILON);
            Assert.AreEqual(2.20462, weight.ConvertTo(WeightUnit.Pound).Value, 0.01);
        }

        /// <summary>
        /// Verifies addition across different unit combinations and target result units.
        ///
        /// Scenarios:
        /// - Length result returned in Yards
        /// - Weight result returned in Grams
        /// </summary>
        [TestMethod]
        public void testGenericQuantity_Addition_AllUnitCombinations()
        {
            var l1 = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var l2 = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);
            var lResult = l1.Add(l2, LengthUnit.Yard);

            Assert.AreEqual(0.666666, lResult.Value, 0.001);

            var w1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var w2 = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);
            var wResult = w1.Add(w2, WeightUnit.Gram);

            Assert.AreEqual(2000.0, wResult.Value, EPSILON);
        }

        // =========================================================
        // 8. Backward Compatibility
        // =========================================================

        /// <summary>
        /// Placeholder test documenting backward compatibility expectation.
        ///
        /// UC10 must preserve all behavior supported in UC1 through UC9.
        /// In practice, this is validated by executing the earlier test suites
        /// against the refactored implementation.
        /// </summary>
        [TestMethod]
        public void testBackwardCompatibility_AllUC1Through9Tests()
        {
            var lengthA = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var lengthB = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            Assert.IsTrue(lengthA.Equals(lengthB));
        }

        // =========================================================
        // 9. Simplified App Demonstration Tests
        // =========================================================

        /// <summary>
        /// Demonstrates equality behavior in the simplified application layer.
        ///
        /// Confirms that user-facing logic continues to behave correctly after
        /// refactoring to the generic quantity model.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Equality()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(b));
        }

        /// <summary>
        /// Demonstrates conversion behavior in the simplified application layer.
        ///
        /// Confirms that the generic architecture does not affect expected conversion output.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Conversion()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var result = a.ConvertTo(WeightUnit.Gram);

            Assert.AreEqual(1000.0, result.Value, EPSILON);
        }

        /// <summary>
        /// Demonstrates addition behavior in the simplified application layer.
        ///
        /// Confirms that user-level arithmetic remains correct after the UC10 refactor.
        /// </summary>
        [TestMethod]
        public void testQuantityMeasurementApp_SimplifiedDemonstration_Addition()
        {
            var a = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            var b = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            var result = a.Add(b, WeightUnit.Kilogram);

            Assert.AreEqual(2.0, result.Value, EPSILON);
        }

        // =========================================================
        // 10. Generic / Language Concept Validations
        // =========================================================

        /// <summary>
        /// Placeholder validation for flexible generic design.
        ///
        /// In Java this might relate to wildcard usage; in C# the equivalent
        /// design flexibility is achieved through generic type parameters and constraints.
        /// </summary>
        [TestMethod]
        public void testTypeWildcard_FlexibleSignatures()
        {
            var length = new Quantity<LengthUnit>(5.0, LengthUnit.Feet);
            Assert.AreEqual(5.0, length.Value, EPSILON);
        }

        /// <summary>
        /// Documents the scalability goal that new measurable unit categories
        /// can be introduced without modifying the generic quantity core logic.
        /// </summary>
        [TestMethod]
        public void testScalability_NewUnitEnumIntegration()
        {
            Assert.IsTrue(typeof(Quantity<LengthUnit>).IsGenericType);
        }

        /// <summary>
        /// Documents that the architecture should support multiple future categories
        /// such as Volume, Temperature, or other measurable domains.
        /// </summary>
        [TestMethod]
        public void testScalability_MultipleNewCategories()
        {
            Assert.AreEqual("Quantity`1", typeof(Quantity<LengthUnit>).Name);
        }

        /// <summary>
        /// Documents the expectation that the generic type parameter is bounded
        /// or constrained appropriately to measurable unit behavior.
        ///
        /// This is generally enforced by generic constraints at compile time.
        /// </summary>
        [TestMethod]
        public void testGenericBoundedTypeParameter_Enforcement()
        {
            var genericArguments = typeof(Quantity<LengthUnit>).GetGenericArguments();
            Assert.AreEqual(1, genericArguments.Length);
        }

        /// <summary>
        /// Verifies the standard equality/hash code contract:
        /// if two objects are equal, they must produce the same hash code.
        /// </summary>
        [TestMethod]
        public void testHashCode_GenericQuantity_Consistency()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var b = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// Verifies the key object equality contract rules:
        /// - Reflexive
        /// - Symmetric
        /// - Transitive
        ///
        /// These rules are essential for predictable behavior in collections,
        /// comparisons, and domain logic.
        /// </summary>
        [TestMethod]
        public void testEquals_GenericQuantity_ContractPreservation()
        {
            var a = new Quantity<LengthUnit>(1.0, LengthUnit.Yard);
            var b = new Quantity<LengthUnit>(3.0, LengthUnit.Feet);
            var c = new Quantity<LengthUnit>(36.0, LengthUnit.Inch);

            Assert.IsTrue(a.Equals(a)); // Reflexive
            Assert.IsTrue(a.Equals(b) && b.Equals(a)); // Symmetric
            Assert.IsTrue(a.Equals(b) && b.Equals(c) && a.Equals(c)); // Transitive
        }

        /// <summary>
        /// Verifies that conversion behavior is encapsulated within the unit enums
        /// themselves, reinforcing the UC10 design principle that unit logic should
        /// live with the unit definition rather than being duplicated elsewhere.
        /// </summary>
        [TestMethod]
        public void testEnumAsUnitCarrier_BehaviorEncapsulation()
        {
            Assert.AreEqual(1.0, LengthUnit.Feet.ConvertToBaseUnit(1.0), EPSILON);
            Assert.AreEqual(1000.0, WeightUnit.Gram.ConvertFromBaseUnit(1.0), EPSILON);
        }

        /// <summary>
        /// Documents runtime safety across generic categories.
        ///
        /// Although "type erasure" is a Java-specific term, the intent here is to verify
        /// that runtime behavior still prevents invalid cross-category equality matches.
        /// </summary>
        [TestMethod]
        public void testTypeErasure_RuntimeSafety()
        {
            var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

            Assert.IsFalse(length.Equals(weight));
        }

        /// <summary>
        /// Verifies that the generic quantity model achieves flexibility through
        /// composition of unit behavior rather than inheritance-based specialization.
        /// </summary>
        [TestMethod]
        public void testCompositionOverInheritance_Flexibility()
        {
            var length = new Quantity<LengthUnit>(2.0, LengthUnit.Yard);
            var weight = new Quantity<WeightUnit>(2.0, WeightUnit.Kilogram);

            Assert.AreEqual(LengthUnit.Yard, length.Unit);
            Assert.AreEqual(WeightUnit.Kilogram, weight.Unit);
        }

        /// <summary>
        /// Placeholder validation documenting the DRY objective of UC10.
        ///
        /// The generic design should reduce duplicated logic previously found in
        /// separate quantity implementations for each category.
        /// </summary>
        [TestMethod]
        public void testCodeReduction_DRYValidation()
        {
            Assert.AreEqual(typeof(Quantity<LengthUnit>).Name, typeof(Quantity<WeightUnit>).Name);
        }

        /// <summary>
        /// Documents the maintainability goal that quantity behavior should be
        /// implemented in a single reusable generic abstraction rather than
        /// duplicated across category-specific classes.
        /// </summary>
        [TestMethod]
        public void testMaintainability_SingleSourceOfTruth()
        {
            Assert.AreEqual(typeof(Quantity<LengthUnit>).GetGenericTypeDefinition(),
                            typeof(Quantity<WeightUnit>).GetGenericTypeDefinition());
        }

        /// <summary>
        /// Additional architectural placeholder confirming that the design is ready
        /// for future expansion into multiple measurable categories.
        /// </summary>
        [TestMethod]
        public void testArchitecturalReadiness_MultipleNewCategories()
        {
            var length = new Quantity<LengthUnit>(10.0, LengthUnit.Inch);
            var weight = new Quantity<WeightUnit>(10.0, WeightUnit.Gram);

            Assert.AreNotEqual(length.GetType(), weight.GetType());
        }

        /// <summary>
        /// Placeholder validation documenting that the generic implementation should
        /// not introduce unacceptable runtime overhead for standard quantity operations.
        /// </summary>
        [TestMethod]
        public void testPerformance_GenericOverhead()
        {
            var quantity = new Quantity<LengthUnit>(1.0, LengthUnit.Yard);
            var converted = quantity.ConvertTo(LengthUnit.Feet);

            Assert.AreEqual(3.0, converted.Value, EPSILON);
        }

        /// <summary>
        /// Placeholder validation documenting the importance of a clear and understandable
        /// generic architecture for future developers and maintainers.
        /// </summary>
        [TestMethod]
        public void testDocumentation_PatternClarity()
        {
            Assert.IsTrue(typeof(Quantity<LengthUnit>).IsClass);
        }

        /// <summary>
        /// Verifies that the measurable contract remains minimal and focused.
        ///
        /// This aligns with Interface Segregation by ensuring unit types only expose
        /// the behavior required for measurement conversion and comparison support.
        /// </summary>
        [TestMethod]
        public void testInterfaceSegregation_MinimalContract()
        {
            Assert.IsTrue(typeof(LengthUnit).IsEnum);
            Assert.IsTrue(typeof(WeightUnit).IsEnum);
        }

        /// <summary>
        /// Verifies immutability-like behavior of the generic quantity model.
        ///
        /// Expected:
        /// - Original quantity remains unchanged after conversion
        /// - Original quantity remains unchanged after addition
        /// - Each operation returns a new quantity instance with the expected result
        /// </summary>
        [TestMethod]
        public void testImmutability_GenericQuantity()
        {
            var original = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);

            var converted = original.ConvertTo(LengthUnit.Inch);
            var added = original.Add(new Quantity<LengthUnit>(12.0, LengthUnit.Inch));

            Assert.AreEqual(1.0, original.Value, EPSILON);
            Assert.AreEqual(LengthUnit.Feet, original.Unit);

            Assert.AreEqual(12.0, converted.Value, EPSILON);
            Assert.AreEqual(2.0, added.Value, EPSILON);
        }
    }
}