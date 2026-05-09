namespace Lab10.Tests;

public class SmokeTest
{
    [Fact]
    public void TestInfrastructure_Works()
    {
        // Test banal: daca aceasta rulare trece, xUnit + referinta la Lab10 sunt ok.
        Assert.Equal(2, 1 + 1);
    }
}
