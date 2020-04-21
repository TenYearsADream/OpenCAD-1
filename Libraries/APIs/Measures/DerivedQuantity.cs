﻿using System;
using System.Linq;

namespace OpenCAD.APIs.Measures
{
    public sealed class DerivedQuantity : Quantity
    {
        public DerivedQuantity(DerivedQuantityDimension dimension)
        {
            _name = null;
            _symbol = null;
            _uiSymbol = null;
            Dimension = dimension ?? throw new ArgumentNullException(nameof(dimension));
            IsNamed = false;
        }

        public DerivedQuantity(string name, string symbol, DerivedQuantityDimension dimension)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            _uiSymbol = null;
            Dimension = dimension ?? throw new ArgumentNullException(nameof(dimension));
            IsNamed = true;
        }

        public DerivedQuantity(string name, string symbol, string uiSymbol, DerivedQuantityDimension dimension)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            _uiSymbol = uiSymbol;
            Dimension = dimension;
            IsNamed = true;
        }

        public DerivedQuantity(BaseQuantity baseQuantity, double exponent)
        {
            _name = null;
            _symbol = null;
            _uiSymbol = null;
            Dimension = getDimension(baseQuantity, exponent);
            IsNamed = false;
        }

        public DerivedQuantity(string name, string symbol, BaseQuantity baseQuantity, double exponent)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            _uiSymbol = null;
            Dimension = getDimension(baseQuantity, exponent);
            IsNamed = true;
        }

        public DerivedQuantity(string name, string symbol, string uiSymbol, BaseQuantity baseQuantity, double exponent)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            _uiSymbol = uiSymbol;
            Dimension = getDimension(baseQuantity, exponent);
            IsNamed = true;
        }

        private DerivedQuantityDimension getDimension(BaseQuantity baseQuantity, double exponent) =>
            new DerivedQuantityDimension(new DerivedQuantityDimensionMember(baseQuantity ??
                throw new ArgumentNullException(nameof(baseQuantity)), exponent));

        public DerivedQuantityDimension Dimension { get; }

        public override string Name => _name ?? generateName();
        private string generateName() => null;
        private readonly string _name;

        public override string Symbol => _symbol ?? generateSymbol();
        private string generateSymbol() => null;
        private readonly string _symbol;

        public override string UISymbol => _uiSymbol ?? generateUISymbol();
        private string generateUISymbol() => null;
        private readonly string _uiSymbol;

        public readonly bool IsNamed;

        public override Quantity Collapse()
        {
            var members = Dimension.Members
                .Select(m => (bu: m.BaseQuantity.Collapse(), e: m.Exponent))
                .Where(mt => mt.bu != null)
                .GroupBy(mt => mt.bu)
                .Select(g => new DerivedQuantityDimensionMember(g.Key, g.Sum(m => m.e)))
                .Where(m => m.Exponent != 0)
                .ToArray();
            DerivedQuantityDimensionMember firstMember;
            if (members.Length == 0)
                return null;
            else if (members.Length == 1 && (firstMember = members.First()).Exponent == 1)
                return firstMember.BaseQuantity;
            else
                return new DerivedQuantity(new DerivedQuantityDimension(members));
        }
    }
}