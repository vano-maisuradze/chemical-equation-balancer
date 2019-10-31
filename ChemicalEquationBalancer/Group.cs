using HtmlTags;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChemicalEquationBalancer
{
    public class Group : IElementGroup
    {
        private readonly List<IElementGroup> _items;
        private readonly int _count;

        public Group(List<IElementGroup> items, int count)
        {
            if (count < 1)
            {
                throw new Exception("Assertion error: Count must be a positive integer");
            }
            _items = items;
            _count = count;
        }

        public void GetElements(List<string> resultSet)
        {
            foreach (var item in _items)
            {
                item.GetElements(resultSet);
            }
        }

        public int CountElement(string name)
        {
            var utils = new Utils();
            var sum = 0;
            foreach (var item in _items)
            {
                sum = utils.CheckedAdd(sum, utils.CheckedMultiply(item.CountElement(name), _count));
            }
            return sum;
        }

        /// <summary>
        /// Returns an HTML element representing this group.
        /// </summary>
        public HtmlTag ToHtml()
        {
            var utils = new Utils();
            var node = utils.CreateSpan("group", "(");
            foreach (var item in _items)
            {
                node.Append(item.ToHtml());
            }
            node.AppendText(")");
            if (_count != 1)
            {
                node.Append(utils.CreateElement("sub", _count.ToString()));
            }
            return node;
        }

        public override string ToString()
        {
            var node = new StringBuilder();
            node.Append("(");
            foreach (var item in _items)
            {
                node.Append(item.ToString());
            }
            node.Append(")");
            if (_count != 1)
            {
                node.Append(_count.ToString());
            }
            return node.ToString();
        }
    }
}
