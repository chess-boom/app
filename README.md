# App
[![.NET Build & Test](https://github.com/chess-boom/app/actions/workflows/dotnet.yml/badge.svg)](https://github.com/chess-boom/app/actions/workflows/dotnet.yml)
[![Lint Code Base](https://github.com/chess-boom/app/actions/workflows/super-linter.yml/badge.svg)](https://github.com/chess-boom/app/actions/workflows/super-linter.yml)

The Application for our Chess Boom project. The application is where the downloaded games from the extension are imported into and is what provides the analysis (courtesy of [Stockfish](https://stockfishchess.org/) and [Fairy-Stockfish](https://fairy-stockfish.github.io/)).

The Application supports Standard chess, along with several variants (Atomic, Horde, Chess960).

To use the User Profiles to see overall results of your downloaded chess games played on lichess.org, it is required that you put the .pgn files in the CBoom directory in the applcation directory, and `is recommended you have a lichess account to get data for your wins and losses` for this feature. A lichess account is not required for live analysis, just overall summaries with the user profile feature.

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
