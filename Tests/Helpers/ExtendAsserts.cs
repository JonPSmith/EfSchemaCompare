#region licence
// =====================================================
// EfSchemeCompare Project - project to compare EF schema to SQL schema
// Filename: ExtendAsserts.cs
// Date Created: 2015/10/31
// © Copyright Selective Analytics 2015. All rights reserved
// =====================================================
#endregion

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GenericLibsBase;
using NUnit.Framework;

namespace Tests.Helpers
{
    internal static class ExtendAsserts
    {
        internal static void ShouldBeValid(this ISuccessOrErrors status, bool isValid = true)
        {
            Assert.AreEqual(isValid, status.IsValid, string.Join("\n", status.Errors));
        }

        internal static void ShouldBeValid<T>(this ISuccessOrErrors<T> status, bool isValid = true)
        {
            Assert.AreEqual(isValid, status.IsValid, string.Join("\n", status.Errors));
        }

        internal static void ShouldEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldStartWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.StartsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEndWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.EndsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldContain(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.Contains(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldNotEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.True(expectedValue != actualValue, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this float actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this long actualValue, long expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this double actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this decimal actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, (double)actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this int actualValue, int expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqual<T>( this T actualValue, T expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<string> errorMessages)
        {
            Assert.AreEqual(expectedValue, actualValue,  string.Join("\n", errorMessages));
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<ValidationResult> validationResults)
        {
            Assert.AreEqual(expectedValue, actualValue, string.Join("\n", validationResults.Select( x => x.ErrorMessage)));
        }

        internal static void ShouldNotEqual<T>(this T actualValue, T unexpectedValue, string errorMessage = null)
        {
            Assert.AreNotEqual(unexpectedValue, actualValue);
        }

        internal static void ShouldNotEqualNull<T>(this T actualValue, string errorMessage = null) where T : class
        {
            Assert.NotNull( actualValue);
        }

        internal static void ShouldBeGreaterThan(this int actualValue, int greaterThanThis, string errorMessage = null)
        {
            Assert.Greater(actualValue, greaterThanThis);
        }

        internal static void ShouldBeLessThan(this int actualValue, int greaterThanThis, string errorMessage = null)
        {
            Assert.Less(actualValue, greaterThanThis);
        }

        internal static void IsA<T>(this object actualValue, string errorMessage = null) where T : class
        {
            Assert.True(actualValue.GetType() == typeof(T), "expected type {0}, but was of type {1}", typeof(T).Name, actualValue.GetType().Name);
        }
    }
}
