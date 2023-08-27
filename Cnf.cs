namespace SatSolverLib;

public class Cnf
{
    public HashSet<Clause> Clauses;
    public HashSet<(int, bool)> Literals;
    public int CountVars;

    public Cnf(IEnumerable<Clause> clauses)
    {
        Clauses = new HashSet<Clause>(clauses);
        Literals = Clauses.SelectMany(clause => clause.Literals).Distinct().ToHashSet();
        CountVars = Literals.Select(literal => literal.Item1).Distinct().Count();
    }

    public Clause? UnitClause => Clauses.FirstOrDefault(clause => clause.IsUnitClause);
    public (int, bool)? PureLiteral => GetPureLiteral();

    private (int, bool)? GetPureLiteral()
    {
        foreach (var literal in Literals)
        {
            bool isPure = !Literals.Contains((literal.Item1, !literal.Item2));

            if (isPure)
                return literal;
        }

        return null;
    }

    public override string ToString()
    {
        return Clauses.Count == 0 ? "EmptyCNF" : Clauses.Aggregate($"{CountVars}", (current, clause) => current + (clause + " "));
    }

    public Cnf UnitPropagation((int, bool) unitLiteral)
    {
        var newClauses = new HashSet<Clause>(Clauses);
        foreach (var clause in Clauses)
        {
            if (clause.Literals.Contains((unitLiteral.Item1, unitLiteral.Item2)))
            {
                newClauses.Remove(clause);
            }
            else if (clause.Literals.Contains((unitLiteral.Item1, !unitLiteral.Item2)))
                clause.Literals.Remove((unitLiteral.Item1, !unitLiteral.Item2));
        }

        return new Cnf(newClauses);
    }

    public Cnf PureLiteralElimination((int, bool) pureLiteral)
    {
        var newClauses = new HashSet<Clause>(Clauses);
        newClauses.RemoveWhere(clause => clause.Literals.Contains(pureLiteral));
        return new Cnf(newClauses);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (this.GetType() != obj.GetType()) return false;
        if (ReferenceEquals(obj, this)) return true;

        var other = (Cnf)obj;

        return this.Clauses.Count == other.Clauses.Count && this.Clauses.SetEquals(other.Clauses);
    }

    public override int GetHashCode() => HashCode.Combine(Clauses);
}