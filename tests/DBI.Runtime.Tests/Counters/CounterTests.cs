using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Counters;

namespace DBI.Runtime.Tests.Counters;

public class CounterTests
{
    [Fact]
    public void CTU_ShouldCountUp()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctu = new CTU(timeSource) { PV = 5 };

        // Act - count 3 times
        for (int i = 0; i < 3; i++)
        {
            ctu.CU = false;
            ctu.Execute();
            ctu.CU = true;
            ctu.Execute();
        }

        // Assert
        Assert.Equal(3, ctu.CV);
        Assert.False(ctu.Q);
    }

    [Fact]
    public void CTU_ShouldSetQ_WhenReachesPreset()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctu = new CTU(timeSource) { PV = 3 };

        // Act - count 3 times
        for (int i = 0; i < 3; i++)
        {
            ctu.CU = false;
            ctu.Execute();
            ctu.CU = true;
            ctu.Execute();
        }

        // Assert
        Assert.Equal(3, ctu.CV);
        Assert.True(ctu.Q);
    }

    [Fact]
    public void CTU_ShouldReset_WhenRIsTrue()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctu = new CTU(timeSource) { PV = 5 };

        // Act - count then reset
        ctu.CU = true;
        ctu.Execute();
        ctu.CU = false;
        ctu.Execute();
        ctu.CU = true;
        ctu.Execute();
        ctu.R = true;
        ctu.Execute();

        // Assert
        Assert.Equal(0, ctu.CV);
        Assert.False(ctu.Q);
    }

    [Fact]
    public void CTD_ShouldCountDown()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctd = new CTD(timeSource) { PV = 5 };

        // Load preset
        ctd.LD = true;
        ctd.Execute();
        ctd.LD = false;

        // Count down 2 times
        for (int i = 0; i < 2; i++)
        {
            ctd.CD = false;
            ctd.Execute();
            ctd.CD = true;
            ctd.Execute();
        }

        // Assert
        Assert.Equal(3, ctd.CV);
        Assert.False(ctd.Q);
    }

    [Fact]
    public void CTD_ShouldSetQ_WhenReachesZero()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctd = new CTD(timeSource) { PV = 2 };

        // Load preset
        ctd.LD = true;
        ctd.Execute();
        ctd.LD = false;

        // Count down to 0
        for (int i = 0; i < 2; i++)
        {
            ctd.CD = false;
            ctd.Execute();
            ctd.CD = true;
            ctd.Execute();
        }

        // Assert
        Assert.Equal(0, ctd.CV);
        Assert.True(ctd.Q);
    }

    [Fact]
    public void CTUD_ShouldCountBothDirections()
    {
        // Arrange
        var timeSource = MonotonicTimeSource.CreateSimulated();
        var ctud = new CTUD(timeSource) { PV = 5 };

        // Count up 3 times
        for (int i = 0; i < 3; i++)
        {
            ctud.CU = false;
            ctud.Execute();
            ctud.CU = true;
            ctud.Execute();
        }

        Assert.Equal(3, ctud.CV);

        // Count down 1 time
        ctud.CU = false;
        ctud.CD = false;
        ctud.Execute();
        ctud.CD = true;
        ctud.Execute();

        // Assert
        Assert.Equal(2, ctud.CV);
    }
}
