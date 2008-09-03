﻿using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Test.Mosa.Runtime.CompilerFramework.IL
{
    [TestFixture]
    public class Ldarga : MosaCompilerTestRunner
    {
        #region CheckValue

        delegate bool I1_I1(sbyte expect, ref sbyte a);
        [Column(0, 1, sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI1_CheckValue(sbyte a)
        {
            CodeSource = "static class Test { static bool LdargaI1_CheckValue(sbyte expect, ref sbyte a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<I1_I1>("", "Test", "LdargaI1_CheckValue", a, a));
        }

        delegate bool I2_I2(short expect, ref short a);
        [Column(0, 1, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI2_CheckValue(short a)
        {
            CodeSource = "static class Test { static bool LdargaI2_CheckValue(short expect, ref short a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<I2_I2>("", "Test", "LdargaI2_CheckValue", a, a));
        }

        delegate bool I4_I4(int expect, ref int a);
        [Column(0, 1, int.MinValue, int.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI4_CheckValue(int a)
        {
            CodeSource = "static class Test { static bool LdargaI4_CheckValue(int expect, ref int a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<I4_I4>("", "Test", "LdargaI4_CheckValue", a, a));
        }

        delegate bool I8_I8(long expect, ref long a);
        [Column(0, 1, long.MinValue, long.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI8_CheckValue(long a)
        {
            CodeSource = "static class Test { static bool LdargaI8_CheckValue(long expect, ref long a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<I8_I8>("", "Test", "LdargaI8_CheckValue", a, a));
        }

        delegate bool R4_R4(float expect, ref float a);
        [Column(0, 1, float.MinValue, float.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaR4_CheckValue(float a)
        {
            CodeSource = "static class Test { static bool LdargaR4_CheckValue(float expect, ref float a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<R4_R4>("", "Test", "LdargaR4_CheckValue", a, a));
        }

        delegate bool R8_R8(double expect, ref double a);
        [Column(0, 1, double.MinValue, double.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaR8_CheckValue(double a)
        {
            CodeSource = "static class Test { static bool LdargaR8_CheckValue(double expect, ref double a) { return expect == a; } }";
            Assert.IsTrue((bool)Run<R8_R8>("", "Test", "LdargaR8_CheckValue", a, a));
        }

        #endregion

        #region ChangeValue

        delegate void V_I1_I1(sbyte value, ref sbyte a);
        [Row(1, 0), Row(0, 1), Row(1, sbyte.MinValue), Row(0, sbyte.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI1_ChangeValue(sbyte newValue, sbyte oldValue)
        {
            CodeSource = "static class Test { static void LdargaI1_ChangeValue(sbyte value, ref sbyte a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_I1_I1>("", "Test", "LdargaI1_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        delegate void V_I2_I2(short value, ref short a);
        [Row(1, 0), Row(0, 1), Row(1, short.MinValue), Row(0, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI2_ChangeValue(short newValue, short oldValue)
        {
            CodeSource = "static class Test { static void LdargaI2_ChangeValue(short value, ref short a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_I2_I2>("", "Test", "LdargaI2_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        delegate void V_I4_I4(int value, ref int a);
        [Row(1, 0), Row(0, 1), Row(1, int.MinValue), Row(0, int.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI4_ChangeValue(int newValue, int oldValue)
        {
            CodeSource = "static class Test { static void LdargaI4_ChangeValue(int value, ref int a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_I4_I4>("", "Test", "LdargaI4_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        delegate void V_I8_I8(long value, ref long a);
        [Row(1, 0), Row(0, 1), Row(1, long.MinValue), Row(0, long.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaI8_ChangeValue(long newValue, long oldValue)
        {
            CodeSource = "static class Test { static void LdargaI8_ChangeValue(long value, ref long a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_I8_I8>("", "Test", "LdargaI8_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        delegate void V_R4_R4(float value, ref float a);
        [Row(1, 0), Row(0, 1), Row(1, float.MinValue), Row(0, float.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaR4_ChangeValue(float newValue, float oldValue)
        {
            CodeSource = "static class Test { static void LdargaR4_ChangeValue(float value, ref float a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_R4_R4>("", "Test", "LdargaR4_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        delegate void V_R8_R8(double value, ref double a);
        [Row(1, 0), Row(0, 1), Row(1, double.MinValue), Row(0, double.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void LdargaR8_ChangeValue(double newValue, double oldValue)
        {
            CodeSource = "static class Test { static void LdargaR8_ChangeValue(double value, ref double a) { a = value; } }";
            object[] args = new object[] { newValue, oldValue };
            Run<V_R8_R8>("", "Test", "LdargaR8_ChangeValue", args);
            Console.WriteLine("{0} {1} {2}", newValue, args[0], args[1]);
            Assert.AreEqual(newValue, args[1]);
        }

        #endregion
    }
}
