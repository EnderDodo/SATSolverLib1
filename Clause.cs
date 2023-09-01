namespace SatSolverLib;

public class Clause
{
    public HashSet<int> Literals;
    public bool IsUnitClause => Literals.Count == 1;
    public bool IsEmptyClause => Literals.Count == 0;

    public Clause(params int[] literals)
    {
        Literals = new HashSet<int>(literals);
    }
    
    public Clause(IEnumerable<int> literals)
    {
        Literals = new HashSet<int>(literals);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (GetType() != obj.GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;
    
        var other = (Clause)obj;
    
        return Literals.SetEquals(other.Literals);
    }
    public override int GetHashCode() => 2;
    

    public override string ToString()
    {
        if (IsEmptyClause) return "(emptyClause)";
        string result = Literals.Aggregate("(", (current, l) => current + (l + ", "));
        return result.Remove(result.Length - 2) + ')';
    }

}