using HtmlTags;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChemicalEquationBalancer
{
    public class ChemicalElemement : IElementGroup
    {
        private readonly string name;
        private readonly int count;

        public ChemicalElemement(string name, int count)
        {
            if (count < 1)
            {

                throw new Exception("Assertion error: Count must be a positive integer");
            }

            this.name = name;
            this.count = count;
        }

        public void GetElements(List<string> resultSet)
        {
            if (!resultSet.Contains(name))
            {
                resultSet.Add(name);
            }
        }

        public int CountElement(string n)
        {
            return n == name ? count : 0;
        }

        /// <summary>
        /// Returns an HTML element representing this element.
        /// </summary>
        /// <returns></returns>
        public HtmlTag ToHtml()
        {
            var utils = new Utils();
            var node = utils.CreateSpan("element", name);
            if (count != 1)
            {
                node.Append(utils.CreateElement("sub", count.ToString()));
            }
            return node;
        }

        public override string ToString()
        {
            var node = new StringBuilder();
            node.Append(name);
            if (count != 1)
            {
                node.Append(count.ToString());
            }
            return node.ToString();
        }
    }
}
