namespace DBI.Runtime.Core.Interfaces;

/// <summary>
/// Base interface for all PLC Function Blocks (FB).
/// Function Blocks maintain internal state across scan cycles.
/// </summary>
public interface IFunctionBlock
{
    /// <summary>
    /// Executes one cycle of the function block.
    /// Called once per PLC scan cycle.
    /// </summary>
    void Execute();
}
