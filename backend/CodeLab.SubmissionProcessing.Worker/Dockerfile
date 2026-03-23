# syntax=docker/dockerfile:1.7
ARG DOTNET_VERSION=9.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY [".editorconfig", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

COPY ["CodeLab.Contracts/", "CodeLab.Contracts/"]
COPY ["CodeLab.Core/", "CodeLab.Core/"]
COPY ["CodeLab.Domain/", "CodeLab.Domain/"]
COPY ["CodeLab.Infrastructure.Postgres/", "CodeLab.Infrastructure.Postgres/"]
COPY ["CodeLab.SubmissionProcessing.Worker/", "CodeLab.SubmissionProcessing.Worker/"]

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet restore "CodeLab.SubmissionProcessing.Worker/CodeLab.SubmissionProcessing.Worker.csproj"

WORKDIR "/src/CodeLab.SubmissionProcessing.Worker"

RUN --mount=type=cache,target=/root/.nuget/packages,sharing=locked \
    dotnet publish "CodeLab.SubmissionProcessing.Worker.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION} AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CodeLab.SubmissionProcessing.Worker.dll"]