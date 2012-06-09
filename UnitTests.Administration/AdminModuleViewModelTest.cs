using System;
using System.ComponentModel;
using System.Windows.Data;
using HealthTracker.Administration.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using Microsoft.Practices.Prism.Logging;

namespace UnitTests.Administration
{
   [TestClass]
   public class AdminModuleViewModelTest
   {
      #region Contructor
      public AdminModuleViewModelTest()
      { }
      #endregion

      #region Contructor Tests
      [TestMethod]
      public void AdminModuleViewModelDefault()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         AdminModuleViewModel adminModuleViewModel = new AdminModuleViewModel( dataRepository, null, null, loggerMock.Object );

         Assert.AreEqual( 4, adminModuleViewModel.AdminNodes.Count );
      }
      #endregion

      #region Command Tests
      [TestMethod]
      public void AdminModuleViewModelShowFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.FoodGroupView, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();
         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.FoodGroupView + "?ID=" + FullTestData.MeatID.ToString(), UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         var regionManagerMock = new Mock<IRegionManager>();
         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.AdministrationTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.AdministrationTabsRegion] ).Returns( regionMock.Object );

         AdminModuleViewModel adminModuleViewModel =
            new AdminModuleViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         // Create a totally new FoodGroup workspace
         Assert.IsTrue( adminModuleViewModel.ShowFoodGroupCommand.CanExecute( null ) );
         adminModuleViewModel.ShowFoodGroupCommand.Execute( null );

         // Load a FoodGroups from the repository
         Assert.IsTrue( adminModuleViewModel.ShowFoodGroupCommand.CanExecute( FullTestData.MeatID ) );
         adminModuleViewModel.ShowFoodGroupCommand.Execute( FullTestData.MeatID );

         regionMock.VerifyAll();
      }

      [TestMethod]
      public void AdminModuleViewModelShowMealType()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealTypeView, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealTypeView + "?ID=" + FullTestData.LunchID.ToString(), UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         var regionManagerMock = new Mock<IRegionManager>();
         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.AdministrationTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.AdministrationTabsRegion] ).Returns( regionMock.Object );

         AdminModuleViewModel adminModuleViewModel =
            new AdminModuleViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsTrue( adminModuleViewModel.ShowMealTypeCommand.CanExecute( null ) );
         adminModuleViewModel.ShowMealTypeCommand.Execute( null );

         Assert.IsTrue( adminModuleViewModel.ShowMealTypeCommand.CanExecute( FullTestData.LunchID ) );
         adminModuleViewModel.ShowMealTypeCommand.Execute( FullTestData.LunchID );

         regionMock.VerifyAll();
      }

      [TestMethod]
      public void AdminModuleViewModelShowMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealTemplateView, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.MealTemplateView + "?ID=" + FullTestData.CheeseBurgerLunchID, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         var regionManagerMock = new Mock<IRegionManager>();
         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.AdministrationTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.AdministrationTabsRegion] ).Returns( regionMock.Object );

         AdminModuleViewModel viewModel =
            new AdminModuleViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsTrue( viewModel.ShowMealTemplateCommand.CanExecute( null ) );
         viewModel.ShowMealTemplateCommand.Execute( null );

         Assert.IsTrue( viewModel.ShowMealTemplateCommand.CanExecute( FullTestData.CheeseBurgerLunchID ) );
         viewModel.ShowMealTemplateCommand.Execute( FullTestData.CheeseBurgerLunchID );

         regionMock.VerifyAll();
      }

      [TestMethod]
      public void AdminModuleViewModelShowFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.FoodItemView, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         regionMock
            .Setup( x => x.RequestNavigate( new Uri( ViewNames.FoodItemView + "?ID=" + FullTestData.CarrotID, UriKind.Relative ), It.IsAny<Action<NavigationResult>>() ) )
            .Verifiable();

         var regionManagerMock = new Mock<IRegionManager>();
         regionManagerMock.Setup( x => x.Regions.ContainsRegionWithName( RegionNames.AdministrationTabsRegion ) ).Returns( true );
         regionManagerMock.Setup( x => x.Regions[RegionNames.AdministrationTabsRegion] ).Returns( regionMock.Object );

         AdminModuleViewModel viewModel =
            new AdminModuleViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsTrue( viewModel.ShowFoodItemCommand.CanExecute( null ) );
         viewModel.ShowFoodItemCommand.Execute( null );

         Assert.IsTrue( viewModel.ShowFoodItemCommand.CanExecute( FullTestData.CarrotID ) );
         viewModel.ShowFoodItemCommand.Execute( FullTestData.CarrotID );

         regionMock.VerifyAll();
      }
      #endregion
   }
}
