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

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS runtime
WORKDIR /app

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=build /out ./
# Copy source code for migrations
COPY ./src ./src

EXPOSE 8080
ENTRYPOINT ["dotnet", "rpsls.Api.dll"]