#!/bin/sh

workingDir=$(dirname "$0")

dotnet run --project "$workingDir/src/Linker"
