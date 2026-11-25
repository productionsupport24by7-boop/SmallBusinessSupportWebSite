using System.ComponentModel.DataAnnotations;

namespace SupportOps.Application.DTOs.AI;

public class AIChatRequest
{
    [Required]
    [StringLength(4000, MinimumLength = 2)]
    public string Message { get; set; } = string.Empty;
}