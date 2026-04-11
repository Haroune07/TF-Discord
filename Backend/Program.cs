
using Backend.Settings;
using Backend.Src.Models;
using Backend.Src.Repository;
using Backend.Src.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                MongoDBSettings settings = serviceProvider.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<IRepository<User>, MongoRepository<User>>();
            builder.Services.AddScoped<ChannelService>();
            builder.Services.AddScoped<IRepository<Channel>, MongoRepository<Channel>>();
            builder.Services.AddScoped<MessageService>();
            builder.Services.AddScoped<IRepository<Message>, MongoRepository<Message>>();
            builder.WebHost.UseUrls(Shared.Constants.Ports.SERVER_LISTEN_URL);
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
