using HtmlTags;
using System.Collections.Generic;

namespace ChemicalEquationBalancer
{
    public interface IElementGroup
    {
        public void GetElements(List<string> resultSet);
        int CountElement(string name);
        HtmlTag ToHtml();
        string ToString();
    }
}
