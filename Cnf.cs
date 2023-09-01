namespace SatSolverLib;

public class Cnf
{
    public HashSet<Clause> Clauses;
    public HashSet<int> Literals;
    public int CountVars;

    public Cnf(IEnumerable<Clause> clauses)
    {
        Clauses = new HashSet<Clause>(clauses);
        Literals = Clauses.SelectMany(clause => clause.Literals).Distinct().ToHashSet();
        CountVars = Literals.Select(Math.Abs).Distinct().Count();
    }

    public Cnf(IEnumerable<Clause> clauses, int countVars) : this(clauses)
    {
        CountVars = countVars;
    }

    public Clause? UnitClause => Clauses.FirstOrDefault(clause => clause.IsUnitClause);
    public int PureLiteral => GetPureLiteral();

    private int GetPureLiteral()
    {
        foreach (var literal in Literals)
        {
            bool isPure = !Literals.Contains(-literal);

            if (isPure)
                return literal;
        }

        return 0;
    }

    public override string ToString()
    {
        return Clauses.Count == 0 ? "EmptyCNF" : Clauses.Aggregate($"{CountVars}", (current, clause) => current + (clause + "\n "));
    }

    public Cnf UnitPropagation(int unitLiteral)
    {
        var newClauses = new HashSet<Clause>(Clauses);
        foreach (var clause in Clauses)
        {
            if (clause.Literals.Contains(unitLiteral))
            {
                newClauses.Remove(clause);
            }
            else if (clause.Literals.Contains(-unitLiteral))
                clause.Literals.Remove(-unitLiteral);
        }

        return new Cnf(newClauses);
    }

    public Cnf PureLiteralElimination(int pureLiteral)
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

        return this.Clauses.SetEquals(other.Clauses);
    }

    public override int GetHashCode() => 3;
}