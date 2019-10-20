#!/bin/sh

(cd src/WindupButton.GraphQL ; dotnet pack --configuration Release)
(cd src/WindupButton.GraphQL.InMemory ; dotnet pack --configuration Release)
(cd src/WindupButton.GraphQL.Roscoe.Postgres ; dotnet pack --configuration Release)
(cd src/WindupButton.GraphQL.Roscoe.SqlServer ; dotnet pack --configuration Release)
