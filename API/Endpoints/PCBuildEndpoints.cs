using FluentValidation;
using PCStoreApi.Application.DTOs.PCBuild;
using PCStoreApi.Application.Interfaces;
using PCStoreApi.Application.DTOs.User;
using PCStoreApi.Application.Services;

namespace PCStoreApi.API.Endpoints
{
    public static class PCBuildEndpoints
    {
        public static RouteGroupBuilder MapPCBuildEndpoints(this IEndpointRouteBuilder routes)
        {
            const string path = "/pcbuilds";
            var group = routes.MapGroup(path);

            group.MapGet("/", async (IPCBuildService services) =>
            {
                var builds = await services.GetAllBuildsAsync();
                return Results.Ok(builds);
            });

            group.MapGet("/{id}", async (int id, IPCBuildService services) =>
            {
                var build = await services.GetBuildByIdAsync(id);
                return build is null ? Results.NotFound() : Results.Ok(build);
            });

            group.MapGet("/user/{userId}", async (int userId, IPCBuildService pcBuildService, IUserService userService) =>
            {
                var user = await userService.GetUserByIdAsync(userId);
                if(user is null)
                {
                    return Results.NotFound($"No user found with ID {userId}");
                }

                var builds = await pcBuildService.GetBuildsByUserIdAsync(userId);
                return builds is null ? Results.NotFound($"No build found for this user ID {userId}") : Results.Ok(builds);
            });


            group.MapPost("/", async ( PCBuildCreateDto dto,
                IPCBuildService service,
                IValidator<PCBuildCreateDto> validator) =>
            {
                var result = await validator.ValidateAsync(dto);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                    return Results.BadRequest(new { errors });
                }

                var created = await service.CreateBuildAsync(dto);
                return Results.Created($"/pcbuilds/{created.PCBuildId}", created);
            });

            group.MapPut("/{id}", async (int id, PCBuildUpdateDto dto, IPCBuildService service, IValidator<PCBuildUpdateDto> validator) =>
            {
                var result = await validator.ValidateAsync(dto);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

                    return Results.BadRequest(new { errors });
                }

                var success = await service.UpdateBuildAsync(id, dto);
                return success ? Results.NoContent() : Results.NotFound();
            });

            group.MapDelete("/{id}", async (int id, IPCBuildService service) =>
            {
                var success = await service.DeleteBuildAsync(id);
                return success ? Results.NoContent() : Results.NotFound();
            });

            return group;
        }
    }
}
