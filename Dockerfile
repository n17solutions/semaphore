FROM microsoft/dotnet:sdk AS build

WORKDIR /code

COPY . .

RUN dotnet restore
RUN dotnet test --no-restore
RUN dotnet publish ./API/API.csproj --output /output --configuration Release

FROM microsoft/dotnet:aspnetcore-runtime

COPY --from=build /output /app

WORKDIR /app

ENTRYPOINT ["dotnet", "N17Solutions.Semaphore.API.dll"]