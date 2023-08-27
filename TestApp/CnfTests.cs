namespace TestApp;

public class CnfTests
{
    private readonly string _cnfWithOneUnitClause = string.Join('\n',
        "c cnf with one unit clause = (3)",
        "p cnf 3 3",
        "3 0",
        "1 -2 3 0",
        "-1 2 -3 0"
    );

    private readonly string _cnfWithPureLiteral = string.Join('\n',
        "c cnf with pure literal = 3",
        "p cnf 3 4",
        "1 -2 -3 0",
        "-1 -2 -3 0",
        "1 2 0",
        "-1 -2 0"
    );

    private readonly string _cnfForInsert = string.Join('\n',
        "p cnf 3 3",
        "1 -2 -3 0",
        "-1 -2 3 0",
        "1 2 0"
    );
    
    [Test]
    public void UnitPropagateTest()
    {
        var cnf = DimacsParser.ParseText(_cnfWithOneUnitClause);

        while (cnf.UnitClause is { } unitClause)
        {
            var notAssigned = unitClause.Literals.First();
            cnf = cnf.UnitPropagation(notAssigned);
        }

        var expectedStr = string.Join('\n', "p cnf 2 1", "-1 2");
        var expectedCnf = DimacsParser.ParseText(expectedStr);

        Assert.That(cnf, Is.EqualTo(expectedCnf));
    }

    [Test]
    public void GetPureLiteralTest()
    {
        var cnf = DimacsParser.ParseText(_cnfWithPureLiteral);

        var expected = (3, false);
        var actual = cnf.PureLiteral;

        var isEqual = expected == actual;
        Assert.That(isEqual, Is.True);
    }

    [Test]
    public void PureLiteralsEliminationTest()
    {
        var cnf = DimacsParser.ParseText(_cnfWithPureLiteral);

        while (cnf.PureLiteral is { } pureLiteral)
        {
            cnf = cnf.PureLiteralElimination(pureLiteral);
        }

        var expectedStr = string.Join('\n', "p cnf 2 2", "1 2 0", "-1 -2 0");
        var expectedCnf = DimacsParser.ParseText(expectedStr);

        Assert.That(cnf.Clauses.SetEquals(expectedCnf.Clauses));
    }
}