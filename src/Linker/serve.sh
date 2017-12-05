#!/bin/sh

workingDir=$(dirname "$0")
configuration="Debug"

dotnet $workingDir/bin/$configuration/netcoreapp1.0/Linker.dll
