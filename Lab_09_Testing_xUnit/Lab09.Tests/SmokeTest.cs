namespace Lab09.Tests;

public class SmokeTest
{
    [Fact]
    public void TestInfrastructure_Works()
    {
        // Test banal: daca aceasta rulare trece, xUnit + referinta la Lab09 sunt ok.
        Assert.Equal(2, 1 + 1);
    }
}
