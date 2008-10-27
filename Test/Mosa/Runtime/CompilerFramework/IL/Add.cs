﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Alex Lyman (<mailto:mail.alex.lyman@gmail.com>)
 *  Simon Wollwage (<mailto:rootnode@mosa-project.org>)
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 *  Kai P. Reisert (<mailto:kpreisert@googlemail.com>)
 *  
 */

using System;
using Gallio.Framework;
using MbUnit.Framework;
using Test.Mosa.Runtime.CompilerFramework.BaseCode;

namespace Test.Mosa.Runtime.CompilerFramework.IL
{
    /// <summary>
    /// Testcase for the AddInstruction
    /// </summary>
    [TestFixture]
    public class Add : CodeDomTestRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I4_I1_I1(int expect, sbyte a, sbyte b);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(sbyte.MinValue, 0)]
        [Row(sbyte.MinValue, 1)]
        [Row(sbyte.MinValue, 17)]
        [Row(sbyte.MinValue, 123)]
        [Row(sbyte.MinValue, -0)]
        [Row(sbyte.MinValue, -1)]
        [Row(sbyte.MinValue, -17)]
        [Row(sbyte.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(sbyte.MaxValue, 0)]
        [Row(sbyte.MaxValue, 1)]
        [Row(sbyte.MaxValue, 17)]
        [Row(sbyte.MaxValue, 123)]
        [Row(sbyte.MaxValue, -0)]
        [Row(sbyte.MaxValue, -1)]
        [Row(sbyte.MaxValue, -17)]
        [Row(sbyte.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, sbyte.MinValue)]
        [Row(1, sbyte.MinValue)]
        [Row(17, sbyte.MinValue)]
        [Row(123, sbyte.MinValue)]
        [Row(-0, sbyte.MinValue)]
        [Row(-1, sbyte.MinValue)]
        [Row(-17, sbyte.MinValue)]
        [Row(-123, sbyte.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, sbyte.MaxValue)]
        [Row(1, sbyte.MaxValue)]
        [Row(17, sbyte.MaxValue)]
        [Row(123, sbyte.MaxValue)]
        [Row(-0, sbyte.MaxValue)]
        [Row(-1, sbyte.MaxValue)]
        [Row(-17, sbyte.MaxValue)]
        [Row(-123, sbyte.MaxValue)]
        // Extremvaluecases
        [Row(sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddI1(sbyte a, sbyte b)
        {
            CodeSource = "static class Test { static bool AddI1(int expect, sbyte a, sbyte b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<I4_I1_I1>("", "Test", "AddI1", a + b, a, b));
        }
        
        delegate bool I4_Constant_I1(int expect, sbyte x); 
        delegate bool I4_Constant(int expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-42, 48)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(sbyte.MinValue, sbyte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantI1(sbyte a, sbyte b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantI1Right(int expect, sbyte a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I1>("", "Test", "AddConstantI1Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantI1Left(int expect, sbyte b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I1>("", "Test", "AddConstantI1Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantI1Both(int expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant>("", "Test", "AddConstantI1Both", (a + b)));
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        delegate bool I4_I1_C(int expect, sbyte a);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        [Row(1)]
        [Test, Author("rootnode")]
        public void AddConstantI1Right(sbyte a)
        {
            CodeSource = "static class Test { static bool AddConstantI1Right(int expect, sbyte a) { return expect == (a + 1); } }";
            Assert.IsTrue((bool)Run<I4_I1_C>("", "Test", "AddConstantI1Right", a + 1, a));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U1_U1(uint expect, byte a, byte b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(byte.MinValue, 0)]
        [Row(byte.MinValue, 1)]
        [Row(byte.MinValue, 17)]
        [Row(byte.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(byte.MaxValue, 0)]
        [Row(byte.MaxValue, 1)]
        [Row(byte.MaxValue, 17)]
        [Row(byte.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, byte.MinValue)]
        [Row(1, byte.MinValue)]
        [Row(17, byte.MinValue)]
        [Row(123, byte.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, byte.MaxValue)]
        [Row(1, byte.MaxValue)]
        [Row(17, byte.MaxValue)]
        [Row(123, byte.MaxValue)]
        // Extremvaluecases
        [Row(byte.MinValue, byte.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddU1(byte a, byte b)
        {
            CodeSource = "static class Test { static bool AddU1(uint expect, byte a, byte b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<U4_U1_U1>("", "Test", "AddU1", (uint)(a + b), a, b));
        }
        
        delegate bool U4_Constant_U1(uint expect, byte x); 
        delegate bool U4_Constant(uint expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(byte.MinValue, byte.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantU1(byte a, byte b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantU1Right(uint expect, byte a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U1>("", "Test", "AddConstantU1Right", (uint)(a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantU1Left(uint expect, byte b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U1>("", "Test", "AddConstantU1Left", (uint)(a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantU1Both(uint expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant>("", "Test", "AddConstantU1Both", (uint)(a + b)));
        }
        
        delegate bool I4_I2_I2(int expect, short a, short b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(short.MinValue, 0)]
        [Row(short.MinValue, 1)]
        [Row(short.MinValue, 17)]
        [Row(short.MinValue, 123)]
        [Row(short.MinValue, -0)]
        [Row(short.MinValue, -1)]
        [Row(short.MinValue, -17)]
        [Row(short.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(short.MaxValue, 0)]
        [Row(short.MaxValue, 1)]
        [Row(short.MaxValue, 17)]
        [Row(short.MaxValue, 123)]
        [Row(short.MaxValue, -0)]
        [Row(short.MaxValue, -1)]
        [Row(short.MaxValue, -17)]
        [Row(short.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, short.MinValue)]
        [Row(1, short.MinValue)]
        [Row(17, short.MinValue)]
        [Row(123, short.MinValue)]
        [Row(-0, short.MinValue)]
        [Row(-1, short.MinValue)]
        [Row(-17, short.MinValue)]
        [Row(-123, short.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, short.MaxValue)]
        [Row(1, short.MaxValue)]
        [Row(17, short.MaxValue)]
        [Row(123, short.MaxValue)]
        [Row(-0, short.MaxValue)]
        [Row(-1, short.MaxValue)]
        [Row(-17, short.MaxValue)]
        [Row(-123, short.MaxValue)]
        // Extremvaluecases
        [Row(short.MinValue, short.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddI2(short a, short b)
        {
            CodeSource = "static class Test { static bool AddI2(int expect, short a, short b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<I4_I2_I2>("", "Test", "AddI2", (a + b), a, b));
        }
        
        delegate bool I4_Constant_I2(int expect, short x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(short.MinValue, short.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantI2(short a, short b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantI2Right(int expect, short a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I2>("", "Test", "AddConstantI2Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantI2Left(int expect, short b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I2>("", "Test", "AddConstantI2Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantI2Both(int expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant>("", "Test", "AddConstantI2Both", (a + b)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U2_U2(uint expect, ushort a, ushort b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(ushort.MinValue, 0)]
        [Row(ushort.MinValue, 1)]
        [Row(ushort.MinValue, 17)]
        [Row(ushort.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(ushort.MaxValue, 0)]
        [Row(ushort.MaxValue, 1)]
        [Row(ushort.MaxValue, 17)]
        [Row(ushort.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, ushort.MinValue)]
        [Row(1, ushort.MinValue)]
        [Row(17, ushort.MinValue)]
        [Row(123, ushort.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, ushort.MaxValue)]
        [Row(1, ushort.MaxValue)]
        [Row(17, ushort.MaxValue)]
        [Row(123, ushort.MaxValue)]
        // Extremvaluecases
        [Row(ushort.MinValue, ushort.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddU2(ushort a, ushort b)
        {
            CodeSource = "static class Test { static bool AddU2(uint expect, ushort a, ushort b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<U4_U2_U2>("", "Test", "AddU2", (uint)(a + b), a, b));
        }
        
        delegate bool U4_Constant_U2(uint expect, ushort x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(ushort.MinValue, ushort.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantU2(ushort a, ushort b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantU2Right(uint expect, ushort a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U2>("", "Test", "AddConstantU2Right", (uint)(a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantU2Left(uint expect, ushort b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U2>("", "Test", "AddConstantU2Left", (uint)(a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantU2Both(uint expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant>("", "Test", "AddConstantU2Both", (uint)(a + b)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I4_I4_I4(int expect, int a, int b);
        // Normal Testcases + (0, 0)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(int.MinValue, 0)]
        [Row(int.MinValue, 1)]
        [Row(int.MinValue, 17)]
        [Row(int.MinValue, 123)]
        [Row(int.MinValue, -0)]
        [Row(int.MinValue, -1)]
        [Row(int.MinValue, -17)]
        [Row(int.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(int.MaxValue, 0)]
        [Row(int.MaxValue, 1)]
        [Row(int.MaxValue, 17)]
        [Row(int.MaxValue, 123)]
        [Row(int.MaxValue, -0)]
        [Row(int.MaxValue, -1)]
        [Row(int.MaxValue, -17)]
        [Row(int.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, int.MinValue)]
        [Row(1, int.MinValue)]
        [Row(17, int.MinValue)]
        [Row(123, int.MinValue)]
        [Row(-0, int.MinValue)]
        [Row(-1, int.MinValue)]
        [Row(-17, int.MinValue)]
        [Row(-123, int.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, int.MaxValue)]
        [Row(1, int.MaxValue)]
        [Row(17, int.MaxValue)]
        [Row(123, int.MaxValue)]
        [Row(-0, int.MaxValue)]
        [Row(-1, int.MaxValue)]
        [Row(-17, int.MaxValue)]
        [Row(-123, int.MaxValue)]
        // Extremvaluecases
        [Row(int.MinValue, int.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddI4(int a, int b)
        {
            CodeSource = "static class Test { static bool AddI4(int expect, int a, int b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<I4_I4_I4>("", "Test", "AddI4", (a + b), a, b));
        }
        
        delegate bool I4_Constant_I4(int expect, int x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(int.MinValue, int.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantI4(int a, int b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantI4Right(int expect, int a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I4>("", "Test", "AddConstantI4Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantI4Left(int expect, int b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<I4_Constant_I4>("", "Test", "AddConstantI4Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantI4Both(int expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I4_Constant>("", "Test", "AddConstantI4Both", (a + b)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U4_U4_U4(uint expect, uint a, uint b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(uint.MinValue, 0)]
        [Row(uint.MinValue, 1)]
        [Row(uint.MinValue, 17)]
        [Row(uint.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(uint.MaxValue, 0)]
        [Row(uint.MaxValue, 1)]
        [Row(uint.MaxValue, 17)]
        [Row(uint.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, uint.MinValue)]
        [Row(1, uint.MinValue)]
        [Row(17, uint.MinValue)]
        [Row(123, uint.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, uint.MaxValue)]
        [Row(1, uint.MaxValue)]
        [Row(17, uint.MaxValue)]
        [Row(123, uint.MaxValue)]
        // Extremvaluecases
        [Row(uint.MinValue, uint.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddU4(uint a, uint b)
        {
            CodeSource = "static class Test { static bool AddU4(uint expect, uint a, uint b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<U4_U4_U4>("", "Test", "AddU4", (uint)(a + b), a, b));
        }
        
        delegate bool U4_Constant_U4(uint expect, uint x); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(uint.MinValue, uint.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantU4(uint a, uint b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantU4Right(uint expect, uint a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U4>("", "Test", "AddConstantU4Right", (uint)(a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantU4Left(uint expect, uint b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<U4_Constant_U4>("", "Test", "AddConstantU4Left", (uint)(a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantU4Both(uint expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U4_Constant>("", "Test", "AddConstantU4Both", (uint)(a + b)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool I8_I8_I8(long expect, long a, long b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(1, -2)]
        [Row(-1, 2)]
        [Row(0, 0)]
        [Row(-17, -2)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        [Row(-2, 1)]
        [Row(2, -1)]
        [Row(-2, -17)]
        // (MinValue, X) Cases
        [Row(long.MinValue, 0)]
        [Row(long.MinValue, 1)]
        [Row(long.MinValue, 17)]
        [Row(long.MinValue, 123)]
        [Row(long.MinValue, -0)]
        [Row(long.MinValue, -1)]
        [Row(long.MinValue, -17)]
        [Row(long.MinValue, -123)]
        // (MaxValue, X) Cases
        [Row(long.MaxValue, 0)]
        [Row(long.MaxValue, 1)]
        [Row(long.MaxValue, 17)]
        [Row(long.MaxValue, 123)]
        [Row(long.MaxValue, -0)]
        [Row(long.MaxValue, -1)]
        [Row(long.MaxValue, -17)]
        [Row(long.MaxValue, -123)]
        // (X, MinValue) Cases
        [Row(0, long.MinValue)]
        [Row(1, long.MinValue)]
        [Row(17, long.MinValue)]
        [Row(123, long.MinValue)]
        [Row(-0, long.MinValue)]
        [Row(-1, long.MinValue)]
        [Row(-17, long.MinValue)]
        [Row(-123, long.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, long.MaxValue)]
        [Row(1, long.MaxValue)]
        [Row(17, long.MaxValue)]
        [Row(123, long.MaxValue)]
        [Row(-0, long.MaxValue)]
        [Row(-1, long.MaxValue)]
        [Row(-17, long.MaxValue)]
        [Row(-123, long.MaxValue)]
        [Row(0x0000000100000000L, 0x0000000100000000L)]
        // Extremvaluecases
        [Row(long.MinValue, long.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddI8(long a, long b)
        {
            CodeSource = "static class Test { static bool AddI8(long expect, long a, long b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<I8_I8_I8>("", "Test", "AddI8", (a + b), a, b));
        }
        
        delegate bool I8_Constant_I8(long expect, long x);
        delegate bool I8_Constant(long expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(-23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(long.MinValue, long.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantI8(long a, long b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantI8Right(long expect, long a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I8_Constant_I8>("", "Test", "AddConstantI8Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantI8Left(long expect, long b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<I8_Constant_I8>("", "Test", "AddConstantI8Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantI8Both(long expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<I8_Constant>("", "Test", "AddConstantI8Both", (a + b)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool U8_U8_U8(ulong expect, ulong a, ulong b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1, 2)]
        [Row(23, 21)]
        [Row(0, 0)]
        // And reverse
        [Row(2, 1)]
        [Row(21, 23)]
        // (MinValue, X) Cases
        [Row(ulong.MinValue, 0)]
        [Row(ulong.MinValue, 1)]
        [Row(ulong.MinValue, 17)]
        [Row(ulong.MinValue, 123)]
        // (MaxValue, X) Cases
        [Row(ulong.MaxValue, 0)]
        [Row(ulong.MaxValue, 1)]
        [Row(ulong.MaxValue, 17)]
        [Row(ulong.MaxValue, 123)]
        // (X, MinValue) Cases
        [Row(0, ulong.MinValue)]
        [Row(1, ulong.MinValue)]
        [Row(17, ulong.MinValue)]
        [Row(123, ulong.MinValue)]
        // (X, MaxValue) Cases
        [Row(0, ulong.MaxValue)]
        [Row(1, ulong.MaxValue)]
        [Row(17, ulong.MaxValue)]
        [Row(123, ulong.MaxValue)]
        // Extremvaluecases
        [Row(ulong.MinValue, ulong.MaxValue)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddU8(ulong a, ulong b)
        {
            CodeSource = "static class Test { static bool AddU8(ulong expect, ulong a, ulong b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<U8_U8_U8>("", "Test", "AddU8", (ulong)(a + b), a, b));
        }
        
        delegate bool U8_Constant_U8(ulong expect, ulong x);
        delegate bool U8_Constant(ulong expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148)]
        [Row(17, 1)]
        [Row(0, 0)]
        [Row(ulong.MinValue, ulong.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantU8(ulong a, ulong b)
        {
            // right side constant
            CodeSource = "static class Test { static bool AddConstantU8Right(ulong expect, ulong a) { return expect == (a + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U8_Constant_U8>("", "Test", "AddConstantU8Right", (ulong)(a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantU8Left(ulong expect, ulong b) { return expect == (" + a.ToString() + " + b); } }";
            Assert.IsTrue((bool)Run<U8_Constant_U8>("", "Test", "AddConstantU8Left", (ulong)(a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantU8Both(ulong expect) { return expect == (" + a.ToString() + " + " + b.ToString() + "); } }";
            Assert.IsTrue((bool)Run<U8_Constant>("", "Test", "AddConstantU8Both", (ulong)(a + b)));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool R4_R4_R4(float expect, float a, float b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1.0f, 1.0f)]
        [Row(1.0f, -2.198f)]
        [Row(-1.2f, 2.11f)]
        [Row(0.0f, 0.0f)]
        // (MinValue, X) Cases
        [Row(float.MinValue, 0.0f)]
        [Row(float.MinValue, 1.2f)]
        [Row(float.MinValue, 17.6f)]
        [Row(float.MinValue, 123.1f)]
        [Row(float.MinValue, -0.0f)]
        [Row(float.MinValue, -1.5f)]
        [Row(float.MinValue, -17.99f)]
        [Row(float.MinValue, -123.235f)]
        // (MaxValue, X) Cases
        [Row(float.MaxValue, 0.0f)]
        [Row(float.MaxValue, 1.67f)]
        [Row(float.MaxValue, 17.875f)]
        [Row(float.MaxValue, 123.283f)]
        [Row(float.MaxValue, -0.0f)]
        [Row(float.MaxValue, -1.1497f)]
        [Row(float.MaxValue, -17.12f)]
        [Row(float.MaxValue, -123.34f)]
        // (X, MinValue) Cases
        [Row(0.0f, float.MinValue)]
        [Row(1.2, float.MinValue)]
        [Row(17.4f, float.MinValue)]
        [Row(123.561f, float.MinValue)]
        [Row(-0.0f, float.MinValue)]
        [Row(-1.78f, float.MinValue)]
        [Row(-17.59f, float.MinValue)]
        [Row(-123.41f, float.MinValue)]
        // (X, MaxValue) Cases
        [Row(0.0f, float.MaxValue)]
        [Row(1.00012f, float.MaxValue)]
        [Row(17.094002f, float.MaxValue)]
        [Row(123.001f, float.MaxValue)]
        [Row(-0.0f, float.MaxValue)]
        [Row(-1.045f, float.MaxValue)]
        [Row(-17.0002501f, float.MaxValue)]
        [Row(-123.023f, float.MaxValue)]
        // Extremvaluecases
        [Row(float.MinValue, float.MaxValue)]
        [Row(1.0f, float.NaN)]
        [Row(float.NaN, 1.0f)]
        [Row(1.0f, float.PositiveInfinity)]
        [Row(float.PositiveInfinity, 1.0f)]
        [Row(1.0f, float.NegativeInfinity)]
        [Row(float.NegativeInfinity, 1.0f)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddR4(float a, float b)
        {
            CodeSource = "static class Test { static bool AddR4(float expect, float a, float b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<R4_R4_R4>("", "Test", "AddR4", a + b, a, b));
        }
        
        delegate bool R4_Constant_R4(float expect, float x);
        delegate bool R4_Constant(float expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23f, 148.0016f)]
        [Row(17.2f, 1f)]
        [Row(0f, 0f)]
        [Row(float.MinValue, float.MaxValue)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantR4(float a, float b)
        {
            string x = a.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
            string y = b.ToString(System.Globalization.CultureInfo.InvariantCulture) + "f";
            
            // right side constant
            CodeSource = "static class Test { static bool AddConstantR4Right(float expect, float a) { return expect == (a + " + y + "); } }";
            Assert.IsTrue((bool)Run<R4_Constant_R4>("", "Test", "AddConstantR4Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantR4Left(float expect, float b) { return expect == (" + x + " + b); } }";
            Assert.IsTrue((bool)Run<R4_Constant_R4>("", "Test", "AddConstantR4Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantR4Both(float expect) { return expect == (" + x + " + " + y + "); } }";
            Assert.IsTrue((bool)Run<R4_Constant>("", "Test", "AddConstantR4Both", (a + b)));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expect"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool R8_R8_R8(double expect, double a, double b);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(1.2, 2.1)]
        [Row(23.0, 21.2578)]
        [Row(1.0, -2.198)]
        [Row(-1.2, 2.11)]
        [Row(0.0, 0.0)]
        [Row(-17.1, -2.3)]
        // (MinValue, X) Cases
        [Row(double.MinValue, 0.0)]
        [Row(double.MinValue, 1.2)]
        [Row(double.MinValue, 17.6)]
        [Row(double.MinValue, 123.1)]
        [Row(double.MinValue, -0.0)]
        [Row(double.MinValue, -1.5)]
        [Row(double.MinValue, -17.99)]
        [Row(double.MinValue, -123.235)]
        // (MaxValue, X) Cases
        [Row(double.MaxValue, 0.0)]
        [Row(double.MaxValue, 1.67)]
        [Row(double.MaxValue, 17.875)]
        [Row(double.MaxValue, 123.283)]
        [Row(double.MaxValue, -0.0)]
        [Row(double.MaxValue, -1.1497)]
        [Row(double.MaxValue, -17.12)]
        [Row(double.MaxValue, -123.34)]
        // (X, MinValue) Cases
        [Row(0.0, double.MinValue)]
        [Row(1.2, double.MinValue)]
        [Row(17.4, double.MinValue)]
        [Row(123.561, double.MinValue)]
        [Row(-0.0, double.MinValue)]
        [Row(-1.78, double.MinValue)]
        [Row(-17.59, double.MinValue)]
        [Row(-123.41, double.MinValue)]
        // (X, MaxValue) Cases
        [Row(0.0, double.MaxValue)]
        [Row(1.00012, double.MaxValue)]
        [Row(17.094002, double.MaxValue)]
        [Row(123.001, double.MaxValue)]
        [Row(-0.0, double.MaxValue)]
        [Row(-1.045, double.MaxValue)]
        [Row(-17.0002501, double.MaxValue)]
        [Row(-123.023, double.MaxValue)]
        // Extremvaluecases
        [Row(double.MinValue, double.MaxValue)]
        [Row(1.0f, double.NaN)]
        [Row(double.NaN, 1.0f)]
        [Row(1.0f, double.PositiveInfinity)]
        [Row(double.PositiveInfinity, 1.0f)]
        [Row(1.0f, double.NegativeInfinity)]
        [Row(double.NegativeInfinity, 1.0f)]
        [Test, Author("alyman", "mail.alex.lyman@gmail.com")]
        public void AddR8(double a, double b)
        {
            CodeSource = "static class Test { static bool AddR8(double expect, double a, double b) { return expect == (a + b); } }";
            Assert.IsTrue((bool)Run<R8_R8_R8>("", "Test", "AddR8", (a + b), a, b));
        }
        
        delegate bool R8_Constant_R8(double expect, double x);
        delegate bool R8_Constant(double expect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [Row(23, 148.0016)]
        [Row(17.2, 1.0)]
        [Row(0.0, 0.0)]
        [Row(-1.79769313486231E+308, 1.79769313486231E+308)]
        [Test, Author("boddlnagg", "kpreisert@googlemail.com")]
        public void AddConstantR8(double a, double b)
        {                
            string x = a.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string y = b.ToString(System.Globalization.CultureInfo.InvariantCulture);
            
            // right side constant
            CodeSource = "static class Test { static bool AddConstantR8Right(double expect, double a) { return expect == (a + " + y + "); } }";
            Assert.IsTrue((bool)Run<R8_Constant_R8>("", "Test", "AddConstantR8Right", (a + b), a));
            
            // left side constant
            CodeSource = "static class Test { static bool AddConstantR8Left(double expect, double b) { return expect == (" + x + " + b); } }";
            Assert.IsTrue((bool)Run<R8_Constant_R8>("", "Test", "AddConstantR8Left", (a + b), b));
            
            // both constant
            CodeSource = "static class Test { static bool AddConstantR8Both(double expect) { return expect == (" + x + " + " + y + "); } }";
            Assert.IsTrue((bool)Run<R8_Constant>("", "Test", "AddConstantR8Both", (a + b)));
        }
    }
}
