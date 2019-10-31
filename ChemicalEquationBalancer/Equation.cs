using HtmlTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChemicalEquationBalancer
{
    public class Equation
    {
        public List<Term> LeftSide { get; private set; }
        public List<Term> RightSide { get; private set; }

        public Equation(List<Term> lhs, List<Term> rhs)
        {
            LeftSide = lhs;
            RightSide = rhs;
        }

        /// <summary>
        /// Returns an array of the names all of the elements used in this equation. 
        /// The array represents a set, so the items are in an arbitrary order and no item is repeated.
        /// </summary>
        public List<string> GetElements()
        {
            var result = new List<string>();
            var bothSides = LeftSide.Concat(RightSide);
            foreach (var item in bothSides)
            {
                item.GetElements(result);
            }
            return result;
        }


        /// <summary>
        /// Returns an HTML element representing this equation. 
        /// 'coefs' is an optional argument, which is an array of coefficients to match with the terms.
        /// </summary>
        public HtmlTag ToHtml(List<int> coefs = null)
        {
            if (coefs != null && coefs.Count != LeftSide.Count + RightSide.Count)
                throw new ArgumentException("Mismatched number of coefficients");

            var node = new HtmlTag("div");
            var utils = new Utils();

            var j = 0;

            void termsToHtml(List<Term> terms)
            {
                var head = true;
                foreach (var term in terms)
                {
                    var coef = coefs != null ? coefs[j] : 1;
                    if (coef != 0)
                    {
                        if (head)
                            head = false;
                        else
                            node.Append(utils.CreateSpan("plus", " + "));
                        if (coef != 1)
                        {
                            var span = utils.CreateSpan("coefficient", new Regex("-").Replace(coef.ToString(), Utils.MINUS));
                            if (coef < 0)
                                span.AddClass("negative");
                            node.Append(span);
                        }
                        node.Append(term.ToHtml());
                    }
                    j++;
                }
            }

            termsToHtml(LeftSide);
            node.Append(utils.CreateSpan("rightarrow", " \u2192 "));
            termsToHtml(RightSide);

            return node;
        }

        public string ToString(List<int> coefs = null)
        {
            if (coefs != null && coefs.Count != LeftSide.Count + RightSide.Count)
                throw new ArgumentException("Mismatched number of coefficients");

            var node = new StringBuilder();

            var j = 0;

            void termsToHtml(List<Term> terms)
            {
                var head = true;
                foreach (var term in terms)
                {
                    var coef = coefs != null ? coefs[j] : 1;
                    if (coef != 0)
                    {
                        if (head)
                            head = false;
                        else
                            node.Append(" + ");
                        if (coef != 1)
                        {
                            var span = new Regex("-").Replace(coef.ToString(), Utils.MINUS);
                            node.Append(span);
                        }
                        node.Append(term.ToString());
                    }
                    j++;
                }
            }

            termsToHtml(LeftSide);
            node.Append(" = ");
            termsToHtml(RightSide);

            return node.ToString();
        }
    }
}
