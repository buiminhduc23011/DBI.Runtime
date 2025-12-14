namespace DBI.Runtime.Core.Interfaces;

/// <summary>
/// Base interface for PLC Functions (FC).
/// Functions are stateless and return a result.
/// </summary>
/// <typeparam name="TResult">The return type of the function.</typeparam>
public interface IFunction<TResult>
{
    /// <summary>
    /// Executes the function and returns the result.
    /// </summary>
    TResult Execute();
}

/// <summary>
/// Non-generic function interface for void functions.
/// </summary>
public interface IFunction
{
    /// <summary>
    /// Executes the function.
    /// </summary>
    void Execute();
}
