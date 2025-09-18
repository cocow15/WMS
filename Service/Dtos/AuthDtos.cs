using System.ComponentModel.DataAnnotations;

namespace ApplicationTest.Dtos;

public class RegisterRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(200), EmailAddress]
    public string Email { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Username { get; set; } = default!;

    [Required, MinLength(6)]
    public string Password { get; set; } = default!;

    [MaxLength(50)]
    public string? Role { get; set; } = "User";
}

public class LoginRequest 
{
    [Required, MaxLength(200)]
    public string Username { get; set; } = default!;

    [Required, MinLength(6)]
    public string Password { get; set; } = default!;
}
