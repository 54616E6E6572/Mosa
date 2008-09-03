﻿using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.Reflection.Emit;

namespace Test.Mosa.Runtime.CompilerFramework.IL
{
    [TestFixture]
    public class ConvI2 : MosaCompilerTestRunner
    {
        delegate bool Native_ConvI2_I1(short expect, sbyte a);
        [Column(0, 1, 2, sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_I1(sbyte a)
        {
            CodeSource = "static class Test { static bool ConvI2_I1(short expect, sbyte a) { return expect == ((short)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_I1>("", "Test", "ConvI2_I1", ((short)a), a));
        }

        delegate bool Native_ConvI2_I2(short expect, short a);
        [Column(0, 1, 2, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_I2(short a)
        {
            CodeSource = "static class Test { static bool ConvI2_I2(short expect, short a) { return expect == ((short)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_I2>("", "Test", "ConvI2_I2", ((short)a), a));
        }

        delegate bool Native_ConvI2_I4(short expect, int a);
        [Column(0, 1, 2, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_I4(int a)
        {
            CodeSource = "static class Test { static bool ConvI2_I4(short expect, int a) { return expect == ((short)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_I4>("", "Test", "ConvI2_I4", ((short)a), a));
        }

        delegate bool Native_ConvI2_I8(short expect, long a);
        [Column(0, 1, 2, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_I8(long a)
        {
            CodeSource = "static class Test { static bool ConvI2_I8(short expect, long a) { return expect == ((short)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_I4>("", "Test", "ConvI2_I8", ((short)a), a));
        }

        delegate bool Native_ConvI2_R4(short expect, float a);
        [Column(0, 1, 2, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_R4(float a)
        {
            CodeSource = "static class Test { static bool ConvI1_R4(short expect, float a) { return expect == ((sbyte)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_R4>("", "Test", "ConvI1_R4", ((sbyte)a), a));
        }

        delegate bool Native_ConvI2_R8(short expect, double a);
        [Column(0, 1, 2, short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void ConvI2_R8(double a)
        {
            CodeSource = "static class Test { static bool ConvI1_R8(short expect, double a) { return expect == ((sbyte)a); } }";
            Assert.IsTrue((bool)Run<Native_ConvI2_R4>("", "Test", "ConvI1_R8", ((sbyte)a), a));
        }
    }
}
