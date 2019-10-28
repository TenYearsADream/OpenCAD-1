﻿using System;

namespace OpenCAD.APIs.Math
{
    public class MathExpressionPolinomial : MathExpressionMember
    {
        public MathExpressionPolinomial(MathExpressionMonomial[] monomials)
        {
            Monomials = monomials ?? throw new ArgumentNullException(nameof(monomials));
        }

        public MathExpressionMonomial[] Monomials { get; }
    }
}
