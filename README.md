# App
[![.NET Build & Test](https://github.com/chess-boom/app/actions/workflows/dotnet.yml/badge.svg)](https://github.com/chess-boom/app/actions/workflows/dotnet.yml)
## Design Infrastructure

See [Confluence Page](https://chessboom.atlassian.net/l/cp/hDGAeeMt)

## Build
To build the project
`dotnet build`

## Test Coverage
GitHub Actions provides basic high level code coverage. For more detailed code coverage please use Visual Studio and [Run Coverlet Report](https://www.code4it.dev/blog/code-coverage-vs-2019-coverlet). Alternatively you can follow the following procedure to generate a coverage report:

1. Download report generator using `dotnet`: `dotnet tool install -g dotnet-reportgenerator-globaltool`
2. `cd src` or, even easier, `cd ChessBoom.NUnitTests` 
3. run `dotnet test --collect:"XPlat Code Coverage"`. A folder `TestResults` should be generated containing an XML file called `coverage.cobertura.xml`
4. Use the `reportgenerator` command to generate a report by passing in the coverage file path, and a path to save the coverage code. Here is the general format:
    ```
    reportgenerator
    -reports:"Path\To\TestProject\TestResults\{guid}\coverage.cobertura.xml"
    -targetdir:"coveragereport"
    -reporttypes:Html
    ```

    A specific example using all steps mentioned *aside from step 1*:
    ```
    cd ChessBoom.NUnitTests
    dotnet test --collect:"XPlat Code Coverage"

    reportgenerator -reports:"./TestResults/36ca65f7-f197-41a9-a08e-8ac94f5b9c1e/coverage.cobertura.xml" -targetdir:"./coverage -reporttypes:Html
    ```
5. open `index.html` from the produced folder in your browser to see coverage.



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
