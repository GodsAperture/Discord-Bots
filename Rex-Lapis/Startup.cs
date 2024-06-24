﻿global using Discord;
global using Discord.Interactions;
global using Discord.WebSocket;

global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices.Marshalling;
using DiscordNetTemplate.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using Serilog;
DiscordSocketClient client = new DiscordSocketClient(            
    new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                FormatUsersInBidirectionalUnicode = false,
                AlwaysDownloadUsers = true,
                LogGatewayIntentWarnings = false,
                LogLevel = LogSeverity.Info,
                UseInteractionSnowflakeDate = false
            });


    var builder = new HostBuilder();


    builder.ConfigureAppConfiguration(options
        => options.AddJsonFile("appsettings.json")
            .AddEnvironmentVariables());

    var loggerConfig = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File($"logs/log-{DateTime.Now:yy.MM.dd_HH.mm}.log")
        .CreateLogger();

    builder.ConfigureServices((host, services) =>
    {
        services.AddLogging(options => options.AddSerilog(loggerConfig, dispose: true));

        services.AddSingleton(client);

        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Info
        }));

        services.AddDbContextFactory<UserDB>(options => options.UseSqlite(Path.Combine("Users","UserInfo.db")));

        services.AddSingleton<InteractionHandler>();

        services.AddHostedService<DiscordBotService>();
    });

    var app = builder.Build();

    await app.RunAsync();