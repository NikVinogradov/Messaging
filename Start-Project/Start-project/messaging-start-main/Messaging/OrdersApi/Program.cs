using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Domain;
using Orders.Service;
using OrdersApi.Consumers;
using OrdersApi.Infrastructure.Mappings;
using OrdersApi.Service.Clients;
using OrdersApi.Services;
using System.Reflection;

namespace OrdersApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddAutoMapper(typeof(OrderProfileMapping));
            builder.Services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

            });

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>();

            builder.Services.AddMassTransit(x =>
            {
                //Formatters
                //x.SetKebabCaseEndpointNameFormatter();
                //x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("hellos", true));
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("hellos", false));

                //Several ways to register consumers
                //x.AddConsumer<OrderCreatedConsumer, OrderCreatedConsumerDefinition>();

                //var entryAssembly = Assembly.GetEntryAssembly();
                //x.AddConsumers(entryAssembly);

                x.AddConsumer(typeof(OrderCreatedConsumer));

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    //comment because i want to use OrderCreatedConsumerDefinition
                    cfg.ReceiveEndpoint("order-created", e =>
                    {
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
