    using Shop.Common.Models;
	using Shop.Identity.Dtos;
    namespace Shop.Identity.Extensions;
    public static class UserDtoExtension
    {
        public static ApplicationUser AsApplicationUser(this UserDto user)
        {
            return new ApplicationUser
            {
                UserName = user.Username,
                Email = user.Email,
                Budget = user.Budget
            };
        }
    }