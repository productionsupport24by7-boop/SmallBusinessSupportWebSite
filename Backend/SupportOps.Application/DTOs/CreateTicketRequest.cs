using System.ComponentModel.DataAnnotations;

namespace SupportOps.Application.DTOs.Tickets;

public class CreateTicketRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(5000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Priority is required.")]
    [RegularExpression(
        "^(Low|Medium|High|Critical)$",
        ErrorMessage = "Invalid priority selected.")]
    public string Priority { get; set; } = "Medium";

    [Required(ErrorMessage = "Category is required.")]
    [RegularExpression(
        "^(Technical|Application|Database|Access|Infrastructure|Other)$",
        ErrorMessage = "Invalid category selected.")]
    public string Category { get; set; } = "Other";
}