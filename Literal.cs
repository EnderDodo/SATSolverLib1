namespace SatSolverLib;

public class Literal
{
    public int Index { get; }
    public bool Sign { get; }

    public Literal(int index, bool sign)
    {
        Index = index;
        Sign = sign;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null)) return false;
        if (GetType() != obj.GetType()) return false;
        if (ReferenceEquals(this, obj)) return true;
    
        var other = (Literal)obj;
    
        return Index.Equals(other.Index) && Sign.Equals(other.Sign);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Sign);
    }

    public bool IsComplement(Literal literal)
    {
        return Index.Equals(literal.Index) && Sign.Equals(!literal.Sign);
    }

    public Literal Complement()
    {
        return new Literal(Index, !Sign);
    }

    public override string ToString()
    {
        string result = "";
        if (!Sign) result += '-';
        return result + Index;
    }
}