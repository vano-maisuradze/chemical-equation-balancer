using System;

namespace ChemicalEquationBalancer.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputs = new[] 
            {
                "H2 + O2 = H2O",
                "O2 + FeS2 = SO2 + Fe2O3",
                "H2O2 + Cr2O7^2- = Cr^3+ + O2 + OH^-",
                "K4Fe(CN)6 + KMnO4 + H2SO4 = KHSO4 + Fe2(SO4)3 + MnSO4 + HNO3 + CO2 + H2O"
            };
            var balancer = new Balancer();

            foreach (var input in inputs)
            {
                balancer.Solve(input);
                //Console.WriteLine(balancer.EquationHtml);
                Console.WriteLine($" Input: {input}");
                Console.WriteLine($"Output: {balancer.EquationText}");
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
