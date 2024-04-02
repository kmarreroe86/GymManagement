#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
COPY ["src/GymManagement.Api/GymManagement.Api.csproj", "src/GymManagement.Api/"]
COPY ["src/GymManagement.Application/GymManagement.Application.csproj", "src/GymManagement.Application/"]
COPY ["src/GymManagement.Domain/GymManagement.Domain.csproj", "src/GymManagement.Domain/"]
COPY ["src/GymManagement.Contracts/GymManagement.Contracts.csproj", "src/GymManagement.Contracts/"]
COPY ["src/GymManagement.Infrastructure/GymManagement.Infrastructure.csproj", "src/GymManagement.Infrastructure/"]

RUN dotnet restore "./src/GymManagement.Api/GymManagement.Api.csproj"
COPY ./src ./src
WORKDIR "/src/GymManagement.Api"
RUN dotnet publish "GymManagement.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GymManagement.Api.dll"]
