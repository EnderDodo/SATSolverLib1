using SATSolver;

namespace SatSolverLib;

public class Cnf
{
    public HashSet<Clause> Clauses;
    public Literal[] Literals;
    public Literal[] Solution;

    public Cnf(IEnumerable<Clause> clauses)
    {
        Clauses = new HashSet<Clause>(clauses);
        Literals = Clauses.SelectMany(clause => clause.Literals).ToArray();
        var literals = Literals
            .Select(literal => literal.Index).Distinct()
            .Select(i => new Literal(i, true)).ToArray();
        Solution = new Literal[literals.Length + 1];
        for (int i = 0; i < Solution.Length; i++)
        {
            Solution[i] = new Literal(0, false);
        }
    }

    //in the first iteration "intermediateSolution" should be cnf.Solution
    public static bool Dpll(Cnf cnf, Literal[] intermediateSolution, out Literal[] solution)
    {
        solution = new Literal[intermediateSolution.Length];
        intermediateSolution.CopyTo(solution, 0);

        //unit propagation
        foreach (var clauseLiteral in from clause in cnf.Clauses
                 where clause.IsUnitClause
                 select clause.Literals.First())
        {
            solution[clauseLiteral.Index] = clauseLiteral;

            foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(clauseLiteral)))
            {
                cnf.Clauses.Remove(clause);
            }

            var notClauseLiteral = new Literal(clauseLiteral.Index, !clauseLiteral.Sign);

            foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(notClauseLiteral)))
            {
                clause.Literals.Remove(notClauseLiteral);
            }
        }

        //pure literal elimination
        foreach (var literal in cnf.Literals)
        {
            bool isPure = (from clause in cnf.Clauses where clause.Literals.Contains(literal) select clause).All(
                clause => !clause.Literals.Any(l => l.Index == literal.Index && l.Sign != literal.Sign));
            if (isPure)
            {
                solution[literal.Index] = literal; 

                foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(literal)))
                {
                    cnf.Clauses.Remove(clause);
                }
            }
        }

        //stop conditions
        if (cnf.Clauses.Count == 0)
            return true;
        if (cnf.Clauses.Any(clause => clause.IsEmptyClause))
            return false;

        //select one literal
        Literal l = new Literal(0, true);

        for (int i = 1; i < intermediateSolution.Length; i++)
        {
            if (intermediateSolution[i].Index == 0)
            {
                l = new Literal(i, true);
                break;
            }
        }
        //if for-loop does not define l, it means that, actually, DPLL finished working already
        if (l.Equals(new Literal(0, true)))
            return true;

        //recursion
        var cnf1 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(cnf1, 0);
        cnf1[^1] = new Clause(l);

        var cnf2 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(cnf2, 0);
        cnf2[^1] = new Clause(new Literal(l.Index, !l.Sign));

        bool dpll1 = Dpll(new Cnf(cnf1), solution, out var solution1);
        bool dpll2 = Dpll(new Cnf(cnf2), solution, out var solution2);

        if (dpll1)
            solution1.CopyTo(solution, 0);
        else if (dpll2)
            solution2.CopyTo(solution, 0);

        return dpll1 || dpll2;
    }
}