using System;
using System.Collections.Generic;
using System.Text;

using SupportOps.Application.DTOs.Users;

namespace SupportOps.Application.Interfaces;

public interface ISupportAgentService
{
    Task<List<SupportAgentResponse>> GetAllSupportAgentsAsync();

    Task<bool> IsSupportAgentAsync(int userId);
}