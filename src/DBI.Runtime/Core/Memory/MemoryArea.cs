namespace DBI.Runtime.Core.Memory;

/// <summary>
/// Represents a PLC memory area (I, Q, or M).
/// Provides byte, word, and dword access with bit addressing.
/// </summary>
public class MemoryArea
{
    private readonly byte[] _data;
    private readonly MemoryByte[] _bytes;

    /// <summary>
    /// Creates a new memory area with the specified size in bytes.
    /// </summary>
    /// <param name="sizeInBytes">Size of the memory area in bytes.</param>
    public MemoryArea(int sizeInBytes = 256)
    {
        if (sizeInBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Size must be positive");

        _data = new byte[sizeInBytes];
        _bytes = new MemoryByte[sizeInBytes];
        for (int i = 0; i < sizeInBytes; i++)
            _bytes[i] = new MemoryByte();
    }

    /// <summary>
    /// Gets the size of this memory area in bytes.
    /// </summary>
    public int Size => _data.Length;

    /// <summary>
    /// Gets a MemoryByte at the specified address for bit-level access.
    /// Usage: area[0][5] for %Mx0.5
    /// </summary>
    public MemoryByte this[int byteAddress]
    {
        get
        {
            ValidateByteAddress(byteAddress);
            // Sync internal value
            _bytes[byteAddress].Value = _data[byteAddress];
            return _bytes[byteAddress];
        }
    }

    /// <summary>
    /// Gets or sets a bit at the specified address.
    /// </summary>
    /// <param name="byteAddress">Byte address.</param>
    /// <param name="bitAddress">Bit address (0-7).</param>
    public bool GetBit(int byteAddress, int bitAddress)
    {
        ValidateByteAddress(byteAddress);
        ValidateBitAddress(bitAddress);
        return (_data[byteAddress] & (1 << bitAddress)) != 0;
    }

    /// <summary>
    /// Sets a bit at the specified address.
    /// </summary>
    public void SetBit(int byteAddress, int bitAddress, bool value)
    {
        ValidateByteAddress(byteAddress);
        ValidateBitAddress(bitAddress);
        if (value)
            _data[byteAddress] |= (byte)(1 << bitAddress);
        else
            _data[byteAddress] &= (byte)~(1 << bitAddress);
    }

    /// <summary>
    /// Gets or sets a byte at the specified address.
    /// </summary>
    public byte GetByte(int address)
    {
        ValidateByteAddress(address);
        return _data[address];
    }

    /// <summary>
    /// Sets a byte at the specified address.
    /// </summary>
    public void SetByte(int address, byte value)
    {
        ValidateByteAddress(address);
        _data[address] = value;
    }

    /// <summary>
    /// Gets a word (16-bit) at the specified byte address.
    /// Big-endian (Siemens convention): address is high byte.
    /// </summary>
    public ushort GetWord(int address)
    {
        ValidateWordAddress(address);
        return (ushort)((_data[address] << 8) | _data[address + 1]);
    }

    /// <summary>
    /// Sets a word (16-bit) at the specified byte address.
    /// Big-endian (Siemens convention): address is high byte.
    /// </summary>
    public void SetWord(int address, ushort value)
    {
        ValidateWordAddress(address);
        _data[address] = (byte)(value >> 8);
        _data[address + 1] = (byte)(value & 0xFF);
    }

    /// <summary>
    /// Gets a double word (32-bit) at the specified byte address.
    /// Big-endian (Siemens convention).
    /// </summary>
    public uint GetDWord(int address)
    {
        ValidateDWordAddress(address);
        return (uint)((_data[address] << 24) | (_data[address + 1] << 16) |
                      (_data[address + 2] << 8) | _data[address + 3]);
    }

    /// <summary>
    /// Sets a double word (32-bit) at the specified byte address.
    /// Big-endian (Siemens convention).
    /// </summary>
    public void SetDWord(int address, uint value)
    {
        ValidateDWordAddress(address);
        _data[address] = (byte)(value >> 24);
        _data[address + 1] = (byte)((value >> 16) & 0xFF);
        _data[address + 2] = (byte)((value >> 8) & 0xFF);
        _data[address + 3] = (byte)(value & 0xFF);
    }

    /// <summary>
    /// Gets a REAL (32-bit float) at the specified byte address.
    /// </summary>
    public float GetReal(int address)
    {
        uint rawValue = GetDWord(address);
        return BitConverter.Int32BitsToSingle((int)rawValue);
    }

    /// <summary>
    /// Sets a REAL (32-bit float) at the specified byte address.
    /// </summary>
    public void SetReal(int address, float value)
    {
        uint rawValue = (uint)BitConverter.SingleToInt32Bits(value);
        SetDWord(address, rawValue);
    }

    /// <summary>
    /// Gets an INT (16-bit signed) at the specified byte address.
    /// </summary>
    public short GetInt(int address) => (short)GetWord(address);

    /// <summary>
    /// Sets an INT (16-bit signed) at the specified byte address.
    /// </summary>
    public void SetInt(int address, short value) => SetWord(address, (ushort)value);

    /// <summary>
    /// Gets a DINT (32-bit signed) at the specified byte address.
    /// </summary>
    public int GetDInt(int address) => (int)GetDWord(address);

    /// <summary>
    /// Sets a DINT (32-bit signed) at the specified byte address.
    /// </summary>
    public void SetDInt(int address, int value) => SetDWord(address, (uint)value);

    /// <summary>
    /// Clears all memory to zero.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_data, 0, _data.Length);
        foreach (var mb in _bytes)
            mb.Clear();
    }

    /// <summary>
    /// Copies the raw data to the specified array.
    /// </summary>
    public void CopyTo(byte[] destination, int destinationIndex = 0)
    {
        Array.Copy(_data, 0, destination, destinationIndex, _data.Length);
    }

    /// <summary>
    /// Copies data from the specified array into this memory area.
    /// </summary>
    public void CopyFrom(byte[] source, int sourceIndex = 0, int length = -1)
    {
        if (length < 0) length = Math.Min(source.Length - sourceIndex, _data.Length);
        Array.Copy(source, sourceIndex, _data, 0, length);
    }

    private void ValidateByteAddress(int address)
    {
        if (address < 0 || address >= _data.Length)
            throw new ArgumentOutOfRangeException(nameof(address), $"Byte address must be 0-{_data.Length - 1}");
    }

    private void ValidateWordAddress(int address)
    {
        if (address < 0 || address + 1 >= _data.Length)
            throw new ArgumentOutOfRangeException(nameof(address), $"Word address must be 0-{_data.Length - 2}");
    }

    private void ValidateDWordAddress(int address)
    {
        if (address < 0 || address + 3 >= _data.Length)
            throw new ArgumentOutOfRangeException(nameof(address), $"DWord address must be 0-{_data.Length - 4}");
    }

    private static void ValidateBitAddress(int bit)
    {
        if (bit < 0 || bit > 7)
            throw new ArgumentOutOfRangeException(nameof(bit), "Bit address must be 0-7");
    }
}
