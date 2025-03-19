using System.Runtime.CompilerServices;

namespace CreditFinancialOrganization.Domain.Exceptions;

public class ArgumentEmptyException : ArgumentException
{
    public ArgumentEmptyException(string message, string? paramName)
        : base(message, paramName)
    {
    }

    public static void ThrowIfEmpty(
        Guid id,
        [CallerArgumentExpression(nameof(id))] string? paramName = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentEmptyException("CustomerId cannot be empty", paramName);
        }
    }
}