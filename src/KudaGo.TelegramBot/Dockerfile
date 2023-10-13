#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/KudaGo.TelegramBot/KudaGo.TelegramBot.csproj", "src/KudaGo.TelegramBot/"]
COPY ["src/KudaGo.Application/KudaGo.Application.csproj", "src/KudaGo.Application/"]
RUN dotnet restore "src/KudaGo.TelegramBot/KudaGo.TelegramBot.csproj"
COPY . .
WORKDIR "/src/src/KudaGo.TelegramBot"
RUN dotnet build "KudaGo.TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KudaGo.TelegramBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

ARG MONGO_USERNAME
ARG MONGO_PASSWORD
ARG MONGO_HOST
ARG MONGO_PORT
ARG BOT_TOKEN

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KudaGo.TelegramBot.dll"]

RUN sed -i "s|${MONGO_USERNAME}|$MONGO_USERNAME|g" appsettings.json
RUN sed -i "s|${MONGO_PASSWORD}|$MONGO_PASSWORD|g" appsettings.json
RUN sed -i "s|${MONGO_HOST}|$MONGO_HOST|g" appsettings.json
RUN sed -i "s|${MONGO_PORT}|$MONGO_PORT|g" appsettings.json
RUN sed -i "s|${BOT_TOKEN}|$BOT_TOKEN|g" appsettings.json
