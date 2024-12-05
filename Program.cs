using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular app's URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAngularApp");

const string filePath = "passwordStore.json";

// Load data from the file if it exists
var passwordStore = File.Exists(filePath)
    ? JsonSerializer.Deserialize<List<PasswordItem>>(File.ReadAllText(filePath))
    : new List<PasswordItem>();

// Save data to the file
void SaveToFile() => File.WriteAllText(filePath, JsonSerializer.Serialize(passwordStore));

app.MapPost("/api/passwords", (PasswordItem item) =>
{
    item.Id = passwordStore.Count > 0 ? passwordStore[^1].Id + 1 : 1; // Assign new ID
    item.password = Convert.ToBase64String(Encoding.ASCII.GetBytes(item.password)); // Encrypt the password
    passwordStore.Add(item);
    SaveToFile();
    return Results.Ok(item);
});

app.MapGet("/api/passwords/{username}", (string username) =>
{
    var userPasswords = passwordStore.FindAll(p => p.UserName.Equals(username, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(userPasswords);
});

app.MapGet("/api/passwords/{id:int}", (int id) =>
{
    var item = passwordStore.Find(p => p.Id == id);
    return item != null ? Results.Ok(item) : Results.NotFound();
});

app.MapGet("/api/passwords", () =>
{
    return Results.Ok(passwordStore);
});

app.MapGet("/api/passwords/{id:int}/decrypted", (int id) =>
{
    var item = passwordStore.Find(p => p.Id == id);
    if (item == null) return Results.NotFound();

    var decryptedPassword = Encoding.ASCII.GetString(Convert.FromBase64String(item.password));
    return Results.Ok(new
    {
        item.Id,
        item.Category,
        item.App,
        item.UserName,
        DecryptedPassword = decryptedPassword
    });
});

app.MapPut("/api/passwords/{id:int}", (int id, PasswordItem updatedItem) =>
{
    var item = passwordStore.Find(p => p.Id == id);
    if (item == null) return Results.NotFound();

    item.Category = updatedItem.Category;
    item.App = updatedItem.App;
    item.UserName = updatedItem.UserName;
    item.password = Convert.ToBase64String(Encoding.ASCII.GetBytes(updatedItem.password));

    SaveToFile();
    return Results.Ok(item);
});

app.MapDelete("/api/passwords/{id:int}", (int id) =>
{
    var item = passwordStore.Find(p => p.Id == id);
    if (item == null) return Results.NotFound();

    passwordStore.Remove(item);
    SaveToFile();
    return Results.Ok($"Password with ID {id} deleted.");
});

app.Run();

record PasswordItem
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string App { get; set; }
    public string UserName { get; set; }
    public string password { get; set; }
}
