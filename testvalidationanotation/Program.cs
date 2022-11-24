using Newtonsoft;
using Newtonsoft.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
        foreach (var pair in context.ModelState)
        {
            if (pair.Value.Errors.Count > 0)
            {
                errors[pair.Key.Replace("]", "").Replace("[", ".")] = pair.Value.Errors.Select(error => error.ErrorMessage).ToList();
            }
        }

        var result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "One or more validation errors occurred.",
            status = 400,
            traceId = context.HttpContext.TraceIdentifier,
            errors = errors
        });

        return result;
    };
})
;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
