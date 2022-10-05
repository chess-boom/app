# App
[![.NET Build & Test](https://github.com/chess-boom/app/actions/workflows/dotnet.yml/badge.svg)](https://github.com/chess-boom/app/actions/workflows/dotnet.yml)
## Design Infrastructure

See [Confluence Page](https://chessboom.atlassian.net/l/cp/hDGAeeMt)

## Build & Generate Metrics
To build the project and generate metrics use 
`dotnet build /t:Metrics`. A metrics XML file will be generated and written to `ChessBoom.Metrics.xml`

## Avalonia

### Run

```
cd src
dotnet run
```

Or, alternatively (from root dir):

```
dotnet run --project src
```
