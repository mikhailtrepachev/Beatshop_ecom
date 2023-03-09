using Microsoft.AspNetCore.Identity;

namespace Beatshop.Models;

public class ApplicationUser : IdentityUser
{
    public string Nickname { get; set; } = string.Empty;
}

