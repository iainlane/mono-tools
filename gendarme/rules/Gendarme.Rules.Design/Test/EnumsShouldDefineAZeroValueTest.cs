// 
// Unit tests for EnumsShouldDefineAZeroValueRule
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2007-2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Reflection;

using Mono.Cecil;

using Gendarme.Framework;
using Gendarme.Rules.Design;

using NUnit.Framework;
using Test.Rules.Helpers;

namespace Test.Rules.Design {

	enum PrivateEnumWithZeroValue {
		Zero,
		One,
		Two
	}

	internal enum InternalEnumWithoutZeroValue {
		One = 1,
		Two,
		Three
	}

	[TestFixture]
	public class EnumsShouldDefineAZeroValueTest {

		public enum NestedPublicEnumWithZeroValue {
			Zero
		}

		[Flags]
		private enum NestedInternalFlagsWithZeroValue {
			GhostBit = 0,
			FirstBit,
		}

		[Flags]
		private enum NestedPrivateFlagsWithoutZeroValue {
			FirstBit = 1,
			SecondBit = 2,
			ThirdBit = 4
		}

		private ITypeRule rule;
		private AssemblyDefinition assembly;
		private TestRunner runner;

		[TestFixtureSetUp]
		public void FixtureSetUp ()
		{
			string unit = Assembly.GetExecutingAssembly ().Location;
			assembly = AssemblyFactory.GetAssembly (unit);
			rule = new EnumsShouldDefineAZeroValueRule ();
			runner = new TestRunner (rule);
		}

		private TypeDefinition GetTest (string name)
		{
			string fullname = "Test.Rules.Design." + name;
			return assembly.MainModule.Types [fullname];
		}

		[Test]
		public void NotAnEnumType ()
		{
			TypeDefinition type = GetTest ("EnumsShouldDefineAZeroValueTest");
			Assert.AreEqual (RuleResult.DoesNotApply, runner.CheckType (type), "RuleResult");
			Assert.AreEqual (0, runner.Defects.Count, "Count");
		}

		[Test]
		public void EnumWithZeroValue ()
		{
			TypeDefinition type = GetTest ("PrivateEnumWithZeroValue");
			Assert.AreEqual (RuleResult.Success, runner.CheckType (type), "RuleResult1");
			Assert.AreEqual (0, runner.Defects.Count, "Count1");

			type = GetTest ("EnumsShouldDefineAZeroValueTest/NestedPublicEnumWithZeroValue");
			Assert.AreEqual (RuleResult.Success, runner.CheckType (type), "RuleResult2");
			Assert.AreEqual (0, runner.Defects.Count, "Count2");
		}

		[Test]
		public void EnumWithoutZeroValue ()
		{
			TypeDefinition type = GetTest ("InternalEnumWithoutZeroValue");
			Assert.AreEqual (RuleResult.Failure, runner.CheckType (type), "RuleResult");
			Assert.AreEqual (1, runner.Defects.Count, "Count");
		}

		[Test]
		public void FlagWithoutZeroValue ()
		{
			TypeDefinition type = GetTest ("EnumsShouldDefineAZeroValueTest/NestedPrivateFlagsWithoutZeroValue");
			Assert.AreEqual (RuleResult.DoesNotApply, runner.CheckType (type), "RuleResult");
			Assert.AreEqual (0, runner.Defects.Count, "Count");
		}

		[Test]
		public void FlagWithZeroValue ()
		{
			// flags are ignored by the rule
			TypeDefinition type = GetTest ("EnumsShouldDefineAZeroValueTest/NestedInternalFlagsWithZeroValue");
			Assert.AreEqual (RuleResult.DoesNotApply, runner.CheckType (type), "RuleResult");
			Assert.AreEqual (0, runner.Defects.Count, "Count");
		}
	}
}
