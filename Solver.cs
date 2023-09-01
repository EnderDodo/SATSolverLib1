namespace SatSolverLib;

public static class Solver
{
    // public static int Counter = 0;

    public static bool Dpll(Cnf cnf, bool[] intSolution, out bool[] solution)
    {
        solution = new bool[intSolution.Length];
        intSolution.CopyTo(solution, 0);
        while (cnf.UnitClause is { } unitClause)
        {
            var unitLiteral = unitClause.Literals.Single();
            solution[Math.Abs(unitLiteral)] = unitLiteral > 0;
            cnf = cnf.UnitPropagation(unitLiteral);
        }

        //pure literal elimination
        while (cnf.PureLiteral != 0)
        {
            solution[Math.Abs(cnf.PureLiteral)] = cnf.PureLiteral > 0;
            cnf = cnf.PureLiteralElimination(cnf.PureLiteral);
        }

        // Counter++;
        //
        // if (Counter == 10)
        // {
        //     Console.WriteLine(60);
        // }

        //stop conditions
        if (!cnf.Clauses.Any())
        {
            //Console.WriteLine(Counter);
            return true;
        }

        if (cnf.Clauses.Any(clause => clause.IsEmptyClause))
            return false;

        //select one literal
        var l = cnf.Literals.First();

        //recursion
        var clausesClone1 = cnf.Clauses.Select(clause => new Clause(clause.Literals));
        var clauses1 = new HashSet<Clause>(clausesClone1) { new Clause(l) };

        bool dpll1 = Dpll(new Cnf(clauses1), solution, out var solution1);
        bool dpll2 = false;

        if (dpll1) solution1.CopyTo(solution, 0);
        else
        {
            var clausesClone2 = cnf.Clauses.Select(clause => new Clause(clause.Literals));
            var clauses2 = new HashSet<Clause>(clausesClone2) { new Clause(-l) };
            dpll2 = Dpll(new Cnf(clauses2), solution, out var solution2);
            if (dpll2)
            {
                solution2.CopyTo(solution, 0);
            }
        }

        return dpll1 || dpll2;
    }

    public static bool Dpll(Cnf cnf, out bool[] solution)
    {
        var l = cnf.Literals.First();

        var intSolution = new bool[cnf.CountVars + 1];
        return Dpll(cnf, intSolution, out solution);
    }
}