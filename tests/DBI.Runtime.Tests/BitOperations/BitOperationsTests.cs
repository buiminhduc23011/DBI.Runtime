using DBI.Runtime.Functions.BitLogic;

namespace DBI.Runtime.Tests.BitOperations;

public class BitOperationsTests
{
    [Fact]
    public void AND_ShouldPerformBitwiseAnd()
    {
        Assert.Equal((byte)0b00001100, PlcBitLogic.AND((byte)0b00101100, (byte)0b00011110));
        Assert.Equal((ushort)0x00F0, PlcBitLogic.AND((ushort)0x0FF0, (ushort)0x00FF));
        Assert.Equal(0x000F000Fu, PlcBitLogic.AND(0x00FF00FFu, 0x0F0F0F0Fu));
    }

    [Fact]
    public void OR_ShouldPerformBitwiseOr()
    {
        Assert.Equal((byte)0b00111110, PlcBitLogic.OR((byte)0b00101100, (byte)0b00011110));
        Assert.Equal((ushort)0x0FFF, PlcBitLogic.OR((ushort)0x0FF0, (ushort)0x00FF));
    }

    [Fact]
    public void XOR_ShouldPerformBitwiseXor()
    {
        Assert.Equal((byte)0b00110010, PlcBitLogic.XOR((byte)0b00101100, (byte)0b00011110));
    }

    [Fact]
    public void NOT_ShouldInvertBits()
    {
        Assert.Equal((byte)0b11010011, PlcBitLogic.NOT((byte)0b00101100));
    }

    [Fact]
    public void SHL_ShouldShiftLeft()
    {
        Assert.Equal((byte)0b10110000, PlcBitLogic.SHL((byte)0b00101100, 2));
        Assert.Equal((ushort)0x0F00, PlcBitLogic.SHL((ushort)0x00F0, 4));
        Assert.Equal(0x0000FF00u, PlcBitLogic.SHL(0x000000FFu, 8));
    }

    [Fact]
    public void SHR_ShouldShiftRight()
    {
        Assert.Equal((byte)0b00001011, PlcBitLogic.SHR((byte)0b00101100, 2));
        Assert.Equal((ushort)0x000F, PlcBitLogic.SHR((ushort)0x00F0, 4));
    }

    [Fact]
    public void ROL_ShouldRotateLeft()
    {
        Assert.Equal((byte)0b01011001, PlcBitLogic.ROL((byte)0b10101100, 1));
    }

    [Fact]
    public void ROR_ShouldRotateRight()
    {
        Assert.Equal((byte)0b01010110, PlcBitLogic.ROR((byte)0b10101100, 1));
    }

    [Fact]
    public void TEST_BIT_ShouldReturnCorrectValue()
    {
        byte value = 0b00101100;
        Assert.False(PlcBitLogic.TEST_BIT(value, 0));
        Assert.False(PlcBitLogic.TEST_BIT(value, 1));
        Assert.True(PlcBitLogic.TEST_BIT(value, 2));
        Assert.True(PlcBitLogic.TEST_BIT(value, 3));
        Assert.False(PlcBitLogic.TEST_BIT(value, 4));
        Assert.True(PlcBitLogic.TEST_BIT(value, 5));
    }

    [Fact]
    public void SET_BIT_ShouldSetSpecificBit()
    {
        Assert.Equal((byte)0x08, PlcBitLogic.SET_BIT((byte)0x00, 3));
        Assert.Equal((ushort)0x0100, PlcBitLogic.SET_BIT((ushort)0x0000, 8));
        Assert.Equal(0x80000000u, PlcBitLogic.SET_BIT(0x00000000u, 31));
    }

    [Fact]
    public void RESET_BIT_ShouldClearSpecificBit()
    {
        Assert.Equal((byte)0xF7, PlcBitLogic.RESET_BIT((byte)0xFF, 3));
        Assert.Equal((ushort)0xFEFF, PlcBitLogic.RESET_BIT((ushort)0xFFFF, 8));
    }

    [Fact]
    public void TOGGLE_BIT_ShouldToggleSpecificBit()
    {
        Assert.Equal((byte)0x08, PlcBitLogic.TOGGLE_BIT((byte)0x00, 3));
        Assert.Equal((byte)0x00, PlcBitLogic.TOGGLE_BIT((byte)0x08, 3));
    }

    [Fact]
    public void SWAP_ShouldSwapBytes()
    {
        Assert.Equal((ushort)0xCDAB, PlcBitLogic.SWAP((ushort)0xABCD));
        Assert.Equal(0x78563412u, PlcBitLogic.SWAP(0x12345678u));
    }
}

