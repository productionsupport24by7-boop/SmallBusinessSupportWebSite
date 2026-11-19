using System;
using System.Collections.Generic;
using System.Text;


namespace SupportOps.Application.DTOs.Users;

public class SupportAgentResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
