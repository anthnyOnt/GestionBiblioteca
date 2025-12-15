#!/bin/bash

# Script to run ReqnRoll tests with code coverage
echo "Running ReqnRoll tests with code coverage..."

# Change to the ReqnRoll test project directory
cd ReqnrollTest

# Run tests with coverage collection
# --collect:"XPlat Code Coverage" enables coverlet collection
# --results-directory specifies where to save coverage results
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Find the latest coverage file
COVERAGE_FILE=$(find ./TestResults -name "coverage.cobertura.xml" | head -1)

if [ -z "$COVERAGE_FILE" ]; then
    echo "Error: No coverage file found"
    exit 1
fi

echo "Coverage file found: $COVERAGE_FILE"

# Generate HTML report
echo "Generating HTML coverage report..."
reportgenerator \
    -reports:"$COVERAGE_FILE" \
    -targetdir:"./CoverageReport" \
    -reporttypes:Html

echo "Coverage report generated in ./CoverageReport/index.html"

# Generate summary to console
echo "Coverage Summary:"
reportgenerator \
    -reports:"$COVERAGE_FILE" \
    -targetdir:"./CoverageReport" \
    -reporttypes:TextSummary

cat ./CoverageReport/Summary.txt 2>/dev/null || echo "Summary file not found, check the HTML report"

echo "Done! Open ./ReqnrollTest/CoverageReport/index.html to view the detailed coverage report."
