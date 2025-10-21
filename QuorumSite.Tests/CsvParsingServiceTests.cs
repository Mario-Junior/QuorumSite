using QuorumSite.Services;
using QuorumSite.Models;

namespace QuorumSite.Tests;

public class CsvParsingServiceTests
{
    private readonly CsvParsingService _service;
    private readonly string _dataPath;

    public CsvParsingServiceTests()
    {
        _service = new CsvParsingService();
        _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
    }

    [Fact]
    public void ParseCsv_ShouldParseLegislators_WithCorrectCount()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "legislators.csv");

        // Act
        var result = _service.ParseCsv<Legislator>(filePath);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(20, "there are exactly 20 legislators in the CSV file");
    }

    [Fact]
    public void ParseCsv_ShouldParseLegislators_WithCorrectData()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "legislators.csv");

        // Act
        var result = _service.ParseCsv<Legislator>(filePath);

        // Assert - Check specific legislators
        var repYarmuth = result.FirstOrDefault(l => l.Id == 412211);
        repYarmuth.Should().NotBeNull();
        repYarmuth!.Name.Should().Be("Rep. John Yarmuth (D-KY-3)");

        var repOcasioCortez = result.FirstOrDefault(l => l.Id == 1269767);
        repOcasioCortez.Should().NotBeNull();
        repOcasioCortez!.Name.Should().Be("Rep. Alexandria Ocasio-Cortez (D-NY-14)");
    }

    [Fact]
    public void ParseCsv_ShouldParseBills_WithCorrectCount()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "bills.csv");

        // Act
        var result = _service.ParseCsv<Bill>(filePath);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2, "there are exactly 2 bills in the CSV file");
    }

    [Fact]
    public void ParseCsv_ShouldParseBills_WithCorrectData()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "bills.csv");

        // Act
        var result = _service.ParseCsv<Bill>(filePath);

        // Assert - Check H.R. 5376
        var hr5376 = result.FirstOrDefault(b => b.Id == 2952375);
        hr5376.Should().NotBeNull();
        hr5376!.Title.Should().Be("H.R. 5376: Build Back Better Act");
        hr5376.PrimarySponsor.Should().Be(412211);

        // Assert - Check H.R. 3684
        var hr3684 = result.FirstOrDefault(b => b.Id == 2900994);
        hr3684.Should().NotBeNull();
        hr3684!.Title.Should().Be("H.R. 3684: Infrastructure Investment and Jobs Act");
        hr3684.PrimarySponsor.Should().Be(400100);
    }

    [Fact]
    public void ParseCsv_ShouldParseVotes_WithCorrectCount()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "votes.csv");

        // Act
        var result = _service.ParseCsv<Vote>(filePath);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2, "there are exactly 2 votes in the CSV file");
    }

    [Fact]
    public void ParseCsv_ShouldParseVotes_WithCorrectMapping()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "votes.csv");

        // Act
        var result = _service.ParseCsv<Vote>(filePath);

        // Assert
        var vote1 = result.FirstOrDefault(v => v.Id == 3314452);
        vote1.Should().NotBeNull();
        vote1!.Bill_Id.Should().Be(2900994);

        var vote2 = result.FirstOrDefault(v => v.Id == 3321166);
        vote2.Should().NotBeNull();
        vote2!.Bill_Id.Should().Be(2952375);
    }

    [Fact]
    public void ParseCsv_ShouldParseVoteResults_WithCorrectCount()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "vote_results.csv");

        // Act
        var result = _service.ParseCsv<VoteResult>(filePath);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(38, "there are exactly 38 vote results in the CSV file");
    }

    [Fact]
    public void ParseCsv_ShouldParseVoteResults_WithValidVoteTypes()
    {
        // Arrange
        var filePath = Path.Combine(_dataPath, "vote_results.csv");

        // Act
        var result = _service.ParseCsv<VoteResult>(filePath);

        // Assert - Vote type must be 1 (Yea) or 2 (Nay)
        result.Should().AllSatisfy(vr =>
        {
            vr.Vote_Type.Should().BeOneOf(1, 2);
        });
    }
}
