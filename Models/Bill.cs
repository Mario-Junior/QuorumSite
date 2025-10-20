namespace QuorumSite.Models;

public class Bill
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int PrimarySponsor { get; set; }  // legislator_id
}
