using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Timers;

namespace DBI.Runtime.Tests.Timers;

public class TOFTests
{
    [Fact]
    public void TOF_ShouldBeTrue_WhenInputIsTrue()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tof = new TOF(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        tof.IN = true;
        tof.Execute();

        // Assert
        Assert.True(tof.Q);
        Assert.Equal(TimeSpan.Zero, tof.ET);
    }

    [Fact]
    public void TOF_ShouldDelayOff()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tof = new TOF(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act - IN goes TRUE then FALSE
        tof.IN = true;
        tof.Execute();
        tof.IN = false;
        tof.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(500));
        tof.Execute();

        // Assert - should still be TRUE during delay
        Assert.True(tof.Q);
        Assert.True(tof.ET >= TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void TOF_ShouldTurnOff_AfterDelayElapsed()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var tof = new TOF(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        tof.IN = true;
        tof.Execute();
        tof.IN = false;
        tof.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(1000));
        tof.Execute();

        // Assert
        Assert.False(tof.Q);
        Assert.Equal(tof.PT, tof.ET);
    }
}
