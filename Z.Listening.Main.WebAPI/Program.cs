using Z.CommonInitializer;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "e:/temp/Listening.Main.log",
    EventBusQueueName = "Z.Listening.Main"
});
// Add services to the container.
//builder.Services.AddScoped<IListeningRepository, ListeningRepository>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Z.Listening.Main.WebAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listening.Main.WebAPI v1"));
}

app.UseZDefault();
app.MapControllers();
app.Run();
