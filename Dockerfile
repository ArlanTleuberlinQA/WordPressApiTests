FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet build --configuration Debug
CMD ["dotnet", "test", "--no-build", "--configuration", "Debug"]