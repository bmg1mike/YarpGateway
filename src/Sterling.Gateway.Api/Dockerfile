FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Sterling.Gateway.Api/Sterling.Gateway.Api.csproj", "src/Sterling.Gateway.Api/"]
RUN dotnet restore "src/Sterling.Gateway.Api/Sterling.Gateway.Api.csproj"
COPY . .
WORKDIR "/src/src/Sterling.Gateway.Api"
RUN dotnet build "Sterling.Gateway.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Sterling.Gateway.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sterling.Gateway.Api.dll"]
