using HtmlTags;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChemicalEquationBalancer
{
    public class Term
    {
        private readonly List<IElementGroup> _items;
        private readonly int _charge;

        public Term(List<IElementGroup> items, int charge)
        {
            if (items.Count == 0 && charge != -1)
                throw new Exception("Invalid term");  // Electron case
            _items = items;
            _charge = charge;
        }

        public void GetElements(List<string> resultSet)
        {
            if (!resultSet.Contains("e"))
            {
                resultSet.Add("e");
            }
            foreach (var item in _items)
            {
                item.GetElements(resultSet);
            }
        }

        /// <summary>
        /// Counts the number of times the given element (specified as a string) occurs in this term, 
        /// taking groups and counts into account, returning an integer.
        /// </summary>
        public int CountElement(string name)
        {
            if (name == "e")
            {
                return -_charge;
            }
            else
            {
                var sum = 0;
                var utils = new Utils();
                foreach (var item in _items)
                {
                    sum = utils.CheckedAdd(sum, item.CountElement(name));
                }
                return sum;
            }
        }

        public HtmlTag ToHtml()
        {
            var utils = new Utils();
            var node = utils.CreateSpan("term");
            if (_items.Count == 0 && _charge == -1)
            {
                node.Text("e");
                node.Append(utils.CreateElement("sup", Utils.MINUS));
            }
            else
            {
                foreach (var item in _items)
                {
                    node.Append(item.ToHtml());
                }
                if (_charge != 0)
                {
                    string s;
                    if (Math.Abs(_charge) == 1)
                    {
                        s = "";
                    }
                    else
                    {
                        s = Math.Abs(_charge).ToString();
                    }

                    if (_charge > 0)
                    {
                        s += "+";
                    }
                    else
                    {
                        s += Utils.MINUS;
                    }
                    node.Append(utils.CreateElement("sup", s));
                }
            }
            return node;
        }

        public override string ToString()
        {
            var node = new StringBuilder();
            if (_items.Count == 0 && _charge == -1)
            {
                node.Append("e^-");
            }
            else
            {
                foreach (var item in _items)
                {
                    node.Append(item.ToString());
                }
                if (_charge != 0)
                {
                    string s;
                    if (Math.Abs(_charge) == 1)
                    {
                        s = "";
                    }
                    else
                    {
                        s = "^" + Math.Abs(_charge).ToString();
                    }

                    if (_charge > 0)
                    {
                        s += "+";
                    }
                    else
                    {
                        s += "-";
                    }
                    node.Append(s);
                }
            }
            return node.ToString();
        }

    }
}
