namespace DBI.Runtime.Core.Memory;

/// <summary>
/// Represents PLC memory with Input (I), Output (Q), and Marker (M) memory areas.
/// </summary>
public class PlcMemory
{
    /// <summary>
    /// Input image (%I area).
    /// </summary>
    public MemoryArea I { get; }

    /// <summary>
    /// Output image (%Q area).
    /// </summary>
    public MemoryArea Q { get; }

    /// <summary>
    /// Marker/Flag memory (%M area).
    /// </summary>
    public MemoryArea M { get; }

    /// <summary>
    /// Data block simulation area.
    /// </summary>
    public MemoryArea DB { get; }

    /// <summary>
    /// Creates a new PLC memory with configurable area sizes.
    /// </summary>
    public PlcMemory(int inputSize = 256, int outputSize = 256, int markerSize = 1024, int dbSize = 4096)
    {
        I = new MemoryArea(inputSize);
        Q = new MemoryArea(outputSize);
        M = new MemoryArea(markerSize);
        DB = new MemoryArea(dbSize);
    }

    /// <summary>
    /// Clears all memory areas to zero.
    /// </summary>
    public void ClearAll()
    {
        I.Clear();
        Q.Clear();
        M.Clear();
        DB.Clear();
    }

    /// <summary>
    /// Clears only the output image.
    /// </summary>
    public void ClearOutputs() => Q.Clear();

    /// <summary>
    /// Clears only the marker memory.
    /// </summary>
    public void ClearMarkers() => M.Clear();
}
