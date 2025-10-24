using CastleLordTycoon.Server.Configuration;
using CastleLordTycoon.Server.Metadata;
using CastleLordTycoon.Server.Services.Encounters;
using CastleLordTycoon.Server.Services.Metadata;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
if (environment == "Development")
{
    var root = Directory.GetCurrentDirectory();
    var dotenv = Path.Combine(root, "..", ".env.dev");
    if (File.Exists(dotenv))
    {
        DotNetEnv.Env.Load(dotenv);
        Console.WriteLine($"Loaded environment variables from: {dotenv}");
    }
    else
    {
        Console.WriteLine($".env.dev not found at: {dotenv}");
    }
}
else
{
    Console.WriteLine($"Running in {environment} mode - using container environment variables");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.Configure<MetadataOptions>(builder.Configuration.GetSection("Metadata"));
builder.Services.AddSingleton<IMetadataFileResolver, MetadataFileResolver>();
builder.Services.AddSingleton<IGameMetadataProvider, GameMetadataProvider>();
builder.Services.AddSingleton<IEncounterConfigService, EncounterConfigService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var metadataProvider = scope.ServiceProvider.GetRequiredService<IGameMetadataProvider>();
    metadataProvider.GetSnapshot();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/metadata/encounters", (IEncounterConfigService service) =>
    Results.Ok(service.GetAll().Select(config => new
    {
        config.FieldId,
        config.FieldNameKo,
        config.FieldTier,
        config.EncounterType,
        Enemy = new
        {
            config.Enemy.EnemyId,
            Name = config.Enemy.EnemyNameKo,
            config.Enemy.Attribute,
            config.Enemy.Type,
            config.Enemy.BaseLevel
        },
        StatModifier = new
        {
            config.StatModifier.ModifierId,
            Name = config.StatModifier.ModifierNameKo,
            config.StatModifier.AttackScalePct,
            config.StatModifier.DefenseScalePct,
            config.StatModifier.HpScalePct
        },
        DropTable = config.DropTableEntries.Select(entry => new
        {
            entry.EntryId,
            entry.RewardType,
            entry.RewardId,
            entry.RewardNameKo,
            entry.DropRateBasisPoints,
            entry.AmountMin,
            entry.AmountMax
        }),
        SpawnRateBasisPoints = config.RawEncounter.SpawnRateBasisPoints,
        DropTableId = config.RawEncounter.DropTableId,
        Notes = config.RawEncounter.NotesKo
    })))
    .WithName("GetEncounterMetadata");

app.Run();
