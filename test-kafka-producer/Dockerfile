#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["test-kafka-producer/test-kafka-producer.csproj", "test-kafka-producer/"]
COPY ["kafka-configuration-lib/kafka-configuration-lib.csproj", "kafka-configuration-lib/"]
RUN dotnet restore "test-kafka-producer/test-kafka-producer.csproj"
COPY . .
WORKDIR "/src/test-kafka-producer"
RUN dotnet build "test-kafka-producer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "test-kafka-producer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test-kafka-producer.dll"]
