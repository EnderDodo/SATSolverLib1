namespace TestApp;

public class ParserTests
{
    public string text = String.Join('\n',
        "c SAT",
        "p cnf 8 5",
        "4 -5 3 0",
        "-5 -2 -4 0",
        "8 -1 2 0",
        "-3 5 6 0",
        "2 1 8 0");

    [Test]
    public void Test_ParseText()
    {
        var cnf = DimacsParser.ParseText(text);
        var isSat = Solver.Dpll(cnf, out var solution);
        Assert.That(isSat, Is.True);
    }
}