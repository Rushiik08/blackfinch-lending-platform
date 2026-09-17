using Blackfinch.Lending.Api.Data;
using Blackfinch.Lending.Api.Services;
using Blackfinch.Lending.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<LoanDecisionService>();
builder.Services.AddScoped<LoanApplicationService>();
builder.Services.AddDbContext<LendingDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Lending")
        ?? "Data Source=lending.db";
    options.UseSqlite(connectionString);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LendingDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.MapControllers();
app.Run();
