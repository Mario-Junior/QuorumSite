using QuorumSite.Models;

namespace QuorumSite.Services;

public class DataRepository
{
    private readonly CsvParsingService _csvParser;
    private readonly string _dataPath;

    public List<Bill> Bills { get; private set; } = new();
    public List<Legislator> Legislators { get; private set; } = new();
    public List<Vote> Votes { get; private set; } = new();
    public List<VoteResult> VoteResults { get; private set; } = new();

    public DataRepository(CsvParsingService csvParser, IWebHostEnvironment env)
    {
        _csvParser = csvParser;
        _dataPath = Path.Combine(env.ContentRootPath, "Data");
        LoadData();
    }

    private void LoadData()
    {
        Bills = _csvParser.ParseCsv<Bill>(Path.Combine(_dataPath, "bills.csv"));
        Legislators = _csvParser.ParseCsv<Legislator>(Path.Combine(_dataPath, "legislators.csv"));
        Votes = _csvParser.ParseCsv<Vote>(Path.Combine(_dataPath, "votes.csv"));
        VoteResults = _csvParser.ParseCsv<VoteResult>(Path.Combine(_dataPath, "vote_results.csv"));
    }
}
