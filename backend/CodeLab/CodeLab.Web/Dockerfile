FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CodeLab.Web/CodeLab.Web.csproj", "CodeLab.Web/"]
COPY ["CodeLab.CodeRunner/CodeLab.CodeRunner.csproj", "CodeLab.CodeRunner/"]
COPY ["CodeLab.Domain/CodeLab.Domain.csproj", "CodeLab.Domain/"]
COPY ["CodeLab.Core/CodeLab.Core.csproj", "CodeLab.Core/"]
COPY ["CodeLab.Infrastructure.Postgres/CodeLab.Infrastructure.Postgres.csproj", "CodeLab.Infrastructure.Postgres/"]
RUN dotnet restore "CodeLab.Web/CodeLab.Web.csproj"
COPY . .
WORKDIR "/src/CodeLab.Web"
RUN dotnet build "./CodeLab.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CodeLab.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeLab.Web.dll"]
