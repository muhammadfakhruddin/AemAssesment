using AemAssesment;
using AemAssesment.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DBContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHttpClient<PlatformWellApiClient>(c =>
    c.BaseAddress = new Uri(builder.Configuration["ExternalApi:BaseUrl"]!));

var app = builder.Build();

// Create/update the database from the migrations on startup.
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<DBContext>().Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
