using HotelManagementInfratructure.Injection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInfrastructureClass(builder.Configuration);

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy
//            .AllowAnyOrigin()   // Allow all origins
//            .AllowAnyMethod()   // Allow all HTTP methods
//            .AllowAnyHeader();  // Allow all headers
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCredentials", builder =>
    {
        builder.WithOrigins("http://localhost:5174", "https://localhost:5174")
               .AllowCredentials() // This is crucial!
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseCors("AllowCredentials");

// Enable static file serving for uploaded images
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
