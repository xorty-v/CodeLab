# syntax=docker/dockerfile:1.7
ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

RUN dotnet tool install --global dotnet-ef --version 9.0.10
ENV PATH="/root/.dotnet/tools:${PATH}"

COPY [".editorconfig", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

COPY ["CodeLab.Contracts/", "CodeLab.Contracts/"]
COPY ["CodeLab.Core/", "CodeLab.Core/"]
COPY ["CodeLab.Domain/", "CodeLab.Domain/"]
COPY ["CodeLab.Infrastructure.Postgres/", "CodeLab.Infrastructure.Postgres/"]
COPY ["CodeLab.Web/", "CodeLab.Web/"]

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet restore "CodeLab.Web/CodeLab.Web.csproj" --disable-parallel

WORKDIR "/src/CodeLab.Web"

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet ef migrations bundle \
    --project "../CodeLab.Infrastructure.Postgres/CodeLab.Infrastructure.Postgres.csproj" \
    --startup-project "CodeLab.Web.csproj" \
    --output /app/efbundle \
    --configuration Release \
    --verbose

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet publish "CodeLab.Web.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /app/efbundle .
RUN chmod +x efbundle

EXPOSE 8001

ENTRYPOINT ["dotnet", "CodeLab.Web.dll"]