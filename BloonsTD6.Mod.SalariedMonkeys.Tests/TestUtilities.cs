using Moq;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests;

internal static class TestUtilities
{
    /// <summary>
    /// Runs a method to configure a mock, and returns the mocked object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setupCode">The code to setup the mock.</param>
    public static T CreateMock<T>(Action<Mock<T>> setupCode) where T : class
    {
        var mock = new Mock<T>();
        return Configure(mock, setupCode);
    }

    /// <summary>
    /// Runs a method to configure a mock, and returns the mocked object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mock">The mock in question.</param>
    /// <param name="setupCode">The code to setup the mock.</param>
    public static T Configure<T>(this Mock<T> mock, Action<Mock<T>> setupCode) where T : class
    {
        setupCode(mock);
        return mock.Object;
    }
}