﻿using DebtAnalyzer.ClassDebt;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DebtAnalyzer.Test
{
	[TestClass]
	public class TestTypeLengthAnalyzer : CodeFixVerifier
	{
		public TestTypeLengthAnalyzer()
		{
			TypeLengthAnalyzer.DefaultMaximumTypeLength = 10;
		}

		static string LongType => @"
using System;

namespace ConsoleApplication1
{
    class LongMethodClass
    {










    }
}";

		static string LongTypeFixed => @"
using System;
using DebtAnalyzer;

namespace ConsoleApplication1
{
    [DebtType(LineCount = 13, FieldCount = 0)]
    class LongMethodClass
    {










    }
}";

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new TypeDebtAnalyzer();
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new TypeDebtAnnotationProvider();
		}

		[TestMethod]
		public void TestDiagnostic()
		{
			var test = LongType;
			var expected = new DiagnosticResult
			{
				Id = "TypeLengthAnalyzer",
				Message = "Type LongMethodClass is 13 lines long while it should not be longer than 10 lines.",
				Severity = DiagnosticSeverity.Info,
				Locations =
					new[]
					{
						new DiagnosticResultLocation("Test0.cs", 6, 5)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TestFix()
		{
			VerifyCSharpFix(LongType, LongTypeFixed, allowNewCompilerDiagnostics: true);
		}
	}
}