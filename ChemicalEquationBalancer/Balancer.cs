using HtmlTags;
using System;
using System.Collections.Generic;

namespace ChemicalEquationBalancer
{
    public class Balancer
    {
        public string ErrorMessage { get; private set; }
        public HtmlTag EquationHtml { get; private set; }
        public string EquationText { get; private set; }
        public HtmlTag CodeOutputHtml { get; private set; }

        public void Solve(string input)
        {
            // Clear output
            ErrorMessage = "";
            EquationHtml = new HtmlTag("div");
            CodeOutputHtml = new HtmlTag("div");
            var utils = new Utils();

            // Parse equation
            Equation eq = null;
            try
            {
                eq = new Parser(input).ParseEquation();
            }
            catch (BalancerException bex)
            {
                ErrorMessage = "Syntax error: " + bex.Message;

                var start = bex.Start;
                var end = bex.End.HasValue ? bex.End.Value : bex.Start;
                var separators = new List<char> { ' ', '\t' };

                while (end > start && separators.IndexOf(input[end - 1]) != -1)
                {
                    end--;  // Adjust position to eliminate whitespace
                }
                if (start == end)
                {
                    end++;
                }

                CodeOutputHtml.AppendText(input.Substring(0, start));
                if (end <= input.Length)
                {
                    CodeOutputHtml.Append(utils.CreateElement("u", input.Substring(start, end)));
                    CodeOutputHtml.AppendText(input.Substring(end, input.Length));
                }
                else
                {
                    CodeOutputHtml.Append(utils.CreateElement("u", " "));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Syntax error: " + ex.Message;
                return;
            }

            try
            {
                var matrix = utils.BuildMatrix(eq);
                utils.Solve(matrix);
                var coefs = utils.ExtractCoefficients(matrix);
                utils.CheckAnswer(eq, coefs);
                EquationHtml.Append(eq.ToHtml(coefs));
                EquationText = eq.ToString(coefs);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
            }
        }
    }
}
