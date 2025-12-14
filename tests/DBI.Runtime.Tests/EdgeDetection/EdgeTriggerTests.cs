using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.EdgeDetection;

namespace DBI.Runtime.Tests.EdgeDetection;

public class EdgeTriggerTests
{
    [Fact]
    public void R_TRIG_ShouldDetectRisingEdge()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var rTrig = new R_TRIG(timeSource);

        // Act - FALSE to TRUE
        rTrig.CLK = false;
        rTrig.Execute();
        rTrig.CLK = true;
        rTrig.Execute();

        // Assert
        Assert.True(rTrig.Q);
    }

    [Fact]
    public void R_TRIG_ShouldNotRetrigger_WhenClkStaysTrue()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var rTrig = new R_TRIG(timeSource);

        // Act
        rTrig.CLK = true;
        rTrig.Execute();
        rTrig.Execute(); // Second cycle with CLK still TRUE

        // Assert
        Assert.False(rTrig.Q);
    }

    [Fact]
    public void F_TRIG_ShouldDetectFallingEdge()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var fTrig = new F_TRIG(timeSource);

        // Act - Initialize and then TRUE to FALSE
        fTrig.CLK = true;
        fTrig.Execute();
        fTrig.CLK = false;
        fTrig.Execute();

        // Assert
        Assert.True(fTrig.Q);
    }

    [Fact]
    public void F_TRIG_ShouldNotRetrigger_WhenClkStaysFalse()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var fTrig = new F_TRIG(timeSource);

        // Act
        fTrig.CLK = true;
        fTrig.Execute();
        fTrig.CLK = false;
        fTrig.Execute();
        fTrig.Execute(); // Second cycle with CLK still FALSE

        // Assert
        Assert.False(fTrig.Q);
    }
}
