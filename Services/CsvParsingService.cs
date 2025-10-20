using CsvHelper;
using CsvHelper.Configuration;
using QuorumSite.Models;
using System.Globalization;

namespace QuorumSite.Services;

public class CsvParsingService
{
    public List<T> ParseCsv<T>(string filePath)
    {
        using var reader = new StreamReader(filePath);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null,
            PrepareHeaderForMatch = args => args.Header.ToLower()
        };

        using var csv = new CsvReader(reader, config);
        
        // Register class maps
        csv.Context.RegisterClassMap<BillMap>();
        csv.Context.RegisterClassMap<LegislatorMap>();
        csv.Context.RegisterClassMap<VoteMap>();
        csv.Context.RegisterClassMap<VoteResultMap>();
        
        return csv.GetRecords<T>().ToList();
    }
}

// Class Maps for proper CSV column mapping
public sealed class BillMap : ClassMap<Bill>
{
    public BillMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Title).Name("title");
        Map(m => m.PrimarySponsor).Name("sponsor_id");
    }
}

public sealed class LegislatorMap : ClassMap<Legislator>
{
    public LegislatorMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Name).Name("name");
    }
}

public sealed class VoteMap : ClassMap<Vote>
{
    public VoteMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Bill_Id).Name("bill_id");
    }
}

public sealed class VoteResultMap : ClassMap<VoteResult>
{
    public VoteResultMap()
    {
        Map(m => m.Id).Name("id");
        Map(m => m.Legislator_Id).Name("legislator_id");
        Map(m => m.Vote_Id).Name("vote_id");
        Map(m => m.Vote_Type).Name("vote_type");
    }
}
