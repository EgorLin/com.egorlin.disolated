#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")"
dotnet build -c Release
cp bin/Release/netstandard2.0/com.egorlin.disolated.CodeGen.dll ../Analyzers/com.egorlin.disolated.CodeGen.dll
echo "Analyzer updated: Analyzers/com.egorlin.disolated.CodeGen.dll"
