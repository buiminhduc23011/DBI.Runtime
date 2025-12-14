using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Timers;

namespace DBI.Runtime.Tests.Timers;

public class TONTests
{
    [Fact]
    public void TON_ShouldNotActivate_WhenTimeNotElapsed()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ton = new TON(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        ton.IN = true;
        ton.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(500));
        ton.Execute();

        // Assert
        Assert.False(ton.Q);
        Assert.True(ton.ET >= TimeSpan.FromMilliseconds(500));
        Assert.True(ton.ET < ton.PT);
    }

    [Fact]
    public void TON_ShouldActivate_WhenTimeElapsed()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ton = new TON(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act
        ton.IN = true;
        ton.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(1000));
        ton.Execute();

        // Assert
        Assert.True(ton.Q);
        Assert.Equal(ton.PT, ton.ET);
    }

    [Fact]
    public void TON_ShouldReset_WhenInputGoesLow()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ton = new TON(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(1000)
        };

        // Act - start timing
        ton.IN = true;
        ton.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(500));
        ton.Execute();

        // Input goes low
        ton.IN = false;
        ton.Execute();

        // Assert
        Assert.False(ton.Q);
        Assert.Equal(TimeSpan.Zero, ton.ET);
    }

    [Fact]
    public void TON_ShouldStayActive_AfterTimeElapsed()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ton = new TON(timeSource)
        {
            PT = TimeSpan.FromMilliseconds(500)
        };

        // Act
        ton.IN = true;
        ton.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(1000));
        ton.Execute();
        timeSource.Tick(TimeSpan.FromMilliseconds(1000));
        ton.Execute();

        // Assert - Q should stay TRUE as long as IN is TRUE
        Assert.True(ton.Q);
        Assert.Equal(ton.PT, ton.ET); // ET clamped to PT
    }
}
