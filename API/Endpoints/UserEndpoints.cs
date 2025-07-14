using FluentValidation;
using PCStoreApi.Application.DTOs.User;
using PCStoreApi.Application.Interfaces;

namespace PCStoreApi.API.Endpoints
{
    public static class UserEndpoints
    {
        public static RouteGroupBuilder MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/users");

            group.MapGet("/", async (IUserService service) =>
            {
                var user = await service.GetAllUsersAsync();
                return Results.Ok(user);
            });

            group.MapGet("/{id}", async (int id, IUserService service) =>
            {
                var user = await service.GetUserByIdAsync(id);
                return user == null ? Results.NotFound() : Results.Ok(user);
            });

            group.MapPost("/", async (UserCreateDto dto,
                IUserService service,
                IValidator<UserCreateDto> validator) =>
                {
                    var result = await validator.ValidateAsync(dto);
                    if (!result.IsValid)
                    {
                        var errors = result.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage
                            .ToArray()));

                    return Results.BadRequest(new { errors });
                }

                var created = await service.CreateUserAsync(dto);
                return Results.Created($"/users/{created!.UserId}", created);
            });

            group.MapPut("/{id}", async (int id, UserUpdateDto dto,
                IUserService service,
                IValidator<UserUpdateDto> validator) =>
               {
                    var result = await validator.ValidateAsync(dto);
                    if (!result.IsValid)
                    {
                        var errors = result.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage)
                            .ToArray());

                    return Results.BadRequest(new { errors });
                }

                var success = await service.UpdateUserAsync(id, dto);
                return success ? Results.NoContent() : Results.NotFound();
            });


            group.MapDelete("/{id}", async (int id, IUserService service) =>
            {
                var success = await service.DeleteUserAsync(id);
                return success ? Results.NoContent() : Results.NotFound();

            });

            return group;
        }
    }
}