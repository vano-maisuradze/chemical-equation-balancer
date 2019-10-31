
# chemical-equation-balancer

This code lets you balance chemical reaction equations. 
##### Usage example

    var input = "H2 + O2 = H2O";
    var balancer = new Balancer();
    balancer.Solve(input);
    Console.WriteLine($" Input: {input}");
    Console.WriteLine($"Output: {balancer.EquationText}");
    Console.WriteLine($"  HTML: {balancer.EquationHtml}");

##### Demo results:

     Input: H2 + O2 = H2O
    Output: 2H2 + O2 = 2H2O
    
     Input: O2 + FeS2 = SO2 + Fe2O3
    Output: 11O2 + 4FeS2 = 8SO2 + 2Fe2O3
    
     Input: H2O2 + Cr2O7^2- = Cr^3+ + O2 + OH^-
    Output: 8H2O2 + 2Cr2O7^2- = 4Cr^3+ + 7O2 + 16OH-
    
     Input: K4Fe(CN)6 + KMnO4 + H2SO4 = KHSO4 + Fe2(SO4)3 + MnSO4 + HNO3 + CO2 + H2O
    Output: 10K4Fe(CN)6 + 122KMnO4 + 299H2SO4 = 162KHSO4 + 5Fe2(SO4)3 + 122MnSO4 + 60HNO3 + 60CO2 + 188H2O


> This code was translated from Typescript to C#. You can find original code [here](https://www.nayuki.io/res/chemical-equation-balancer-javascript/chemical-equation-balancer.ts).
