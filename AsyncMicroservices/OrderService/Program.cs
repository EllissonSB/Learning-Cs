using MassTransit;
using Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.MapPost("/orders", async (IPublishEndpoint publishEndpoint, CreateOrderDto orderDto) =>
{
    await publishEndpoint.Publish<IOrderCreated>(new
    {
        OrderId = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        orderDto.CustomerName,
        orderDto.TotalAmount
    });

    return Results.Accepted("", new { Status = "Order Submitted", orderDto.CustomerName });
})
.WithName("CreateOrder");

app.Run();

public record CreateOrderDto(string CustomerName, decimal TotalAmount);
