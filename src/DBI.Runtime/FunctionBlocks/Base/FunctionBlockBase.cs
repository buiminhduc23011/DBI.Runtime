using DBI.Runtime.Core.Interfaces;

namespace DBI.Runtime.FunctionBlocks.Base;

/// <summary>
/// Base class for all Function Blocks.
/// Provides common functionality and time source access.
/// </summary>
public abstract class FunctionBlockBase : IFunctionBlock
{
    /// <summary>
    /// Gets the time source for timer operations.
    /// </summary>
    protected ITimeSource TimeSource { get; }

    /// <summary>
    /// Creates a new function block with the specified time source.
    /// </summary>
    protected FunctionBlockBase(ITimeSource timeSource)
    {
        TimeSource = timeSource ?? throw new ArgumentNullException(nameof(timeSource));
    }

    /// <summary>
    /// Executes one cycle of the function block.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Gets the current time from the time source.
    /// </summary>
    protected TimeSpan CurrentTime => TimeSource.Now;
}
