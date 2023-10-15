#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG MONGO_USERNAME
ARG MONGO_PASSWORD
ARG MONGO_HOST
ARG MONGO_PORT
ARG BOT_TOKEN

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/KudaGo.TelegramBot/KudaGo.TelegramBot.csproj", "src/KudaGo.TelegramBot/"]
COPY ["src/KudaGo.Application/KudaGo.Application.csproj", "src/KudaGo.Application/"]
COPY src/KudaGo.TelegramBot/appsettings.json .
RUN dotnet restore "src/KudaGo.TelegramBot/KudaGo.TelegramBot.csproj"
COPY . .
WORKDIR "/src/src/KudaGo.TelegramBot"
RUN echo "ARG_NAME: $MONGO_PASSWORD"
RUN sed -i 's/${MONGO_USERNAME}/anton/g' appsettings.json
RUN sed -i 's/${MONGO_USERNAME}/$MONGO_PASSWORD/g' appsettings.json
RUN sed -i 's/${MONGO_USERNAME}/$MONGO_HOST/g' appsettings.json
RUN sed -i 's/${MONGO_USERNAME}/$MONGO_PORT/g' appsettings.json
RUN sed -i 's/${MONGO_USERNAME}/$BOT_TOKEN/g' appsettings.json
RUN dotnet build "KudaGo.TelegramBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KudaGo.TelegramBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KudaGo.TelegramBot.dll"]

