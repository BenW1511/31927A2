using CampusHaps.Api.Data;
using Microsoft.EntityFrameworkCore;
using CampusHaps.Api.Interfaces;
using CampusHaps.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Let the Svelte dev server call the API
builder.Services.AddCors(o => o.AddPolicy("Client", p =>
    p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

//create the database if it doesn't exist
using (var scope = app.Services.CreateScope()) //creates a temporary scope for accessing services
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); //retrieves the database context
    db.Database.EnsureCreated(); //creates the database and its tables if they dontexist
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
//<>ben changed this

app.UseCors("Client");

app.UseAuthorization();

app.MapControllers();

app.Run();
