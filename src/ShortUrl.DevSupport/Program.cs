using Microsoft.Extensions.Hosting;
using ShortUrl.DbPersistence;

var builder = Host.CreateApplicationBuilder();
builder.AddServiceDefaults();

builder.AddShortUrlDbPersistenceIntegration();

builder.Build();

Console.WriteLine("Bye Bye");