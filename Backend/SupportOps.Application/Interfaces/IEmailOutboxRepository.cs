using SupportOps.Domain.Entities;

namespace SupportOps.Application.Interfaces;

public interface IEmailOutboxRepository
{
    Task AddAsync(EmailOutbox email);
}