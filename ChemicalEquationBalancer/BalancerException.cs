using System;

namespace ChemicalEquationBalancer
{
    public class BalancerException : Exception
    {
        public int Start { get; }
        public int? End { get; }

        public BalancerException()
        {

        }

        public BalancerException(string message) : base(message)
        {

        }

        public BalancerException(string message, int start, int? end = null) : base(message)
        {
            Start = start;
            End = end;
        }
    }
}
