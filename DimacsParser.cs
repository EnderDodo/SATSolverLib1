using System.Text;

namespace SatSolverLib;

public static class DimacsParser
{
    private const char CommentChar = 'c';
    private const string AmountLineStarter = "p cnf";
    private const string LineEndChar = "0";

    public static Cnf ParseText(string text)
    {
        var lines = text.Split("\n");
        return ParseLines(lines);
    }
    
    public static Cnf ParseFile(string filePath)
    {
        var lines = File.ReadLines(filePath);
        return ParseLines(lines);
    }

    private static Cnf ParseLines(IEnumerable<string> lines)
    {
        var clauses = new List<Clause>();
        var countClauses = -1;
        var countVars = -1;
        var readLines = 0;
        foreach (var line in lines)
        {
            if (readLines == countClauses)
                break;

            if (line[0] == CommentChar)
                continue;

            if (line.StartsWith(AmountLineStarter))
            {
                var splitLine = line.Split();
                countVars = Convert.ToInt32(splitLine[2]);
                countClauses = Convert.ToInt32(splitLine[3]);
                continue;
            }

            var literals = new List<int>();

            foreach (var literalStr in line.Split())
            {
                if (literalStr == LineEndChar)
                    break;

                var value = Convert.ToInt32(literalStr);

                if (Math.Abs(value) > countVars)
                    throw new ArgumentException($"Too large index; absolute value of {value} is greater than {countVars}");

                literals.Add(value);
            }

            clauses.Add(new Clause(literals));
            readLines++;
        }

        return new Cnf(clauses, countVars);
    }
}