using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Timers;

namespace DBI.Runtime.Tests.Timers;

public class TPTests
{
    [Fact]
    public void TP_ShouldGeneratePulse_OnRisingEdge()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tp = new TP(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act - rising edge
        tp.IN = true;
        tp.Execute();

        // Assert
        Assert.True(tp.Q);
    }

    [Fact]
    public void TP_ShouldMaintainPulse_ForDuration()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tp = new TP(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        tp.IN = true;
        tp.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(500));
        tp.Execute();

        // Assert - still in pulse
        Assert.True(tp.Q);
    }

    [Fact]
    public void TP_ShouldEndPulse_AfterDuration()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tp = new TP(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        tp.IN = true;
        tp.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(1000));
        tp.Execute();

        // Assert
        Assert.False(tp.Q);
    }

    [Fact]
    public void TP_ShouldNotRetrigger_DuringPulse()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tp = new TP(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act - start pulse
        tp.IN = true;
        tp.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(200));

        // Try to retrigger
        tp.IN = false;
        tp.Execute();
        tp.IN = true;
        tp.Execute();

        // Continue timing
        timeSource.Tick(TimeSpan.FromMilliseconds(700));
        tp.Execute();

        // Assert - original pulse should continue (900ms total, still < 1000ms)
        Assert.True(tp.Q);
    }
}
