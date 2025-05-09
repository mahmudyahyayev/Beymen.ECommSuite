namespace BuildingBlocks.Core.Extensions;

public static class PrimitiveDataTypeExtensions
{
    public static bool IsNegative(this int value) => value < 0;
    public static bool IsNegative(this decimal value) => value < 0;
    public static bool IsNegative(this double value) => value < 0;
    public static bool IsNegative(this long value) => value < 0;
    public static bool IsNegativeOrZero(this int value) => value <= 0;
    public static bool IsNegativeOrZero(this decimal value) => value <= 0;
    public static bool IsNegativeOrZero(this double value) => value <= 0;
    public static bool IsNegativeOrZero(this long value) => value <= 0;
}
