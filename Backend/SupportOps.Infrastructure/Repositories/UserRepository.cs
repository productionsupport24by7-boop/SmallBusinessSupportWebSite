using Microsoft.EntityFrameworkCore;
using SupportOps.Application.Interfaces;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Data;
namespace SupportOps.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SupportOpsDbContext _context;

    public UserRepository(SupportOpsDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email && x.IsActive);
    }
}