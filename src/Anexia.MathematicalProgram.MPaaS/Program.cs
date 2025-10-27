using Anexia.MathematicalProgram.MPaaS;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = null);

builder.Services.Configure<MPaaSOptions>(builder.Configuration.GetSection("MPaaS"));

builder.Services.AddSingleton<JobsRepository>();
builder.Services.AddHostedService<JobRunner>();

builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin());
}

await app.RunAsync();