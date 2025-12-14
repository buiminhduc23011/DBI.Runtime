using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Counters;
using DBI.Runtime.FunctionBlocks.Timers;

namespace DBI.Runtime.Tests.ScanCycle;

public class PlcRuntimeTests
{
    [Fact]
    public void Runtime_ShouldExecuteSingleCycle()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var runtime = new PlcRuntime(timeSource);
        var ctu = new CTU(timeSource) { PV = 10 };
        runtime.RegisterFunctionBlock(ctu);

        // Act - Count should trigger on rising edges
        ctu.CU = true;
        runtime.ExecuteSingleCycle(); // Rising edge 1, CV=1
        ctu.CU = false;
        runtime.ExecuteSingleCycle(); // No edge
        ctu.CU = true;
        runtime.ExecuteSingleCycle(); // Rising edge 2, CV=2

        // Assert
        Assert.Equal(2, ctu.CV);
        Assert.Equal(3, runtime.CycleCount);
    }

    [Fact]
    public void Runtime_ShouldMaintainStateAcrossCycles()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var runtime = new PlcRuntime(timeSource);
        var ton = new TON(timeSource)
        {
            IN = true,
            PT = TimeSpan.FromMilliseconds(500)
        };
        runtime.RegisterFunctionBlock(ton);
        runtime.CycleTime = TimeSpan.FromMilliseconds(100);

        // Act - run 6 cycles (600ms total simulated time)
        for (int i = 0; i < 6; i++)
        {
            runtime.ExecuteSingleCycle();
        }

        // Assert - timer should have elapsed after 500ms (6 cycles * 100ms = 600ms)
        Assert.True(ton.Q);
    }

    [Fact]
    public void Runtime_ShouldRaiseOnCycleComplete()
    {
        // Arrange
        var runtime = new PlcRuntime();
        int eventCount = 0;
        runtime.OnScanCycleComplete += (_, _) => eventCount++;

        // Act
        runtime.ExecuteSingleCycle();
        runtime.ExecuteSingleCycle();

        // Assert
        Assert.Equal(2, eventCount);
    }

    [Fact]
    public void Runtime_ShouldUnregisterFunctionBlock()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var runtime = new PlcRuntime(timeSource);
        var ctu = new CTU(timeSource) { PV = 10 };
        runtime.RegisterFunctionBlock(ctu);

        // Count once (rising edge)
        ctu.CU = true;
        runtime.ExecuteSingleCycle();
        Assert.Equal(1, ctu.CV);

        // CU goes low
        ctu.CU = false;
        runtime.ExecuteSingleCycle();

        // Unregister
        runtime.UnregisterFunctionBlock(ctu);

        // Execute cycle - counter should not increment since it's unregistered
        ctu.CU = true;
        runtime.ExecuteSingleCycle();

        // Assert - CV should still be 1 since block was unregistered
        Assert.Equal(1, ctu.CV);
    }

    [Fact]
    public void Runtime_ShouldTrackCycleCount()
    {
        // Arrange
        var runtime = new PlcRuntime();

        // Act
        runtime.ExecuteSingleCycle();
        runtime.ExecuteSingleCycle();
        runtime.ExecuteSingleCycle();

        // Assert
        Assert.Equal(3, runtime.CycleCount);
    }
}
