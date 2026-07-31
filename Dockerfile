FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["RoomRental.API/RoomRental.API.csproj", "RoomRental.API/"]
COPY ["RoomRental.Domain/RoomRental.Domain.csproj", "RoomRental.Domain/"]
COPY ["RoomRental.Infrastructure/RoomRental.Infrastructure.csproj", "RoomRental.Infrastructure/"]

RUN dotnet restore "RoomRental.API/RoomRental.API.csproj"

COPY . .

WORKDIR "/src/RoomRental.API"
RUN dotnet build "RoomRental.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomRental.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "RoomRental.API.dll"]