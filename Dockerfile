FROM microsoft/dotnet:sdk

# SET ASP.NET Core environment variables
ENV ASPNETCORE_ENVIRONMENT="Production"

# Copy files to app directory
COPY /release /app

# Set working directory
WORKDIR /app

# Run
ENTRYPOINT ["dotnet", "N17Solutions.Semaphore.API.dll"]