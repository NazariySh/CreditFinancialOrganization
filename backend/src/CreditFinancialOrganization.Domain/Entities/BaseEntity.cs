namespace CreditFinancialOrganization.Domain.Entities;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; set; }
}

public interface IEntity
{
    Guid Id { get; set; }
}