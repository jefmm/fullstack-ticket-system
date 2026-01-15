using Backend.Data;
using Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS für dein Frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// EF Core SQLite
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlite("Data Source=tickets.db"));

// n8n config + http client + notifier
builder.Services.Configure<N8nOptions>(builder.Configuration.GetSection("N8n"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<TicketNotifier>();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DB automatisch anlegen (Demo-friendly)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicketDbContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.Run();
