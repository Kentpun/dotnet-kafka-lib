using kafka_configuration_lib;
using kafka_configuration_lib.Configurations;
using kafka_configuration_lib.Examples;
using kafka_configuration_lib.Helpers;
using test_kafka_producer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var bootstrapServers = builder.Configuration.GetValue<string>("Kafka:BootstrapServers");
var clientId = builder.Configuration.GetValue<string>("Kafka:ClientId");
KafkaOptions kafkaOptions = new KafkaOptions
{
    BootstrapServers = bootstrapServers,
    ClientId = clientId,
    Debug = "generic" // or "generic,broker,security"
};

builder.Services.UseKafkaProducer();
builder.Services.AddSingleton(kafkaOptions);
builder.Services.AddSingleton<TestProducer>();

builder.Services.AddControllers();
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

TestClass testClass = new TestClass();
testClass.id = "123456";
testClass.message = "test-123456";
var testProducer = app.Services.GetRequiredService<TestProducer>();
testProducer.TestPublish(testClass);

app.UseAuthorization();

app.MapControllers();

app.Run();

