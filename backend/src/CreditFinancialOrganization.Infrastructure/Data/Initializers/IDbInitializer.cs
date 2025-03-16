﻿namespace CreditFinancialOrganization.Infrastructure.Data.Initializers;

public interface IDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}