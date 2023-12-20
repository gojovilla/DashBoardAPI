
using DashBoardAPI.Entity;
using Microsoft.Extensions.Configuration;
using DashBoardAPI.Repository;
using DashBoardAPI.Service.DashBoardService;
using Microsoft.EntityFrameworkCore;
using System.Net;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

 
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<DataContext>(options=>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings")); 
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IDashBoardService, DashBoardService>();
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.AddCors(option => option.AddPolicy(name: "AllowAngularOrigins",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
    }));

var app = builder.Build();
app.UseRouting();
//app.UseEndpoints(endpoints =>
//{
//    //Commented on CR-FEB-2022
//    //endpoints.MapControllerRoute("default", "{controller=Dashboard}/{action=Index}");
//    //changed on CR-FEB-2022
//    endpoints.MapControllerRoute("default", "{controller=DashBoard}/{action=GetDashBoardData}");
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()|| app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAngularOrigins");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=DashBoard}/{action=GetDashBoardData}");
});

app.Run();
