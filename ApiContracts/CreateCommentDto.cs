namespace ApiContracts;

public class CreateCommentDto
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(500, MinimumLength = 1)]
    public string Body { get; set; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    public int UserId { get; set; }

    [System.ComponentModel.DataAnnotations.Required]
    public int PostId { get; set; }
}