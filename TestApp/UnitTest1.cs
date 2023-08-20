using SATSolver;

namespace TestApp;

public class Tests
{
    private static IEnumerable<string> _unsatCases = new[]
    {
        "TestFiles/unsat_1_2.txt", "TestFiles/unsat_2_3.txt", "TestFiles/unsat_50_100.txt",
        "TestFiles/unsat_100_160.txt", "TestFiles/unsat_100_200.txt"
    };

    [Test]
    [TestCaseSource(nameof(_unsatCases))]
    public void Test_UnsatCase_ReturnsUnsat(string filePath)
    {
        var cnf = DimacsParser.ParseFile(filePath);
        var result = Solver.SolveSat(cnf, out cnf.Solution);
        Assert.That(!result);
    }

    private static IEnumerable<string> _satCases = new[]
    {
        "TestFiles/example.txt", "TestFiles/examplePureLiteral.txt",
        "TestFiles/sat_20_91.txt", "TestFiles/sat_50_80.txt", "TestFiles/sat_50_80_2.txt", "TestFiles/sat_50_170.txt",
        "TestFiles/sat_200_1200.txt"
    };

    [Test]
    [TestCaseSource(nameof(_satCases))]
    public void Test_SatCase_ReturnsSat(string filePath)
    {
        var cnf = DimacsParser.ParseFile(filePath);
        var result = Solver.SolveSat(cnf, out cnf.Solution);

        Assert.That(result && cnf.Solution != null);
    }
}