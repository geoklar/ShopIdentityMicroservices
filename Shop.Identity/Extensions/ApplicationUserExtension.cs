	using Shop.Common.Models;
	using Shop.Identity.Dtos;
	namespace Shop.Identity.Extensions;
	public static class ApplicationUserExtension
	{
		public static UserDto AsUserDto(this ApplicationUser user)
		{
			return new UserDto
			(
				user.Id,
				user?.UserName,
				user?.Email,
				user.Budget
			);
		}
	}
