using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using BloonsTD6.Mod.SalariedMonkeys.Tests.Mocks;
using Moq;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests;

internal static class TestUtilities
{
    /// <summary>
    /// Returns default reusable test data to be used with the API.
    /// </summary>
    public static FakeBloonsApi CreateDefaultTestApi(double cash = 10000.0f)
    {
        static void CreateTowerMock(Mock<ISalariedTower> tower, float towerCost, FakeBloonsApi api)
        {
            tower.Setup(x => x.GetTotalCost()).Returns(towerCost);
            tower.Setup(x => x.Sell()).Callback(() =>
            {
                api.TowersSold += 1;
                api.Towers.Remove(tower.Object);
            });
        }

        var api = new FakeBloonsApi(cash, new List<ISalariedTower>());
        api.Towers = new List<ISalariedTower>()
        {
            // Unsorted to test sorting part of selling
            CreateMock<ISalariedTower>(tower => { CreateTowerMock(tower, 4000.0f, api); }), // 200
            CreateMock<ISalariedTower>(tower => { CreateTowerMock(tower, 1000.0f, api); }), // 50
            CreateMock<ISalariedTower>(tower => { CreateTowerMock(tower, 3000.0f, api); }), // 150
            CreateMock<ISalariedTower>(tower => { CreateTowerMock(tower, 2000.0f, api); }), // 100
        };

        return api;
    }

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