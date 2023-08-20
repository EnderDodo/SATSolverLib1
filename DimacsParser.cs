using System.Text;
using SatSolverLib;

namespace SATSolver;

public static class DimacsParser
{
    private const char CommentChar = 'c';
    private const string AmountLineStarter = "p cnf";
    private const string LineEndChar = "0";


    private const string Sat = "s SATISFIABLE";
    private const string Unsat = "s NOT SATISFIABLE";

    private const char AnswerStartChar = 'v';

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

            var literals = new List<Literal>();

            foreach (var literalStr in line.Split())
            {
                if (literalStr == LineEndChar)
                    break;

                var index = Convert.ToInt32(literalStr);

                if (Math.Abs(index) > countVars)
                    throw new ArgumentException($"Too large index; absolute value of {index} is greater than {countVars}");

                literals.Add(new Literal(Math.Abs(index), index > 0));
            }

            clauses.Add(new Clause(literals));
            readLines++;
        }

        return new Cnf(clauses);
    }
    
    public static void WriteModel(Action<string> writer, List<(int, bool)>? model)
    {
        if (model == null)
        {
            writer.Invoke(Unsat);
            return;
        }

        var builder = new StringBuilder(Sat);
        builder.Append('\n');
        builder.Append(AnswerStartChar);
        builder.Append(' ');

        foreach (var literalValue in model)
        {
            var abs = Math.Abs(literalValue.Item1);
            builder.Append(literalValue.Item2 ? abs : -abs);
            builder.Append(' ');
        }

        builder.Append(LineEndChar);
        writer.Invoke(builder.ToString());
    }
}