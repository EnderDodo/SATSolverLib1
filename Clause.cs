namespace SatSolverLib;

public class Clause
{
    public HashSet<(int, bool)> Literals;
    public bool IsUnitClause => Literals.Count == 1;
    public bool IsEmptyClause => Literals.Count == 0;

    public Clause(params (int, bool)[] literals)
    {
        Literals = new HashSet<(int, bool)>(literals);
    }
    
    public Clause(IEnumerable<(int, bool)> literals)
    {
        Literals = new HashSet<(int, bool)>(literals);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (GetType() != obj.GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;
    
        var other = (Clause)obj;
    
        return Literals.SetEquals(other.Literals);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Literals);
    }

    public override string ToString()
    {
        if (IsEmptyClause) return "(emptyClause)";
        string result = Literals.Aggregate("(", (current, l) => current + (l + ", "));
        return result.Remove(result.Length - 2) + ')';
    }

}