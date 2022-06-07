using System.Runtime.CompilerServices;
using BloonsTD6.Mod.SalariedMonkeys.Interfaces;
using Moq;
using static BloonsTD6.Mod.SalariedMonkeys.Tests.TestUtilities;

namespace BloonsTD6.Mod.SalariedMonkeys.Tests;

public class SalariedMonkeysTests
{
    [Fact]
    public void SellTowers_WhenException_RestoresSellStatus()
    {
        // Arrange
        var api = CreateDefaultTestApi();
        api.AllowSelling = true; // change default value.

        var fakeTowerManager = CreateMock<ITowerManager>(x =>
        {
            x.Setup(y => y.BloonsApi).Returns(api);
            x.Setup(y => y.SellTowers(0, It.IsAny<float>())).Throws<Exception>(); // Just in case.
        });
        
        var salariedMonkeys = new SalariedMonkeys();
        salariedMonkeys.Construct(fakeTowerManager, default!, default!);
        
        // Act & Assert
        // Test with allowSelling On
        salariedMonkeys.PaySalaries(0);
        salariedMonkeys.SellTowers(0);
        Assert.True(api.AllowSelling);

        // Test with allowSelling Off
        api.AllowSelling = false;
        salariedMonkeys.PaySalaries(0);
        salariedMonkeys.SellTowers(0);
        Assert.False(api.AllowSelling);
    }

    [Fact]
    public void SellTowers_WhenNegativeSalary_SellsTowers()
    {
        // Arrange
        var calledSellFunction = new StrongBox<bool>(false);
        var totalSalary = 0.0;
        var api = CreateDefaultTestApi();
        api.Cash = -100.0f; // Negative Salary

        var towerManagerMock = new Mock<ITowerManager>();
        towerManagerMock.Setup(y => y.BloonsApi).Returns(api);
        towerManagerMock.Setup(y => y.SellTowers(0, It.IsAny<float>())).Callback(() => calledSellFunction.Value = true);

        var fakeTowerManager = towerManagerMock.Object;
        var salariedMonkeys  = new SalariedMonkeys();
        salariedMonkeys.Construct(fakeTowerManager, default!, default!);

        // Act & Assert
        // Test with allowSelling On
        salariedMonkeys.PaySalaries(0);
        salariedMonkeys.SellTowers(0);
        Assert.True(calledSellFunction.Value);
    }
}