namespace QuorumSite.Models;

public class VoteResult
{
    public int Id { get; set; }
    public int Legislator_Id { get; set; }
    public int Vote_Id { get; set; }
    public int Vote_Type { get; set; } // 1 = Yea, 2 = Nay
}
