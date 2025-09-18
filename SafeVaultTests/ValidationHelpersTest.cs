using SafeVault.Security;
public class ValidationHelpersTest
{
    // Tests for IsValidInput
    [Theory]
    [InlineData("ValidInput123", "", true)]
    [InlineData("Invalid Input!", "", false)]
    [InlineData("Special@Char", "@", true)]
    [InlineData("", "", false)]
    [InlineData(null, "", false)]
    public void IsValidInput_Tests(string input, string allowedSpecialChars, bool expected)
    {
        var result = ValidationHelpers.IsValidInput(input, allowedSpecialChars);
        Assert.Equal(expected, result);
    }

    // Tests for IsValidXSSInput
    [Theory]
    [InlineData("Normal text", true)]
    [InlineData("<script>alert('XSS');</script>", false)]
    [InlineData("<iframe src='malicious.com'></iframe>", false)]
    [InlineData("Safe <b>bold</b> text", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    public void IsValidXSSInput_Tests(string input, bool expected)
    {
        var result = ValidationHelpers.IsValidXSSInput(input);
        Assert.Equal(expected, result);
    }

}