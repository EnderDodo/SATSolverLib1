namespace SatSolverLib;

public static class Solver
{
    public static bool SolveSat(Cnf cnf, out Literal[] solution)
    {
        var intSolution = new Literal[cnf.CountVars + 1];
        for (int i = 0; i < intSolution.Length; i++)
        {
            intSolution[i] = new Literal(0, false);
        }
        return Cnf.Dpll(cnf, intSolution, out solution);
    }
}