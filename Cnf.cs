namespace SatSolverLib;

public class Cnf
{
    public HashSet<Clause> Clauses;
    public Literal[] Literals;
    public int CountVars;

    public Cnf(IEnumerable<Clause> clauses, int countVars)
    {
        Clauses = new HashSet<Clause>(clauses);
        Literals = Clauses.SelectMany(clause => clause.Literals).Distinct().ToArray();
        CountVars = countVars;
    }

    public static bool Dpll(Cnf cnf, Literal[] intermediateSolution, out Literal[] solution)
    {
        solution = new Literal[intermediateSolution.Length];
        intermediateSolution.CopyTo(solution, 0);

        //unit propagation
        while (cnf.Literals.Length > 0)
        {
            var unitLiterals = from clause in cnf.Clauses
                where clause.IsUnitClause
                select clause.Literals.First();
            var array = unitLiterals as Literal[] ??
                        unitLiterals.ToArray(); //arraying "to avoid possible reenumeration"
            if (!array.Any())
                break;
            var clauseLiteral = array.First();

            if (solution[clauseLiteral.Index].IsComplement(clauseLiteral))
                return false;

            solution[clauseLiteral.Index] = new Literal(clauseLiteral.Index, clauseLiteral.Sign);

            foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(clauseLiteral)))
            {
                cnf.Clauses.Remove(clause);
            }

            foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(clauseLiteral.Complement())))
            {
                clause.Literals.Remove(clauseLiteral.Complement());
            }

            cnf.Literals = cnf.Clauses.SelectMany(clause2 => clause2.Literals).Distinct().ToArray();
        }

        //pure literal elimination
        while (cnf.Literals.Length > 0)
        {
            int i = 0;
            foreach (var literal in cnf.Literals)
            {
                bool isPure = !cnf.Literals.Contains(literal.Complement());
                // (from clause in cnf.Clauses where clause.Literals.Contains(literal) select clause).All(
                //     clause => !clause.Literals.Any(l => l.Index == literal.Index && l.Sign != literal.Sign));
                if (isPure)
                {
                    if (solution[literal.Index].IsComplement(literal))
                        return false;

                    solution[literal.Index] = new Literal(literal.Index, literal.Sign);

                    foreach (var clause in cnf.Clauses.Where(clause => clause.Literals.Contains(literal)))
                    {
                        cnf.Clauses.Remove(clause);
                    }

                    cnf.Literals = cnf.Clauses.SelectMany(clause2 => clause2.Literals).Distinct().ToArray();
                    i++;
                    break;
                }
            }

            if (i == 0)
                break;
        }

        //stop conditions
        if (cnf.Clauses.Count == 0)
            return true;
        if (cnf.Clauses.Any(clause => clause.IsEmptyClause))
            return false;

        //select one literal
        
        // Literal l = new Literal(0, false);
        //
        // for (int i = 1; i < intermediateSolution.Length; i++)
        // {
        //     if (intermediateSolution[i].Index == 0)
        //     {
        //         l = new Literal(i, true);
        //         break;
        //     }
        // }
        Literal l = new Literal(cnf.Literals.First().Index, cnf.Literals.First().Sign);
        
        //recursion
        var clauses1 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(clauses1, 0);
        clauses1[^1] = new Clause(l);

        var clauses2 = new Clause[cnf.Clauses.Count + 1];
        cnf.Clauses.CopyTo(clauses2, 0);
        clauses2[^1] = new Clause(l.Complement());

        bool dpll1 = Dpll(new Cnf(clauses1, cnf.CountVars), solution, out var solution1);
        bool dpll2 = Dpll(new Cnf(clauses2, cnf.CountVars), solution, out var solution2);

        if (dpll1)
            solution1.CopyTo(solution, 0);
        else if (dpll2)
            solution2.CopyTo(solution, 0);

        return dpll1 || dpll2;
    }

    public override string ToString()
    {
        return Clauses.Count == 0 ? "EmptyCNF" : Clauses.Aggregate("", (current, clause) => current + (clause + " "));
    }
}