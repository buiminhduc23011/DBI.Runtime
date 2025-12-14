using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Logic;

namespace DBI.Runtime.Tests.Logic;

public class LogicBlockTests
{
    [Fact]
    public void SR_SetShouldDominate()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var sr = new SR(timeSource);

        // Act - both SET1 and RESET true
        sr.SET1 = true;
        sr.RESET = true;
        sr.Execute();

        // Assert - SET dominates
        Assert.True(sr.Q1);
    }

    [Fact]
    public void SR_ShouldReset_WhenOnlyResetTrue()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var sr = new SR(timeSource);

        // Set first
        sr.SET1 = true;
        sr.Execute();
        Assert.True(sr.Q1);

        // Then reset
        sr.SET1 = false;
        sr.RESET = true;
        sr.Execute();

        // Assert
        Assert.False(sr.Q1);
    }

    [Fact]
    public void RS_ResetShouldDominate()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var rs = new RS(timeSource);

        // Act - both SET and RESET1 true
        rs.SET = true;
        rs.RESET1 = true;
        rs.Execute();

        // Assert - RESET dominates
        Assert.False(rs.Q1);
    }

    [Fact]
    public void FlipFlop_ShouldToggle_OnRisingEdge()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ff = new FlipFlop(timeSource);

        // Initial state
        Assert.False(ff.Q);

        // First toggle
        ff.CLK = true;
        ff.Execute();
        Assert.True(ff.Q);

        // Keep CLK true - should not toggle
        ff.Execute();
        Assert.True(ff.Q);

        // Another toggle
        ff.CLK = false;
        ff.Execute();
        ff.CLK = true;
        ff.Execute();
        Assert.False(ff.Q);
    }
}
