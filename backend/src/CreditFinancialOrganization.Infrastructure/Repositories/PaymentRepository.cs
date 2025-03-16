using CreditFinancialOrganization.Domain.Entities.Payments;
using CreditFinancialOrganization.Domain.Repositories;
using CreditFinancialOrganization.Infrastructure.Data;

namespace CreditFinancialOrganization.Infrastructure.Repositories;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }
}