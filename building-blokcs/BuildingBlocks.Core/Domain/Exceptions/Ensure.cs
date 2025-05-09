using BuildingBlocks.Core.Exception;
using BuildingBlocks.Core.Extensions;

namespace BuildingBlocks.Core.Domain.Exceptions;
public static class Ensure
{
    public static void NotNullOrEmpty<TException>(string value)
        where TException : BaseException
    {
        if (string.IsNullOrEmpty(value))
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNullOrEmpty<TException>(string value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (string.IsNullOrEmpty(value))
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNull<T, TException>(T value)
        where TException : BaseException
        where T : class
    {
        if (value == null)
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNull<T, TException>(T value, params object[] exceptionConstructorArgs)
        where TException : BaseException
        where T : class
    {
        if (value == null)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNull<TException>(string value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value == null)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegative<TException>(int value)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegative<TException>(int value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegative<TException>(double value)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegative<TException>(double value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegative<TException>(decimal value)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegative<TException>(decimal value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegative<TException>(long value)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegative<TException>(long value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegative())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegativeOrZero<TException>(int value)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegativeOrZero<TException>(int value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegativeOrZero<TException>(double value)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegativeOrZero<TException>(double value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegativeOrZero<TException>(decimal value)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegativeOrZero<TException>(decimal value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void NotNegativeOrZero<TException>(long value)
       where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void NotNegativeOrZero<TException>(long value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value.IsNegativeOrZero())
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void True<TException>(bool value)
        where TException : BaseException
    {
        if (!value)
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void True<TException>(bool value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (!value)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void False<TException>(bool value)
        where TException : BaseException
    {
        if (value)
            throw (TException)Activator.CreateInstance(typeof(TException))!;
    }

    public static void False<TException>(bool value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void Positive<TException>(long? value, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value is not > 0)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }

    public static void InclusiveBetween<TException>(int value, int min, int max, params object[] exceptionConstructorArgs)
        where TException : BaseException
    {
        if (value < min || value > max)
            throw (TException)Activator.CreateInstance(typeof(TException), exceptionConstructorArgs)!;
    }
}