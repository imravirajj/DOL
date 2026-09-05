FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/DOL.SharedKernel/DOL.SharedKernel.csproj", "src/DOL.SharedKernel/"]
COPY ["src/Services/Identity/DOL.Identity.Domain/DOL.Identity.Domain.csproj", "src/Services/Identity/DOL.Identity.Domain/"]
COPY ["src/Services/Identity/DOL.Identity.Application/DOL.Identity.Application.csproj", "src/Services/Identity/DOL.Identity.Application/"]
COPY ["src/Services/Identity/DOL.Identity.Infrastructure/DOL.Identity.Infrastructure.csproj", "src/Services/Identity/DOL.Identity.Infrastructure/"]
COPY ["src/Services/Identity/DOL.Identity.API/DOL.Identity.API.csproj", "src/Services/Identity/DOL.Identity.API/"]

RUN dotnet restore "src/Services/Identity/DOL.Identity.API/DOL.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/DOL.Identity.API"
RUN dotnet build "DOL.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DOL.Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DOL.Identity.API.dll"]
