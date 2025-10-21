# Technical Write-Up: QuorumSite Implementation

## 1. Strategy and Development Decisions

### Architecture & Technology Stack

I chose **ASP.NET Core 8 Minimal API** for its modern, lightweight approach and excellent performance characteristics. The architecture follows clean separation of concerns with three distinct layers:

- **Models Layer**: Plain data structures (DTOs) representing Bills, Legislators, Votes, and VoteResults
- **Services Layer**: Business logic separated into specialized services:
  - `CsvParsingService`: Handles CSV parsing using CsvHelper library
  - `DataRepository`: In-memory singleton storage with one-time data loading at startup
  - `QueryService`: LINQ-based aggregation and analysis logic
- **API Layer**: RESTful endpoints exposing summarized data

### Time Complexity Considerations

The solution prioritizes **O(1) read operations** at runtime by performing all expensive operations during startup:
- CSV parsing: **O(n)** - executed once at application startup
- Data storage: **O(1)** lookups using Dictionary structures indexed by IDs
- Query aggregation: **O(n)** - using efficient LINQ operations with joins
- API responses: **O(1)** - data pre-aggregated and cached in memory

This design trades slightly longer startup time (~100ms) for blazing-fast API responses (~1-5ms), which is ideal for a web application serving multiple concurrent users.

### Technology Decisions

- **CsvHelper**: Industry-standard library with robust CSV parsing, handling edge cases like quoted fields and custom delimiters
- **xUnit + FluentAssertions**: Modern testing stack providing readable assertions and excellent IDE integration
- **Chart.js**: Lightweight, zero-dependency visualization library for the frontend
- **Docker Multi-Stage Builds**: Optimized images (~200MB final size) by separating build and runtime environments

### Testing Strategy

I implemented **22 comprehensive unit tests** (100% pass rate) covering:
- All CSV parsing scenarios (8 tests)
- Business logic validation (14 tests)
- Edge cases: missing sponsors, legislators without votes, negative vote validation
- Data integrity: ensuring totals match source CSV records

### Effort Cost Analysis

- Initial setup & architecture: ~30 minutes
- Core CSV parsing implementation: ~45 minutes
- Business logic & LINQ queries: ~1 hour
- Frontend dashboard with Chart.js: ~45 minutes
- Comprehensive test suite: ~1.5 hours
- Docker configuration & optimization: ~30 minutes
- Documentation (README with detailed setup): ~30 minutes

**Total estimated time: ~5-6 hours**

---

## 2. Handling Future Column Additions

To accommodate new columns like **"Bill Voted On Date"** or **"Co-Sponsors"**, I would implement the following changes:

### Immediate Changes (Backward Compatible)

```csharp
// Update Models
public class Vote
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int LegislatorId { get; set; }
    public DateTime? VotedOnDate { get; set; }  // New nullable field
}

public class Bill
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int PrimarySponsor { get; set; }
    public List<int> CoSponsors { get; set; } = new();  // New list field
}
```

### Parsing Strategy

Use CsvHelper's flexible mapping with optional columns:

```csharp
public class VoteMap : ClassMap<Vote>
{
    public VoteMap()
    {
        Map(m => m.VotedOnDate).Name("bill_voted_on_date").Optional();
        Map(m => m.CoSponsors).Convert(row => 
            row.Row.GetField("co_sponsors")?.Split(',').Select(int.Parse).ToList()
        ).Optional();
    }
}
```

### API Enhancement

Add versioned endpoints or extend existing ones:

```csharp
app.MapGet("/api/v2/summary/bills", (QueryService service) => 
    service.GetBillSummariesWithDetails()
);
```

### Database Migration Path

For production scalability, I would migrate from in-memory storage to a proper database:
- **PostgreSQL** with Entity Framework Core for relational queries
- Implement migrations using `dotnet ef migrations add`
- Keep the service layer abstraction - only change the repository implementation
- This allows zero changes to business logic and API contracts

### Frontend Adaptations

The Chart.js dashboard is already modular - new columns would require:
- Additional API calls in `app.js`
- New chart configurations (timeline charts for dates, network graphs for co-sponsors)
- Minimal CSS adjustments for responsive layouts

---

## 3. Generating CSV Exports from Application Data

If the requirement reversed (generating CSVs from internal data), I would implement:

### New Service Layer

```csharp
public class CsvExportService
{
    public async Task<byte[]> ExportLegislatorsToCsvAsync(IEnumerable<Legislator> legislators)
    {
        using var memoryStream = new MemoryStream();
        using var writer = new StreamWriter(memoryStream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        
        await csv.WriteRecordsAsync(legislators);
        await writer.FlushAsync();
        return memoryStream.ToArray();
    }
}
```

### New API Endpoints

```csharp
app.MapGet("/api/export/legislators", async (CsvExportService service) =>
{
    var data = await service.ExportLegislatorsToCsvAsync(legislators);
    return Results.File(data, "text/csv", "legislators_export.csv");
});
```

### Dynamic Query Builder

For selective exports with filters:

```csharp
app.MapGet("/api/export/bills", async (
    [FromQuery] int? sponsorId,
    [FromQuery] DateTime? fromDate,
    CsvExportService service) =>
{
    var filtered = await service.GetFilteredBillsAsync(sponsorId, fromDate);
    return await service.ExportToCsvAsync(filtered);
});
```

### Frontend Integration

Add download buttons to the dashboard:

```javascript
async function downloadCsv(endpoint) {
    const response = await fetch(endpoint);
    const blob = await response.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'export.csv';
    a.click();
}
```

---

## 4. Time Spent on Assignment

**Total Time: Approximately 5-6 hours**

### Breakdown

- **Hour 1**: Project setup, CSV analysis, architecture planning
- **Hour 2-3**: Core implementation (models, services, CSV parsing)
- **Hour 3-4**: API endpoints, frontend dashboard, Chart.js integration
- **Hour 4-5**: Comprehensive test suite (22 tests with edge cases)
- **Hour 5-6**: Docker configuration, documentation, code review & refactoring

### Time Distribution

- Backend development: ~40%
- Testing & validation: ~30%
- Frontend & visualization: ~20%
- DevOps & documentation: ~10%

The development followed an iterative approach: implementing features incrementally, writing tests immediately after each component, and refining based on test feedback. This test-driven approach ensured high code quality and caught several edge cases early (like handling legislators without votes and missing sponsor data).

---

## Final Notes

This solution balances **production readiness** with **development speed**. The clean architecture allows easy extension, the comprehensive test suite ensures reliability, and the Docker containerization provides consistent deployment across environments. The in-memory approach is perfect for the current dataset size (~20 legislators, 2 bills, 38 votes) but the service abstraction makes database migration straightforward when needed.

---