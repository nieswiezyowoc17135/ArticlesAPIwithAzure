using ArticlesAPI.Data;
using ArticlesAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//baza danych
builder.Services.AddDbContext<ArticlesContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyArticlesConnectionString")));

//dodanie do scopa serwisów
builder.Services.AddScoped<IArticleService, ArticleService>();

var app = builder.Build();

//if (app.Enviroment.IsDevelopment())
//{ 
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
