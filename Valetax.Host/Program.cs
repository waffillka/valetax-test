using Valetax.Db.Dependencies;
using Valetax.Host.Infrastructure.Middlewares;
using Valetax.Services.Dependencies;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.RegisterRepositories();
builder.Services.RegisterServices();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExceptionMiddleware>();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();