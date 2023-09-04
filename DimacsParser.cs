using System.Collections.Immutable;
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

    public static void WriteModelToConsole(bool[] model)
    {
        if (!model[0])
        {
            Console.WriteLine("s NOT SATISFIABLE");
        }
        else
        {
            var builder = new StringBuilder("s SATISFIABLE");
            builder.Append('\n');
            builder.Append("v ");
            for (int i = 1; i < model.Length; i++)
            {
                builder.Append(model[i] ? i : -i);
                builder.Append(' ');
            }

            builder.Append(0);
            Console.WriteLine(builder.ToString());
        }
    }
}