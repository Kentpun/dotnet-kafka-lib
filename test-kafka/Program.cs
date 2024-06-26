using KP.Lib.Kafka;
using KP.Lib.Kafka.Configurations;
using KP.Lib.Kafka.Helpers;
using test_kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
var consumerGroupId = builder.Configuration.GetValue<string>("Kafka:ConsumerGroupId");
KafkaOptions kafkaOptions = new KafkaOptions
{
    BootstrapServers = bootstrapServers,
    ConsumerGroupId = consumerGroupId,
    Debug = "generic" // or "generic,broker,security"
};

builder.Services.AddLogging();
builder.Services.AddSingleton<TestConsumer>();
builder.Services.UseKafkaConsumer(kafkaOptions);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

