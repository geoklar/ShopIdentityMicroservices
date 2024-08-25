using System.ComponentModel.DataAnnotations;
namespace Shop.Identity.Dtos;
public record UserDto(
	Guid Id,
	string? Username,
	string? Email,
	decimal Budget
);
public record UpdateUserDto(
	[Required][EmailAddress] string Email,
	[Range(0, 1000000)] decimal Budget
);