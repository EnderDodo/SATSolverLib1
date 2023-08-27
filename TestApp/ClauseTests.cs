namespace TestApp;

public class ClauseTests
{
    [Test]
    public void Test_IsUnitClause()
    {
        var unitClause = new Clause((1, true));
        Assert.That(unitClause.IsUnitClause);
    }
    [Test]
    public void Test_IsNotUnitClause_IsNotEmptyClause()
    {
        var clause = new Clause((1, true), (2, false));
        Assert.That(clause is { IsUnitClause: false, IsEmptyClause: false });
    }
    [Test]
    public void Test_IsEmptyClause()
    {
        var unitClause = new Clause();
        Assert.That(unitClause.IsEmptyClause);
    }

    [Test]
    public void Test_Equals()
    {
        var clause1 = new Clause((1, true), (2, false));
        var clause2 = new Clause((1, true), (2, false), (3, true));
        clause2.Literals.Remove((3, true));
        Assert.That(clause1, Is.EqualTo(clause2));
    }
    [Test]
    public void Test_Tuple_Equals()
    {
        var literal1 = (5, true);
        var literal2 = (5, true);
        Assert.That(literal1, Is.EqualTo(literal2));
    }
}