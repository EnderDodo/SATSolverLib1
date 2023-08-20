using SatSolverLib;

namespace SATSolver;

public static class Solver
{
    public static bool SolveSat(Cnf cnf, out Literal[] solution)
    {
        return Cnf.Dpll(cnf, cnf.Solution, out solution);
    }
}