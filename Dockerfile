FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build

WORKDIR /src

# Copy project files and restore dependencies
COPY src/rpsls.Api/*.csproj ./rpsls.Api/
COPY src/rpsls.Application/*.csproj ./rpsls.Application/
COPY src/rpsls.Domain/*.csproj ./rpsls.Domain/
COPY src/rpsls.Infrastructure/*.csproj ./rpsls.Infrastructure/
RUN dotnet restore ./rpsls.Api/rpsls.Api.csproj

# Copy source code and publish app
COPY ./src ./
WORKDIR /src/rpsls.Api
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS runtime

WORKDIR /app

COPY --from=build /out ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "rpsls.Api.dll"]