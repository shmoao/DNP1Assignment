namespace ApiContracts;

public class CreateUserDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(30, MinimumLength = 2)]
    public string UserName { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(50, MinimumLength = 6)]
    public string Password { get; set; } = "";
}