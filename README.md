# Welcome to QuorumSite
# Mid Level Fullstack Software Engineer Technical Challenge repository 

A legislative data analysis platform built with ASP.NET Core 8 Minimal API and an interactive dashboard using Chart.js.

## Overview

QuorumSite processes legislative data from CSV files to generate comprehensive statistics about bills and legislators, presenting insights through an interactive visual dashboard. The application demonstrates clean architecture, efficient data processing, and modern web development practices.

## Features

- **📊 Interactive Dashboard**: Real-time data visualization with Chart.js
- **🔍 Legislator Analytics**: Track voting patterns for all 20 legislators
- **📜 Bill Tracking**: Monitor support/opposition for legislative bills
- **🚀 RESTful API**: Clean Minimal API endpoints for data access
- **🐳 Docker Support**: Fully containerized with multi-stage builds
- **✅ Comprehensive Tests**: 22 unit tests with HTML reporting

## Project Structure

```
QuorumSite/
├── Models/              # Data models (Bill, Legislator, Vote, VoteResult)
├── Services/            # Business logic layer
│   ├── CsvParsingService.cs      # CSV parsing with CsvHelper
│   ├── DataRepository.cs          # In-memory data storage
│   └── QueryService.cs            # LINQ-based query logic
├── Data/                # CSV data files
│   ├── bills.csv
│   ├── legislators.csv
│   ├── votes.csv
│   └── vote_results.csv
├── wwwroot/             # Static frontend
│   ├── index.html
│   └── app.js
├── Dockerfile
├── Dockerfile.test
├── QuorumSite.Tests/    # Unit tests (xUnit + FluentAssertions)
└── Program.cs           # Application entry point
```

## Quick Start

### Option 1: Docker (Recommended)

```bash
# Build and run
docker build -t quorumsite .
docker run -p 8080:8080 quorumsite
```

Access the dashboard at: **http://localhost:8080**

### Option 2: .NET SDK

```bash
# Restore dependencies and run
dotnet restore
dotnet run
```

Access at: **http://localhost:5000** (or as indicated in terminal)

## API Endpoints

### \`GET /api/summary/legislators\`
Returns voting statistics for each legislator.

**Response:**
```json
[
  {
    "id": 904789,
    "legislator": "Rep. Don Bacon (R-NE-2)",
    "supportedBills": 1,
    "opposedBills": 1
  }
]
```

### \`GET /api/summary/bills\`
Returns statistics for each bill including supporters, opposers, and primary sponsor.

**Response:**
```json
[
  {
    "id": 2900994,
    "bill": "H.R. 3684: Infrastructure Investment and Jobs Act",
    "supporters": 13,
    "opposers": 6,
    "primarySponsor": "Unknown"
  }
]
```

## Data Insights

Based on the provided CSV data:

### Bills
- **H.R. 3684 (Infrastructure)**: 13 supporters, 6 opposers
- **H.R. 5376 (Build Back Better)**: 6 supporters, 13 opposers

### Coverage
- 20 legislators tracked
- 2 bills analyzed
- 38 vote records processed

## Testing

### Run Tests Locally

```bash
# Execute all tests
dotnet test

# With detailed output
dotnet test --verbosity normal
```

### Docker Testing with HTML Reports

```bash
# Build test image
docker build -f Dockerfile.test -t quorumsite-tests .

# Create TestResults directory (required to avoid permission issues)
mkdir -p TestResults

# Run tests (choose your platform)
# Linux/macOS:
docker run --rm -v \$(pwd)/TestResults:/src/TestResults quorumsite-tests

# Windows PowerShell:
docker run --rm -v \${PWD}/TestResults:/src/TestResults quorumsite-tests

# Windows CMD:
docker run --rm -v %cd%/TestResults:/src/TestResults quorumsite-tests

# Open HTML report (choose your platform)
# Linux:
xdg-open TestResults/*.html

# macOS:
open TestResults/*.html

# Windows PowerShell:
Start-Process (Get-ChildItem TestResults\*.html).FullName

# Windows CMD:
start TestResults\*.html
```

### Test Coverage

- **CsvParsingServiceTests** (8 tests): Validates CSV parsing accuracy
- **QueryServiceTests** (14 tests): Validates business logic and calculations
- **Total**: 22 tests, 100% pass rate

Key validations:
- ✅ All 20 legislators loaded correctly
- ✅ Both bills processed with accurate vote counts
- ✅ Vote aggregation logic verified
- ✅ Edge cases handled (missing sponsors, legislators without votes)
- ✅ No negative vote counts
- ✅ Total votes match CSV data (38 records)

## Technology Stack

### Backend
- **.NET 8**: Modern, high-performance framework
- **ASP.NET Core Minimal API**: Lightweight REST API
- **CsvHelper**: Robust CSV parsing library
- **Dependency Injection**: Built-in IoC container

### Frontend
- **HTML5/CSS3**: Modern, responsive design
- **JavaScript (ES6+)**: Async/await, Fetch API
- **Chart.js**: Interactive data visualizations

### Testing
- **xUnit**: Industry-standard test framework
- **FluentAssertions**: Readable, expressive assertions
- **Moq**: Mocking framework for dependencies

### DevOps
- **Docker**: Multi-stage builds for optimization
- **HTML Test Reports**: Native .NET HTML logger

## Architecture Highlights

- **Clean Architecture**: Separation of concerns (Models, Services, API)
- **In-Memory Data**: No database required - fast and simple
- **LINQ Queries**: Elegant, efficient data processing
- **Singleton Services**: Data loaded once at startup
- **RESTful Design**: Standard HTTP methods and status codes
- **Static File Serving**: Integrated frontend hosting

## Business Logic

### Legislator Statistics
For each legislator, counts:
- **Supported Bills**: Vote_Type = 1 (Yea)
- **Opposed Bills**: Vote_Type = 2 (Nay)

### Bill Statistics
For each bill, calculates:
- **Supporters**: Count of Vote_Type = 1
- **Opposers**: Count of Vote_Type = 2
- **Primary Sponsor**: Legislator who sponsored the bill (or "Unknown")

## Development Notes

- **No External Database**: All data loaded from CSV files into memory
- **Error Handling**: Graceful handling of missing data and edge cases
- **Cross-Platform**: Runs on Linux, macOS, and Windows
- **Production-Ready**: Docker containerization for easy deployment

## Next Steps

<table>
<tr>
<td width="50%" valign="top">

### 🏗️ Architecture & Backend
- [ ] Refactor the API to use the MSC architecture
- [ ] Develop new application routes
- [ ] Build the database for the API
- [ ] Consume data dynamically
- [ ] Make the analysis CSV available for download

</td>
<td width="50%" valign="top">

### 🎨 Frontend & UX
- [ ] Build a frontend in React
- [ ] Make the application responsive
- [ ] Develop the dark theme
- [ ] Develop the Not Found page

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 🧪 Testing
- [ ] Develop frontend tests
- [ ] Develop integration tests

</td>
<td width="50%" valign="top">

### 🚀 Deployment
- [ ] Deploy the application

</td>
</tr>
</table>

---

**Built with 💜 using .NET 8 and modern web technologies**
