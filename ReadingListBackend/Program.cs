using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReadingListBackend;
using ReadingListBackend.Database;
using ReadingListBackend.Interfaces;
using ReadingListBackend.Services;
using ReadingListBackend.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// automapper config
var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddMvc();

// configure DB connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// controller services
builder.Services.AddScoped<IListService, ListService>();

// create env
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// error handling
/*app.UseExceptionHandler("/error");
app.UseStatusCodePagesWithReExecute("/error/{0}");*/
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "error",
        pattern: "/error/{statusCode}",
        defaults: new { controller = "Error", action = "HandleError" }
    );
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();