using BloonsTD6.Mod.SalariedMonkeys.Implementation;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using static BloonsTD6.Mod.SalariedMonkeys.Tests.TestUtilities;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests;

public class TowerManagerTests
{
    [Theory]
    [InlineData(0.05, 500, 2500)]
    [InlineData(0.10, 1000, 2000)]
    [InlineData(0.20, 2000, 1000)]
    public void GetSalary(float costPercentPerRound, float expectedResult, float availableSalary)
    {
        const float totalCash = 3000.0f;

        // Arrange
        var fakeBloonsApi = CreateMock<IBloonsApi>(mock =>
        {
            // Create towers.
            // Total Cost: 10000
            mock.Setup(api => api.GetCash(0)).Returns(totalCash);
            mock.Setup(api => api.GetTowers(0)).Returns(() => new List<ISalariedTower>()
            {
                CreateMock<ISalariedTower>(tower => tower.Setup(x => x.GetTotalCost()).Returns(1000.0f)),
                CreateMock<ISalariedTower>(tower => tower.Setup(x => x.GetTotalCost()).Returns(2000.0f)),
                CreateMock<ISalariedTower>(tower => tower.Setup(x => x.GetTotalCost()).Returns(3000.0f)),
                CreateMock<ISalariedTower>(tower => tower.Setup(x => x.GetTotalCost()).Returns(4000.0f)),
            });
        });
        
        // Act & Assert
        var towerManager = new TowerManager(fakeBloonsApi, new ModSettings() { CostPercentPerRound = costPercentPerRound });
        Assert.Equal(expectedResult, towerManager.GetTotalSalary(0));
        Assert.Equal(availableSalary, towerManager.GetAvailableSalary(0, out _));
    }

    [Theory]
    [InlineData(50, 1)]
    [InlineData(150, 2)]
    [InlineData(300, 3)]
    [InlineData(500, 4)]
    public void SellTowers(float salaryRequired, int expectedTowersSold)
    {
        const float costPercentPerRound = 0.05f;

        // Arrange
        var fakeBloonsApi = CreateDefaultTestApi();
        var initialTowerCount = fakeBloonsApi.Towers.Count;

        // Act
        var towerManager = new TowerManager(fakeBloonsApi, new ModSettings() { CostPercentPerRound = costPercentPerRound });
        towerManager.SellTowers(0, salaryRequired);

        // Assert
        Assert.Equal(expectedTowersSold, fakeBloonsApi.TowersSold);
        Assert.Equal(expectedTowersSold, initialTowerCount - fakeBloonsApi.Towers.Count); // Tests the mock itself.
    }
}