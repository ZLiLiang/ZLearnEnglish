using Z.CommonInitializer;
using Z.JWT;
using Z.MediaEncoder.WebAPI.BgServices;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "e:/temp/MediaEncoder.log",
    EventBusQueueName = "MediaEncoder.WebAPI"
});
builder.Services.Configure<FileServerOptions>(builder.Configuration.GetSection("FileService:Endpoint"));
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddHttpClient();
builder.Services.AddHostedService<EncodingBgService>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MediaEncoder.WebAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediaEncoder.WebAPI v1"));
}
app.UseZDefault();
app.MapControllers();
app.Run();

