using RSSFeedReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<SubscriptionStore>();

const string FrontendCorsPolicy = "FrontendCorsPolicy";
var frontendOrigins = new[] { "http://localhost:5193", "https://localhost:7188" };
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

app.MapGet("/api/subscriptions", (SubscriptionStore store) => Results.Ok(store.GetAll()))
    .WithName("GetSubscriptions");

app.MapPost("/api/subscriptions", (SubscriptionRequest request, SubscriptionStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Url))
    {
        return Results.BadRequest();
    }

    var subscription = store.Add(request.Url);
    return Results.Created($"/api/subscriptions", subscription);
})
    .WithName("AddSubscription");

app.Run();

record SubscriptionRequest(string Url);

