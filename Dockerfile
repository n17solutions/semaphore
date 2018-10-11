FROM microsoft/dotnet:sdk AS build

WORKDIR /code

COPY . .

RUN dotnet publish ./API/API.csproj --output /release --configuration Release

FROM microsoft/dotnet:aspnetcore-runtime

COPY --from=build /release /app

WORKDIR /app

ENTRYPOINT ["dotnet", "N17Solutions.Semaphore.API.dll"]
