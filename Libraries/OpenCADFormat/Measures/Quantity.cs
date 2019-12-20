﻿using System;
using System.Linq;

namespace OpenCAD.OpenCADFormat.Measures
{
    public abstract class Quantity: IMultipliable<Quantity, Quantity>, IExponentiable<Quantity>
    {
        public static Quantity operator *(Quantity a, Quantity b) => Math.Multiply(a, b);

        public static Quantity operator /(Quantity a, Quantity b) => Math.Divide(a, b);

        public static Quantity operator !(Quantity a) => Math.Invert(a);

        public static Quantity operator ^(Quantity a, double b) => Math.Power(a, b);

        public static Quantity Parse(string value)
        {
            Quantity result;
            if (TryParse(value, out result))
                return result;

            throw new ArgumentOutOfRangeException(nameof(value));
        }

        public static bool TryParse(string value, out Quantity result) =>
            tryParseBySymbol(value, out result) || tryParseByUISymbol(value, out result) || tryParseByName(value, out result);

        private static bool tryParseBySymbol(string symbol, out Quantity result)
        {
            result = MetricSystemManager.GetAllQuantities().FirstOrDefault(q => q.Symbol == symbol);
            if (result == null)
                return false;

            return true;
        }

        private static bool tryParseByUISymbol(string uiSymbol, out Quantity result)
        {
            result = MetricSystemManager.GetAllQuantities().FirstOrDefault(q => q.UISymbol == uiSymbol);
            if (result == null)
                return false;

            return true;
        }

        private static bool tryParseByName(string name, out Quantity result)
        {
            result = MetricSystemManager.GetAllQuantities().FirstOrDefault(q => q.Name == name);
            if (result == null)
                return false;

            return true;
        }

        public abstract string Name { get; }

        public abstract string Symbol { get; }

        public abstract string UISymbol { get; }

        public MetricSystem MetricSystem { get; internal set; } = null;

        public abstract Quantity Collapse();

        Quantity IMultipliable<Quantity, Quantity>.Multiply(Quantity value)
        {
            DerivedQuantity derivedThis = null;
            if (this is BaseQuantity)
                derivedThis = new DerivedQuantity((BaseQuantity)this, 1);
            else if (this is DerivedQuantity)
                derivedThis = (DerivedQuantity)this;

            DerivedQuantity derivedValue = null;
            if (value is BaseQuantity)
                derivedValue = new DerivedQuantity((BaseQuantity)value, 1);
            else if (value is DerivedQuantity)
                derivedValue = (DerivedQuantity)value;

            if (derivedThis is null || derivedValue is null)
                throw new NotSupportedException();

            return multiply(derivedThis, derivedValue);
        }

        private Quantity multiply(DerivedQuantity a, DerivedQuantity b)
        {
            var members = a.Dimension.Members.Concat(b.Dimension.Members).ToArray();
            var expression = new DerivedQuantityDimension(members);
            return new DerivedQuantity(expression);
        }

        Quantity IExponentiable<Quantity>.Exponentiate(double exponent)
        {
            DerivedQuantity derivedThis = null;
            if (this is BaseQuantity)
                derivedThis = new DerivedQuantity((BaseQuantity)this, 1);
            else if (this is DerivedQuantity)
                derivedThis = (DerivedQuantity)this;

            if (derivedThis is null)
                throw new NotSupportedException();

            return exponentiate(derivedThis, exponent);
        }

        private Quantity exponentiate(DerivedQuantity derivedQuantity, double exponent)
        {
            var members = derivedQuantity.Dimension.Members.Select(m => new DerivedQuantityDimensionMember(m.BaseQuantity, 
                m.Exponent * exponent)).ToArray();
            var expression = new DerivedQuantityDimension(members);
            return new DerivedQuantity(expression);
        }
    }
}