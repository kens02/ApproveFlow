#!/usr/bin/env bash
set -euo pipefail

dotnet restore ApproveFlow.sln
dotnet build ApproveFlow.sln -c Release
dotnet test ApproveFlow.sln -c Release
cd frontend
npm install
npm run lint
npm run test
npm run build
