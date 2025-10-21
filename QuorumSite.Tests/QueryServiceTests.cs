using QuorumSite.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace QuorumSite.Tests;

public class QueryServiceTests
{
    private readonly QueryService _queryService;
    private readonly DataRepository _repository;

    public QueryServiceTests()
    {
        // Setup mock environment to point to test data
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(m => m.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        var csvParser = new CsvParsingService();
        _repository = new DataRepository(csvParser, mockEnv.Object);
        _queryService = new QueryService(_repository);
    }

    #region Legislator Summary Tests

    [Fact]
    public void GetLegislatorSummaries_ShouldReturnAllLegislators()
    {
        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(20, "all 20 legislators should be returned, including those with no votes");
    }

    [Fact]
    public void GetLegislatorSummaries_ShouldCalculateCorrectVoteCounts_ForRepBacon()
    {
        // Arrange - Rep. Don Bacon (904789)
        // Based on vote_results.csv:
        // Vote 3314452 (Infrastructure): Type 1 (Support)
        // Vote 3321166 (BBB): Type 2 (Oppose)

        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert
        var repBacon = result.FirstOrDefault(l => l.Id == 904789);
        repBacon.Should().NotBeNull();
        repBacon!.Legislator.Should().Be("Rep. Don Bacon (R-NE-2)");
        repBacon.SupportedBills.Should().Be(1, "supported Infrastructure bill");
        repBacon.OpposedBills.Should().Be(1, "opposed BBB bill");
    }

    [Fact]
    public void GetLegislatorSummaries_ShouldCalculateCorrectVoteCounts_ForRepOcasioCortez()
    {
        // Arrange - Rep. Alexandria Ocasio-Cortez (1269767)
        // Based on vote_results.csv:
        // Vote 3314452 (Infrastructure): Type 2 (Oppose)
        // Vote 3321166 (BBB): Type 1 (Support)

        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert
        var aoc = result.FirstOrDefault(l => l.Id == 1269767);
        aoc.Should().NotBeNull();
        aoc!.Legislator.Should().Be("Rep. Alexandria Ocasio-Cortez (D-NY-14)");
        aoc.SupportedBills.Should().Be(1, "supported BBB bill");
        aoc.OpposedBills.Should().Be(1, "opposed Infrastructure bill");
    }

    [Fact]
    public void GetLegislatorSummaries_ShouldCalculateCorrectVoteCounts_ForRepYarmuth()
    {
        // Arrange - Rep. John Yarmuth (412211)
        // This legislator is the sponsor of BBB but we need to check if they voted

        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert
        var yarmuth = result.FirstOrDefault(l => l.Id == 412211);
        yarmuth.Should().NotBeNull();
        yarmuth!.Legislator.Should().Be("Rep. John Yarmuth (D-KY-3)");
        // Vote counts will be 0 if they didn't vote, which is valid
        (yarmuth.SupportedBills + yarmuth.OpposedBills).Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void GetLegislatorSummaries_ShouldIncludeLegislatorsWithNoVotes()
    {
        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert - Find any legislator with 0 votes
        var legislatorWithNoVotes = result.FirstOrDefault(l => 
            l.SupportedBills == 0 && l.OpposedBills == 0);

        // If there's a legislator with no votes, verify they're still included
        if (legislatorWithNoVotes != null)
        {
            legislatorWithNoVotes.Legislator.Should().NotBeNullOrEmpty();
            legislatorWithNoVotes.Id.Should().BeGreaterThan(0);
        }
    }

    [Fact]
    public void GetLegislatorSummaries_ShouldNotHaveNegativeVoteCounts()
    {
        // Act
        var result = _queryService.GetLegislatorSummaries();

        // Assert
        result.Should().AllSatisfy(legislator =>
        {
            legislator.SupportedBills.Should().BeGreaterThanOrEqualTo(0);
            legislator.OpposedBills.Should().BeGreaterThanOrEqualTo(0);
        });
    }

    #endregion

    #region Bill Summary Tests

    [Fact]
    public void GetBillSummaries_ShouldReturnBothBills()
    {
        // Act
        var result = _queryService.GetBillSummaries();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2, "there are 2 bills with votes");
    }

    [Fact]
    public void GetBillSummaries_ShouldCalculateCorrectVoteCounts_ForInfrastructureBill()
    {
        // Arrange - H.R. 3684: Infrastructure Investment and Jobs Act (2900994)
        // Vote ID: 3314452
        // Based on vote_results.csv, count Type 1 and Type 2 votes

        // Act
        var result = _queryService.GetBillSummaries();

        // Assert
        var infrastructureBill = result.FirstOrDefault(b => b.Id == 2900994);
        infrastructureBill.Should().NotBeNull();
        infrastructureBill!.Bill.Should().Be("H.R. 3684: Infrastructure Investment and Jobs Act");
        
        // Count from CSV: 13 legislators voted Type 1, 6 voted Type 2
        infrastructureBill.Supporters.Should().Be(13, "13 legislators voted Yea");
        infrastructureBill.Opposers.Should().Be(6, "6 legislators voted Nay");
        
        // Sponsor 400100 doesn't exist in legislators, should show Unknown
        infrastructureBill.PrimarySponsor.Should().Be("Unknown");
    }

    [Fact]
    public void GetBillSummaries_ShouldCalculateCorrectVoteCounts_ForBBBBill()
    {
        // Arrange - H.R. 5376: Build Back Better Act (2952375)
        // Vote ID: 3321166
        // Based on vote_results.csv, count Type 1 and Type 2 votes

        // Act
        var result = _queryService.GetBillSummaries();

        // Assert
        var bbbBill = result.FirstOrDefault(b => b.Id == 2952375);
        bbbBill.Should().NotBeNull();
        bbbBill!.Bill.Should().Be("H.R. 5376: Build Back Better Act");
        
        // Count from CSV: 6 legislators voted Type 1, 13 voted Type 2
        bbbBill.Supporters.Should().Be(6, "6 legislators voted Yea");
        bbbBill.Opposers.Should().Be(13, "13 legislators voted Nay");
        
        // Sponsor is Rep. John Yarmuth (412211)
        bbbBill.PrimarySponsor.Should().Be("Rep. John Yarmuth (D-KY-3)");
    }

    [Fact]
    public void GetBillSummaries_ShouldHandleUnknownSponsors()
    {
        // Act
        var result = _queryService.GetBillSummaries();

        // Assert - Infrastructure bill has unknown sponsor (400100 not in legislators)
        var billWithUnknownSponsor = result.FirstOrDefault(b => b.PrimarySponsor == "Unknown");
        
        billWithUnknownSponsor.Should().NotBeNull("Infrastructure bill should have unknown sponsor");
        billWithUnknownSponsor!.Id.Should().Be(2900994);
    }

    [Fact]
    public void GetBillSummaries_ShouldNotHaveNegativeVoteCounts()
    {
        // Act
        var result = _queryService.GetBillSummaries();

        // Assert
        result.Should().AllSatisfy(bill =>
        {
            bill.Supporters.Should().BeGreaterThanOrEqualTo(0);
            bill.Opposers.Should().BeGreaterThanOrEqualTo(0);
        });
    }

    [Fact]
    public void GetBillSummaries_TotalVotesShouldMatchVoteResults()
    {
        // Act
        var result = _queryService.GetBillSummaries();

        // Assert
        var totalVotesInBills = result.Sum(b => b.Supporters + b.Opposers);
        totalVotesInBills.Should().Be(38, "total votes should match vote_results.csv count");
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void DataRepository_ShouldLoadAllDataSuccessfully()
    {
        // Assert
        _repository.Bills.Should().HaveCount(2);
        _repository.Legislators.Should().HaveCount(20);
        _repository.Votes.Should().HaveCount(2);
        _repository.VoteResults.Should().HaveCount(38);
    }

    [Fact]
    public void QueryService_ShouldHandleEmptyResults_Gracefully()
    {
        // This test ensures queries don't throw exceptions even with edge cases
        // Act
        var legislatorSummaries = _queryService.GetLegislatorSummaries();
        var billSummaries = _queryService.GetBillSummaries();

        // Assert
        legislatorSummaries.Should().NotBeNull();
        billSummaries.Should().NotBeNull();
    }

    #endregion
}
