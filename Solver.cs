namespace SatSolverLib;

public static class Solver
{
    public static int Counter = 0;

    public static bool Dpll(Cnf cnf, bool[] intermediateSolution, out bool[] solution)
    {
        solution = new bool[intermediateSolution.Length];
        intermediateSolution.CopyTo(solution, 0);

        while (cnf.UnitClause is { } unitClause)
        {
            var unitLiteral = unitClause.Literals.Single();
            cnf = cnf.UnitPropagation(unitLiteral);
            solution[unitLiteral.Item1] = unitLiteral.Item2;
        }

        //pure literal elimination
        while (cnf.PureLiteral is { } pureLiteral)
        {
            cnf = cnf.PureLiteralElimination(pureLiteral);
            solution[pureLiteral.Item1] = pureLiteral.Item2;
        }

        //stop conditions
        if (!cnf.Clauses.Any())
            return true;
        if (cnf.Clauses.Any(clause => clause.IsEmptyClause))
            return false;

        //select one literal
        var l = cnf.Literals.First();

        //recursion
        var clauses1 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(clauses1, 1);
        clauses1[0] = new Clause(l);

        bool dpll1 = Dpll(new Cnf(clauses1), solution, out var solution1);

        if (dpll1)
        {
            solution1.CopyTo(solution, 0);
            return true;
        }

        var clauses2 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(clauses2, 1);
        clauses2[0] = new Clause((l.Item1, !l.Item2));
        bool dpll2 = Dpll(new Cnf(clauses2), solution, out var solution2);
        if (dpll2)
        {
            solution2.CopyTo(solution, 0);
            return true;
        }

        return false;
    }

    public static bool Dpll(Cnf cnf, out bool[] solution)
    {
        var intSolution = new bool[cnf.CountVars + 1];
        for (int i = 0; i < intSolution.Length; i++)
        {
            intSolution[i] = false;
        }

        return Dpll(cnf, intSolution, out solution);
    }
}