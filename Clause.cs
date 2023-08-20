namespace SATSolver;

public class Clause
{
    public HashSet<Literal> Literals;
    public bool IsUnitClause => Literals.Count == 1;
    public bool IsEmptyClause => Literals.Count == 0;

    public Clause(params Literal[] literals)
    {
        Literals = new HashSet<Literal>(literals);
    }
    
    public Clause(IEnumerable<Literal> literals)
    {
        Literals = new HashSet<Literal>(literals);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (GetType() != obj.GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;
    
        var other = (Clause)obj;
    
        return Literals.SequenceEqual(other.Literals);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Literals);
    }
}