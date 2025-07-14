using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PCStoreApi.API.Endpoints;
using PCStoreApi.Application.Interfaces;
using PCStoreApi.Application.Services;
using PCStoreApi.Application.Validators;
using PCStoreApi.Infrastructure.Data;
using PCStoreApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPCBuildRepository, PCBuildRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPCBuildService, PCBuildService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDtoValidator>();

var app = builder.Build();
app.MapGet("/", () => "PC Store API is running");

app.MapUserEndpoints();
app.MapPCBuildEndpoints();

app.Run();
