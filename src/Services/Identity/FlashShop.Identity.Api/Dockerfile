FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BuildingBlocks/FlashShop.Shared/FlashShop.Shared.csproj", "src/BuildingBlocks/FlashShop.Shared/"]
COPY ["src/BuildingBlocks/FlashShop.MessageContracts/FlashShop.MessageContracts.csproj", "src/BuildingBlocks/FlashShop.MessageContracts/"]
COPY ["src/Services/Identity/FlashShop.Identity.Api/FlashShop.Identity.Api.csproj", "src/Services/Identity/FlashShop.Identity.Api/"]
RUN dotnet restore "src/Services/Identity/FlashShop.Identity.Api/FlashShop.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity/FlashShop.Identity.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlashShop.Identity.Api.dll"]
