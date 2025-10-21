namespace QuorumSite.Services;

public class QueryService
{
    private readonly DataRepository _repository;

    public QueryService(DataRepository repository)
    {
        _repository = repository;
    }

    public List<LegislatorSummary> GetLegislatorSummaries()
    {
        // Start with ALL legislators and perform LEFT JOIN with votes
        // To include legislators who haven't voted on any bill

        var results = from legislator in _repository.Legislators
                      join voteResult in _repository.VoteResults on legislator.Id equals voteResult.Legislator_Id into voteGroup
                      from voteResult in voteGroup.DefaultIfEmpty()
                      join vote in _repository.Votes on voteResult?.Vote_Id equals vote.Id into votes
                      from vote in votes.DefaultIfEmpty()
                      group voteResult by new { legislator.Id, legislator.Name } into g
                      select new LegislatorSummary
                      {
                          Id = g.Key.Id,
                          Legislator = g.Key.Name,
                          SupportedBills = g.Count(vr => vr != null && vr.Vote_Type == 1),
                          OpposedBills = g.Count(vr => vr != null && vr.Vote_Type == 2)
                      };

        return results.ToList();
    }

    public List<BillSummary> GetBillSummaries()
    {
        // VoteResult → Vote (get bill_id)
        // Group by bill_id
        // Count Vote_Type == 1 and == 2
        // Left Join with Bill.PrimarySponsor → Legislator.Name (to include bills without known sponsor)

        var results = from voteResult in _repository.VoteResults
                      join vote in _repository.Votes on voteResult.Vote_Id equals vote.Id
                      join bill in _repository.Bills on vote.Bill_Id equals bill.Id
                      group voteResult by new { bill.Id, bill.Title, bill.PrimarySponsor } into g
                      join sponsor in _repository.Legislators on g.Key.PrimarySponsor equals sponsor.Id into sponsorGroup
                      from sponsor in sponsorGroup.DefaultIfEmpty()
                      select new BillSummary
                      {
                          Id = g.Key.Id,
                          Bill = g.Key.Title,
                          Supporters = g.Count(vr => vr.Vote_Type == 1),
                          Opposers = g.Count(vr => vr.Vote_Type == 2),
                          PrimarySponsor = sponsor != null ? sponsor.Name : "Unknown"
                      };

        return results.ToList();
    }
}

public class LegislatorSummary
{
    public int Id { get; set; }
    public string Legislator { get; set; } = "";
    public int SupportedBills { get; set; }
    public int OpposedBills { get; set; }
}

public class BillSummary
{
    public int Id { get; set; }
    public string Bill { get; set; } = "";
    public int Supporters { get; set; }
    public int Opposers { get; set; }
    public string PrimarySponsor { get; set; } = "";
}
