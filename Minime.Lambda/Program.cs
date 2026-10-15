using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Minime.Lambda.Endpoints;
using Minime.Lambda.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);
builder.Services.AddOpenApi();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IDynamoDBContext>(sp => new DynamoDBContextBuilder()
    .WithDynamoDBClient(sp.GetRequiredService<IAmazonDynamoDB>)
    .Build());
builder.Services.AddSingleton<IUrlRepository, DynamoDbUrlRepository>();

builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();

// A origem exata vem da configuração em vez de AllowAnyOrigin: com qualquer
// site podendo chamar a API, o reCAPTCHA vira a única defesa do encurtador.
var origens = builder.Configuration["OrigensPermitidas"]?.Split(',', StringSplitOptions.RemoveEmptyEntries)
              ?? ["https://minime.cloud", "http://localhost:4321", "http://localhost:4322"];

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(origens)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();

app.MapUrlEndpoints();

app.Run();
