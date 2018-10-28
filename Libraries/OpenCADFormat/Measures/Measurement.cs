﻿using OpenCAD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCAD.OpenCADFormat.Measures
{
    public class Measurement : IComparable<Measurement>, IEquatable<Measurement>
    {
        public static Measurement Add(Measurement a, Measurement b) => new Measurement(a.Amount
            + Utils.ConvertAmount(b, a.Unit), a.Unit);
        public static Measurement Subtract(Measurement a, Measurement b) => new Measurement(a.Amount
            - Utils.ConvertAmount(b, a.Unit), a.Unit);
        public static Measurement Negate(Measurement a) => new Measurement(-a.Amount, a.Unit);
        public static Measurement Multiply(Measurement a, double b) => new Measurement(a.Amount * b, a.Unit);
        public static Measurement Divide(Measurement a, double b) => new Measurement(a.Amount / b, a.Unit);
        public static double Divide(Measurement a, Measurement b) => a.Amount / Utils.ConvertAmount(b, a.Unit);
        public static Measurement Remainder(Measurement a, double b) => new Measurement(a.Amount % b, a.Unit);
        public static Measurement Remainder(Measurement a, Measurement b) => new Measurement(a.Amount
            % Utils.ConvertAmount(b, a.Unit), a.Unit);

        public static Measurement operator +(Measurement a, Measurement b) => Add(a, b);
        public static Measurement operator -(Measurement a, Measurement b) => Subtract(a, b);
        public static Measurement operator -(Measurement a) => Negate(a);
        public static Measurement operator *(Measurement a, double b) => Multiply(a, b);
        public static Measurement operator /(Measurement a, double b) => Divide(a, b);
        public static double operator /(Measurement a, Measurement b) => Divide(a, b);
        public static Measurement operator %(Measurement a, double b) => Remainder(a, b);
        public static Measurement operator %(Measurement a, Measurement b) => Remainder(a, b);

        public static bool operator ==(Measurement a, Measurement b) => a.Equals(b);
        public static bool operator !=(Measurement a, Measurement b) => !(a == b);

        public static bool operator >(Measurement a, Measurement b) => (a as IComparable<Measurement>).CompareTo(b) > 0;
        public static bool operator <(Measurement a, Measurement b) => (a as IComparable<Measurement>).CompareTo(b) < 0;

        int IComparable<Measurement>.CompareTo(Measurement other)
        {
            if (other.Unit.PhysicalQuantity != Unit.PhysicalQuantity)
                throw new InvalidOperationException("Cannot compare measurements. Measured physical quantities " +
                    "don't match.");

            return Comparer<double>.Default.Compare(GetAbsoluteAmount(), other.GetAbsoluteAmount());
        }

        bool IEquatable<Measurement>.Equals(Measurement other)
        {
            if (other.Unit.PhysicalQuantity != Unit.PhysicalQuantity)
                throw new InvalidOperationException("Cannot equate measurements. Measured physical quantities " +
                    "don't match.");

            return GetAbsoluteAmount() == other.GetAbsoluteAmount();
        }

        public static Measurement Parse(string value)
        {
            bool isFloatingPoint,
                hasExponent;

            string amountStr = null;

            StringScanner scanner = new StringScanner(value);
            StringUtils.ReadDecimalString(scanner, out amountStr, out isFloatingPoint, out hasExponent);

            double amount;

            if (!double.TryParse(amountStr, Conventions.STANDARD_NUMBER_STYLE, Conventions.STANDARD_CULTURE, out amount))
                throw new InvalidOperationException("String does not contain valid amount information.");

            string symbol = value.Remove(0, amountStr.Length);

            Unit unit; MetricPrefix prefix;

            (unit, prefix) = Utils.ParseUnitAndPrefix(symbol);

            return new Measurement(amount, unit, prefix);
        }

        public static bool TryParse(string value, out Measurement result)
        {
            result = null;

            try
            {
                result = Parse(value);

                return true;
            }
            catch { return false; }
        }

        public Measurement(double amount, Unit unit, MetricPrefix prefix = null)
        {
            Amount = amount;
            Unit = unit ?? throw new ArgumentNullException("unit");
            Prefix = prefix;
        }

        public override string ToString() => $"{Amount.ToString(Conventions.STANDARD_CULTURE)}{Prefix?.Symbol}{Unit.Symbol}";
        public string ToUIString() => $"{Amount.ToString(Conventions.STANDARD_CULTURE)}{Prefix?.UISymbol}{Unit.UISymbol}";

        public Measurement ConvertToUnit(Unit outUnit)
        {
            if (outUnit.PhysicalQuantity != Unit.PhysicalQuantity)
                throw new InvalidOperationException("Cannot convert measurement to the specified unit. " +
                    "Units have mismatched physical quantities.");

            return new Measurement(Utils.ConvertAmount(this, outUnit), outUnit);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double GetAbsoluteAmount() => Utils.GetAbsoluteAmount(this);

        public double Amount { get; set; }

        public Unit Unit { get; private set; }

        public MetricPrefix Prefix { get; private set; }

        public string Name => Prefix == null ? $"{Amount} {Unit.Name}" : $"{Amount} {Prefix.Name} {Unit.Name}";
    }
}