namespace ApiContracts;

public class CreatePostDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(1000, MinimumLength = 5)]
    public string Body { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    public int UserId { get; set; }
}