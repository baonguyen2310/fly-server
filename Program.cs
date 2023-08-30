using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")!;
builder.Services.AddSingleton(connectionString);


var app = builder.Build();

// For production scenarios, consider keeping Swagger configurations behind the environment check
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();


using var conn = new SqlConnection(connectionString);

//Test connection
try
{
    conn.Open();
    Console.WriteLine("Connection successful!");
}
catch (Exception ex)
{
    Console.WriteLine("Connection failed: " + ex.Message);
}
finally
{
    conn.Close();
}

app.MapControllers();

app.Run();