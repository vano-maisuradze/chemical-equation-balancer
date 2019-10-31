using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChemicalEquationBalancer
{
    public class Parser
    {
        private readonly Tokenizer _tok;

        public Parser(string formulaStr)
        {
            _tok = new Tokenizer(formulaStr);
        }

        /// <summary>
        /// Parses and returns an equation
        /// </summary>
        public Equation ParseEquation()
        {
            var lhs = new List<Term>
            {
                ParseTerm()
            };
            while (true)
            {
                var next = _tok.Peek();
                if (next == "+")
                {
                    _tok.Consume(next);
                    lhs.Add(ParseTerm());
                }
                else if (next == "=")
                {
                    this._tok.Consume(next);
                    break;
                }
                else
                    throw new Exception($"Plus or equal sign expected. Start: {_tok.Position}");
            }

            var rhs = new List<Term>
            {
                ParseTerm()
            };

            while (true)
            {
                var next = _tok.Peek();
                if (next == null)
                {
                    break;
                }
                else if (next == "+")
                {
                    _tok.Consume(next);
                    rhs.Add(ParseTerm());
                }
                else
                    throw new Exception($"Plus or end expected. Start: {_tok.Position}");
            }
            return new Equation(lhs, rhs);
        }

        /// <summary>
        /// Parses and returns a term
        /// </summary>
        private Term ParseTerm()
        {
            var startPos = _tok.Position;

            // Parse groups and elements
            var items = new List<IElementGroup>();
            var electron = false;
            string next;
            while (true)
            {
                next = _tok.Peek();
                if (next == "(")
                {
                    items.Add(ParseGroup());
                }
                else if (next == "e")
                {
                    _tok.Consume(next);
                    electron = true;
                }
                else if (next != null && new Regex("^[A-Z][a-z]*$").IsMatch(next))
                {
                    items.Add(ParseElement());
                }
                else if (next != null && new Regex("^[0-9]+$").IsMatch(next))
                {
                    throw new Exception($"Invalid term - number not expected. Start: {_tok.Position}");
                }
                else
                {
                    break;
                }
            }

            // Parse optional charge
            int? charge = null;
            if (next == "^")
            {
                _tok.Consume(next);

                next = _tok.Peek();
                if (next == null)
                {
                    throw new Exception($"Number or sign expected. Start: {_tok.Position}");
                }
                else
                {
                    charge = ParseOptionalNumber();
                    next = _tok.Peek();
                }

                if (next == "+")
                {
                    charge = +charge;  // No-op
                }
                else if (next == "-")
                {
                    charge = -charge;
                }
                else
                {
                    throw new Exception($"Sign expected. Start: {_tok.Position}");
                }
                _tok.Take();  // Consume the sign
            }

            // Check and postprocess term
            if (electron)
            {
                if (items.Count > 0)
                {
                    throw new Exception($"Invalid term - electron needs to stand alone. Start: {startPos} End: {_tok.Position}");
                }
                if (charge == null)  // Allow omitting the charge
                {
                    charge = -1;
                }
                if (charge != -1)
                {
                    throw new Exception($"Invalid term - invalid charge for electron. Start: {startPos} End: {_tok.Position}");
                }
            }
            else
            {
                if (items.Count == 0)
                {
                    throw new Exception($"Invalid term - empty. Start: {startPos} End: {_tok.Position}");
                }
                if (charge == null)
                {
                    charge = 0;
                }
            }
            return new Term(items, charge.Value);
        }

        /// <summary>
        /// Parses a number if it's the next token, returning a non-negative integer, with a default of 1.
        /// </summary>
        private int ParseOptionalNumber()
        {
            var utils = new Utils();
            var next = _tok.Peek();
            if (next != null && new Regex("^[0-9]+$").IsMatch(next))
            {
                return utils.CheckedParseInt(_tok.Take());
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Parses and returns an element.
        /// </summary>
        private IElementGroup ParseElement()
        {
            var name = _tok.Take();
            if (!new Regex("^[A-Z][a-z]*$").IsMatch(name))
            {
                throw new Exception("Assertion error");
            }
            return new ChemicalElemement(name, ParseOptionalNumber());
        }

        private IElementGroup ParseGroup()
        {
            var startPos = _tok.Position;
            _tok.Consume("(");
            var items = new List<IElementGroup>();
            while (true)
            {
                var next = _tok.Peek();
                if (next == "(")
                {
                    items.Add(ParseGroup());
                }
                else if (next != null && new Regex("^[A-Z][a-z]*$").IsMatch(next))
                {
                    items.Add(ParseElement());
                }
                else if (next == ")")
                {
                    _tok.Consume(next);
                    if (items.Count == 0)
                    {
                        throw new Exception($"Empty group. Start: {startPos} End: {_tok.Position}");
                    }
                    break;
                }
                else
                {
                    throw new Exception($"Element, group, or closing parenthesis expected. Start: {_tok.Position}");
                }
            }
            return new Group(items, ParseOptionalNumber());
        }
    }
}
