﻿using OpenCAD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenCAD.OpenCADFormat.Measures
{
    public class DerivedQuantityDimension
    {
        public static DerivedQuantityDimension Parse(string value)
        {
            StringScanner scanner = new StringScanner(value);

            IEnumerable<DerivedQuantityDimensionMember> allMembersReader = readAllMembers(scanner);

            return new DerivedQuantityDimension(allMembersReader.ToArray());
        }

        private static IEnumerable<DerivedQuantityDimensionMember> readAllMembers(StringScanner scanner)
        {
            foreach (var member in readMembers(scanner))
                yield return member;

            if (scanner.CurrentChar == '/')
            {
                scanner.Increment();

                if (scanner.CurrentChar == '(')
                {
                    scanner.Increment();

                    foreach (var member in readMembers(scanner))
                        yield return member.Invert();

                    if (scanner.CurrentChar == ')')
                        scanner.Increment();
                    else
                        throw new FormatException("A closing bracket was expected.");
                }
                else
                {
                    DerivedQuantityDimensionMember member;

                    if (readMember(scanner, out member))
                        yield return member.Invert();
                    else
                        throw new FormatException("A valid dimension member was expected.");
                }
            }
        }

        private static IEnumerable<DerivedQuantityDimensionMember> readMembers(StringScanner scanner)
        {
            DerivedQuantityDimensionMember member;

            while (readMember(scanner, out member))
            {
                yield return member;

                if (scanner.CurrentChar == '*')
                    scanner.Increment();
                else
                    yield break;
            }
        }

        private static bool readMember(StringScanner scanner, out DerivedQuantityDimensionMember result)
        {
            using (var token = scanner.SaveIndex())
            {
                string baseQuantityStr = scanner.GetString(token);

                if (StringUtils.ReadIdentifier(scanner, out baseQuantityStr))
                {
                    BaseQuantity baseQuantity;

                    if (!BaseQuantity.TryParse(baseQuantityStr, out baseQuantity))
                        throw new FormatException("A valid base quantity symbol was expected.");

                    double exponent = 1;

                    if (scanner.CurrentChar == '^')
                    {
                        scanner.Increment();

                        string exponentStr;

                        StringUtils.ReadDecimalString(scanner, out exponentStr);

                        if (!double.TryParse(exponentStr, out exponent))
                            throw new FormatException("A valid exponent decimal was expected.");
                    }

                    result = new DerivedQuantityDimensionMember(baseQuantity, exponent);
                    return true;
                }

                scanner.RestoreIndex(token);
                result = default;
                return false;
            }
        }

        public DerivedQuantityDimension(params DerivedQuantityDimensionMember[] members)
        {
            Members = members ?? throw new ArgumentNullException(nameof(members));
        }

        public IReadOnlyList<DerivedQuantityDimensionMember> Members { get; }
    }
}