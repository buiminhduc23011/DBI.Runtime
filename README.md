# DBI.Runtime

Thư viện C# mô phỏng runtime PLC Siemens S7-1200/S7-1500 trên .NET 8+.

Hỗ trợ chuẩn **IEC 61131-3** và các extension đặc thù của Siemens.

## Cài đặt

```bash
# Clone repository
git clone https://github.com/buiminhduc23011/DBI.Runtime.git

# Build
cd DBI.Runtime
dotnet build

# Chạy tests
dotnet test
```

## Bắt đầu nhanh

### Tạo PLC Runtime

```csharp
using DBI.Runtime.Core.Runtime;
using DBI.Runtime.FunctionBlocks.Timers;
using DBI.Runtime.FunctionBlocks.Counters;

// Tạo runtime với cycle time 100ms
var runtime = new PlcRuntime();
runtime.CycleTime = TimeSpan.FromMilliseconds(100);

// Tạo function blocks
var timer = new TON(runtime.TimeSource) { PT = TimeSpan.FromSeconds(5) };
var counter = new CTU(runtime.TimeSource) { PV = 10 };

// Đăng ký blocks để tự động thực thi mỗi scan cycle
runtime.RegisterFunctionBlock(timer);
runtime.RegisterFunctionBlock(counter);

// Khởi động runtime (chạy background)
runtime.Start();

// Dừng runtime
runtime.Stop();
```

### Thực thi thủ công (cho testing)

```csharp
var timeSource = MonotonicTimeSource.CreateSimulated();
var runtime = new PlcRuntime(timeSource);

// Thực thi từng cycle
runtime.ExecuteSingleCycle();
```

---

## Các Function Blocks

### Timers (Bộ định thời)

#### TON - On-Delay Timer

```csharp
var ton = new TON(timeSource)
{
    PT = TimeSpan.FromMilliseconds(1000)  // Preset Time: 1 giây
};

// Sử dụng trong vòng lặp scan
ton.IN = true;   // Input
ton.Execute();

// Đọc outputs
bool output = ton.Q;      // TRUE khi đã đủ thời gian
TimeSpan elapsed = ton.ET; // Thời gian đã trôi qua
```

**Hoạt động:** Q = TRUE khi IN = TRUE liên tục trong thời gian >= PT

#### TOF - Off-Delay Timer

```csharp
var tof = new TOF(timeSource)
{
    PT = TimeSpan.FromMilliseconds(2000)
};

tof.IN = sensorSignal;
tof.Execute();
// Q vẫn TRUE trong 2 giây sau khi IN chuyển từ TRUE → FALSE
```

#### TP - Pulse Timer

```csharp
var tp = new TP(timeSource)
{
    PT = TimeSpan.FromMilliseconds(500)
};

tp.IN = buttonPressed;
tp.Execute();
// Q = TRUE trong đúng 500ms sau rising edge của IN
```

#### TONR - Retentive On-Delay Timer

```csharp
var tonr = new TONR(timeSource)
{
    PT = TimeSpan.FromSeconds(60)
};

tonr.IN = motorRunning;
tonr.R = resetButton;  // Reset tích lũy
tonr.Execute();
// ET tích lũy thời gian qua nhiều lần IN = TRUE
```

---

### Counters (Bộ đếm)

#### CTU - Count Up

```csharp
var ctu = new CTU(timeSource)
{
    PV = 100  // Preset Value
};

ctu.CU = pulseSignal;  // Count Up input
ctu.R = resetButton;   // Reset
ctu.Execute();

int count = ctu.CV;    // Current Value
bool done = ctu.Q;     // TRUE khi CV >= PV
```

#### CTD - Count Down

```csharp
var ctd = new CTD(timeSource)
{
    PV = 50
};

ctd.CD = pulseSignal;  // Count Down input
ctd.LD = loadButton;   // Load PV vào CV
ctd.Execute();

bool zero = ctd.Q;     // TRUE khi CV <= 0
```

#### CTUD - Up/Down Counter

```csharp
var ctud = new CTUD(timeSource)
{
    PV = 100
};

ctud.CU = upPulse;
ctud.CD = downPulse;
ctud.R = resetBtn;
ctud.LD = loadBtn;
ctud.Execute();

bool atMax = ctud.QU;  // CV >= PV
bool atZero = ctud.QD; // CV <= 0
```

---

### Edge Detection (Phát hiện cạnh)

#### R_TRIG - Rising Edge

```csharp
var rTrig = new R_TRIG(timeSource);

rTrig.CLK = inputSignal;
rTrig.Execute();

if (rTrig.Q)
{
    // Chỉ TRUE trong 1 scan khi CLK: FALSE → TRUE
    Console.WriteLine("Rising edge detected!");
}
```

#### F_TRIG - Falling Edge

```csharp
var fTrig = new F_TRIG(timeSource);

fTrig.CLK = inputSignal;
fTrig.Execute();

if (fTrig.Q)
{
    // Chỉ TRUE trong 1 scan khi CLK: TRUE → FALSE
    Console.WriteLine("Falling edge detected!");
}
```

---

### Logic Blocks

#### SR - Set Dominant

```csharp
var sr = new SR(timeSource);

sr.SET1 = setButton;
sr.RESET = resetButton;
sr.Execute();

// Khi cả SET1 và RESET đều TRUE → Q1 = TRUE (SET ưu tiên)
bool output = sr.Q1;
```

#### RS - Reset Dominant

```csharp
var rs = new RS(timeSource);

rs.SET = setButton;
rs.RESET1 = resetButton;
rs.Execute();

// Khi cả SET và RESET1 đều TRUE → Q1 = FALSE (RESET ưu tiên)
```

#### FlipFlop - Toggle

```csharp
var ff = new FlipFlop(timeSource);

ff.CLK = toggleButton;
ff.RST = resetButton;
ff.Execute();

// Q đảo trạng thái mỗi rising edge của CLK
```

---

## Các Functions (Stateless)

### Math Functions

```csharp
using DBI.Runtime.Functions.Math;

int sum = MathFunctions.ADD(10, 20);           // 30
float diff = MathFunctions.SUB(50.5f, 10.2f);  // 40.3
int product = MathFunctions.MUL(5, 6);         // 30
float quotient = MathFunctions.DIV(100f, 3f);  // 33.33...

int limited = MathFunctions.LIMIT(0, value, 100);  // Giới hạn 0-100
int minVal = MathFunctions.MIN(a, b);
int maxVal = MathFunctions.MAX(a, b);

float sqrt = MathFunctions.SQRT(16f);   // 4
float abs = MathFunctions.ABS(-25.5f);  // 25.5
```

### Compare Functions

```csharp
using DBI.Runtime.Functions.Compare;

bool eq = CompareFunctions.EQ(10, 10);   // TRUE
bool ne = CompareFunctions.NE(10, 20);   // TRUE
bool gt = CompareFunctions.GT(20, 10);   // TRUE (20 > 10)
bool lt = CompareFunctions.LT(5, 10);    // TRUE (5 < 10)
bool ge = CompareFunctions.GE(10, 10);   // TRUE (10 >= 10)
bool le = CompareFunctions.LE(5, 10);    // TRUE (5 <= 10)

bool inRange = CompareFunctions.IN_RANGE(50, 0, 100);  // TRUE
```

### Bit Logic

```csharp
using DBI.Runtime.Functions.BitLogic;

// Logical operations
byte result = PlcBitLogic.AND((byte)0xF0, (byte)0x0F);  // 0x00
ushort orResult = PlcBitLogic.OR((ushort)0xFF00, (ushort)0x00FF);  // 0xFFFF
uint xorResult = PlcBitLogic.XOR(0xAAAAAAAAu, 0x55555555u);  // 0xFFFFFFFF

// Shift operations
byte shifted = PlcBitLogic.SHL((byte)0x01, 4);  // 0x10
ushort rotated = PlcBitLogic.ROL((ushort)0x8000, 1);  // 0x0001

// Bit manipulation
bool bitValue = PlcBitLogic.TEST_BIT((byte)0b00001000, 3);  // TRUE
byte setBit = PlcBitLogic.SET_BIT((byte)0x00, 5);    // 0x20
byte resetBit = PlcBitLogic.RESET_BIT((byte)0xFF, 3); // 0xF7
byte toggled = PlcBitLogic.TOGGLE_BIT((byte)0x00, 0); // 0x01

// Byte swap (endian conversion)
ushort swapped = PlcBitLogic.SWAP((ushort)0xABCD);  // 0xCDAB
```

### Convert Functions

```csharp
using DBI.Runtime.Functions.Convert;

// Type conversions
int intVal = ConvertFunctions.BOOL_TO_INT(true);    // 1
float realVal = ConvertFunctions.INT_TO_REAL((short)42);  // 42.0f
short intResult = ConvertFunctions.REAL_TO_INT(3.7f);  // 3

// BCD conversions (Siemens-specific)
int bcdToInt = ConvertFunctions.BCD_TO_INT(0x1234);  // 1234
ushort intToBcd = ConvertFunctions.INT_TO_BCD(5678);  // 0x5678

// Rounding
int rounded = ConvertFunctions.ROUND(3.6f);  // 4
int truncated = ConvertFunctions.TRUNC(3.9f);  // 3
```

---

## Memory System

### PLC Memory Areas

```csharp
var memory = new PlcMemory(
    inputSize: 256,   // %I area
    outputSize: 256,  // %Q area
    markerSize: 1024, // %M area
    dbSize: 4096      // DB area
);

// Truy cập byte
memory.M.SetByte(0, 0xFF);
byte value = memory.M.GetByte(0);

// Truy cập word (16-bit, Big-Endian như Siemens)
memory.M.SetWord(10, 0x1234);
ushort wordVal = memory.M.GetWord(10);

// Truy cập dword (32-bit)
memory.M.SetDWord(20, 0x12345678);
uint dwordVal = memory.M.GetDWord(20);

// Truy cập bit (%Mx.y style)
memory.M.SetBit(0, 5, true);  // %M0.5 = TRUE
bool bit = memory.M.GetBit(0, 5);

// Hoặc dùng indexer
memory.M[0][5] = true;  // %M0.5 = TRUE
bool bitVal = memory.M[0][5];
```

---

## PLC Runtime

### Cấu hình và chạy

```csharp
var runtime = new PlcRuntime();

// Cấu hình
runtime.CycleTime = TimeSpan.FromMilliseconds(50);  // 50ms cycle

// Event khi hoàn thành mỗi scan cycle
runtime.OnScanCycleComplete += (sender, args) =>
{
    Console.WriteLine($"Cycle {args.CycleNumber}: {args.CycleDuration.TotalMilliseconds}ms");
};

// Đăng ký function blocks
runtime.RegisterFunctionBlock(myTimer);
runtime.RegisterFunctionBlock(myCounter);

// Chạy runtime
runtime.Start();

// ... ứng dụng chạy ...

// Dừng và cleanup
runtime.Stop();
runtime.Dispose();
```

### Testing với Simulated Time

```csharp
// Tạo time source giả lập
var timeSource = MonotonicTimeSource.CreateSimulated();
var runtime = new PlcRuntime(timeSource);
var ton = new TON(timeSource) { PT = TimeSpan.FromSeconds(5), IN = true };

runtime.RegisterFunctionBlock(ton);

// Chạy 60 cycles (mỗi cycle = 100ms → 6 giây)
for (int i = 0; i < 60; i++)
{
    runtime.ExecuteSingleCycle();
}

// Timer đã expire
Assert.True(ton.Q);
```

---

## Cấu trúc Project

```
DBI.Runtime/
├── src/DBI.Runtime/
│   ├── Core/
│   │   ├── Interfaces/      # IFunctionBlock, IPlcRuntime, ITimeSource
│   │   ├── Types/           # PlcTime, PlcWord, PlcDWord, PlcByte
│   │   ├── Memory/          # PlcMemory, MemoryArea
│   │   └── Runtime/         # PlcRuntime, MonotonicTimeSource
│   ├── FunctionBlocks/
│   │   ├── Timers/          # TON, TOF, TP, TONR, S_ODT, S_OFFDT, S_PULSE
│   │   ├── Counters/        # CTU, CTD, CTUD
│   │   ├── EdgeDetection/   # R_TRIG, F_TRIG
│   │   └── Logic/           # SR, RS, FlipFlop
│   └── Functions/
│       ├── Math/            # MathFunctions
│       ├── Compare/         # CompareFunctions
│       ├── Convert/         # ConvertFunctions
│       └── BitLogic/        # PlcBitLogic
└── tests/DBI.Runtime.Tests/
```

---

## Danh sách Function Blocks

| Category | Block | Description |
|----------|-------|-------------|
| **Timers** | TON | On-Delay Timer |
| | TOF | Off-Delay Timer |
| | TP | Pulse Timer |
| | TONR | Retentive On-Delay |
| | S_ODT | S5 On-Delay |
| | S_OFFDT | S5 Off-Delay |
| | S_PULSE | S5 Pulse |
| **Counters** | CTU | Count Up |
| | CTD | Count Down |
| | CTUD | Up/Down Counter |
| **Edge** | R_TRIG | Rising Edge |
| | F_TRIG | Falling Edge |
| **Logic** | SR | Set-Reset (Set dominant) |
| | RS | Reset-Set (Reset dominant) |
| | FlipFlop | Toggle on clock |

---

## Chạy Tests

```bash
# Chạy tất cả tests
dotnet test

# Chạy với output chi tiết
dotnet test --verbosity normal

# Chạy tests cụ thể
dotnet test --filter "FullyQualifiedName~TON"
```

---

## License

MIT License

---

## Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request
