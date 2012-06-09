using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HealthTracker.Administration.ViewModels;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using Microsoft.Practices.Prism.Logging;
using HealthTracker.DataRepository.ViewModels;

namespace UnitTests.Administration
{
   [TestClass]
   public class MealTemplateViewModelTest
   {
      #region Constructors
      public MealTemplateViewModelTest()
      { }
      #endregion

      #region Contructor Tests
      [TestMethod]
      public void MealTemplateViewModelDefault()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateViewModel mealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.AreEqual( DisplayStrings.NewMealTemplateTitle, mealTemplateViewModel.Title );
      }
      #endregion

      #region Public Interface Tests
      // NOTE: The following are all tested in the MealViewModelTest as the code is shared
      //   o TypeOfMeal
      //   o TimeOfMeal
      //   o FoodItemServings
      //   o FoodGroupServings
      //   o Calories
      //   o ValidFoodItems
      //   o ValidMealTypes

      [TestMethod]
      public void MealTemplateViewModelIsDirty()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         // Existing Meal Template
         //   Starts clean
         //   Change Name - dirty
         //   Undo - clean
         //   Change Description - dirty
         //   Undo - clean
         MealTemplate cheeseBurgerMealTemplate = dataRepository.GetMealTemplate( FullTestData.CheeseBurgerLunchID );
         Assert.IsNotNull( cheeseBurgerMealTemplate );
         String originalName = cheeseBurgerMealTemplate.Name;
         String originalDescription = cheeseBurgerMealTemplate.Description;

         MealTemplateViewModel cheeseBurgerMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + cheeseBurgerMealTemplate.ID.ToString(), UriKind.Relative ) );
         cheeseBurgerMealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Name += "Modified";
         Assert.IsTrue( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Name = originalName;
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Description += "Different";
         Assert.IsTrue( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Description = originalDescription;
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Name = null;
         Assert.IsTrue( cheeseBurgerMealTemplateViewModel.IsDirty );

         cheeseBurgerMealTemplateViewModel.Name = originalName;
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );

         // Test a couple of the easier to test items from the base class to ensure the base
         // class' IsDirty is being called.
         DateTime originalTimeOfMeal = cheeseBurgerMealTemplateViewModel.TimeOfMeal;
         cheeseBurgerMealTemplateViewModel.TimeOfMeal = DateTime.Now;
         Assert.IsTrue( cheeseBurgerMealTemplateViewModel.IsDirty );
         cheeseBurgerMealTemplateViewModel.TimeOfMeal = originalTimeOfMeal;
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );

         MealType originalMealType = cheeseBurgerMealTemplateViewModel.TypeOfMeal;
         cheeseBurgerMealTemplateViewModel.TypeOfMeal = null;
         Assert.IsTrue( cheeseBurgerMealTemplateViewModel.IsDirty );
         cheeseBurgerMealTemplateViewModel.TypeOfMeal = originalMealType;
         Assert.IsFalse( cheeseBurgerMealTemplateViewModel.IsDirty );
      }

      [TestMethod]
      public void MealTemplateViewModelIsValid()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateViewModel testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testMealTemplateViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, testMealTemplateViewModel["Name"] );
         Assert.IsNull( testMealTemplateViewModel["TimeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_MealType, testMealTemplateViewModel["TypeOfMeal"] );
         Assert.AreEqual( Messages.Error_No_FoodItems, testMealTemplateViewModel["FoodItemServings"] );
         Assert.IsNull( testMealTemplateViewModel["Description"] );

         testMealTemplateViewModel.Name = "Cheeseburger Lunch";
         testMealTemplateViewModel.TimeOfMeal = DateTime.Now;
         testMealTemplateViewModel.TypeOfMeal =
            dataRepository.FindMealType( mt => mt.Name == "Lunch" );
         testMealTemplateViewModel.FoodItemServings.Add(
            new ServingViewModel<FoodItem>(
               dataRepository.FindFoodItem( f => f.Name == "Fruit Salad" ), 1 ) );
         Assert.IsFalse( testMealTemplateViewModel.IsValid );
         Assert.AreEqual( Messages.Error_MealTemplate_Exists, testMealTemplateViewModel.Error );
         Assert.AreEqual( Messages.Error_MealTemplate_Exists, testMealTemplateViewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["TimeOfMeal"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["TypeOfMeal"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["FoodItemServings"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["Description"] ) );

         testMealTemplateViewModel.Name = "Some Unique Name";
         Assert.IsTrue( testMealTemplateViewModel.IsValid );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel.Error ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["Name"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["TimeOfMeal"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["TypeOfMeal"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["FoodItemServings"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTemplateViewModelIsNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         // Two conditions to test
         // Navigating to non-existent template is new
         // Navigating to existing template is not new
         MealTemplateViewModel testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.IsTrue( testMealTemplateViewModel.IsNew );

         navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + FullTestData.CheeseBurgerLunchID.ToString(), UriKind.Relative ) );
         testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testMealTemplateViewModel.IsNew );
      }

      [TestMethod]
      public void MealTemplateViewModelIsUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateViewModel mealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext;

         // Meal templates in the repository cannot be used.
         foreach (MealTemplate mealTemplate in dataRepository.GetAllMealTemplates())
         {
            navigationContext = new NavigationContext(
               regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
            Assert.IsFalse( mealTemplateViewModel.IsUsed );
         }

         navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( mealTemplateViewModel.IsUsed );
      }

      [TestMethod]
      public void MealTemplateViewModelName()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         MealTemplate mealTemplate = dataRepository.GetMealTemplate( FullTestData.CheeseBurgerLunchID );
         MealTemplateViewModel testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         testMealTemplateViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( mealTemplate.Name, testMealTemplateViewModel.Name );
         Assert.IsFalse( testMealTemplateViewModel.IsDirty );
         Assert.IsTrue( testMealTemplateViewModel.IsValid );

         testMealTemplateViewModel.Name = "Some other name";
         Assert.AreEqual( "Some other name", testMealTemplateViewModel.Name );
         Assert.IsTrue( testMealTemplateViewModel.IsDirty );
         Assert.IsTrue( testMealTemplateViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         testMealTemplateViewModel.Name = mealTemplate.Name;
         Assert.IsFalse( testMealTemplateViewModel.IsDirty );
         propertyChangedHandler.Reset();

         // Make the name invalid, show that the error is set properly
         testMealTemplateViewModel.Name = "";
         Assert.IsTrue( String.IsNullOrEmpty( testMealTemplateViewModel.Name ) );
         Assert.IsTrue( testMealTemplateViewModel.IsDirty );
         Assert.IsFalse( testMealTemplateViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, testMealTemplateViewModel.Error );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
      }

      [TestMethod]
      public void MealTemplateViewModelDescription()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         MealTemplate testMealTemplate = dataRepository.GetMealTemplate( FullTestData.CheeseBurgerLunchID );
         MealTemplateViewModel testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTempalte?ID=" + testMealTemplate.ID.ToString(), UriKind.Relative ) );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         testMealTemplateViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( testMealTemplate.Description, testMealTemplateViewModel.Description );
         Assert.IsFalse( testMealTemplateViewModel.IsDirty );
         Assert.IsTrue( testMealTemplateViewModel.IsValid );

         testMealTemplateViewModel.Description = "Some other description";
         Assert.AreEqual( "Some other description", testMealTemplateViewModel.Description );
         Assert.IsTrue( testMealTemplateViewModel.IsDirty );
         Assert.IsTrue( testMealTemplateViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Description" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
      }

      [TestMethod]
      public void MealTemplateViewModelID()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateViewModel testMealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + FullTestData.CheeseBurgerLunchID.ToString(), UriKind.Relative ) );
         testMealTemplateViewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.CheeseBurgerLunchID, testMealTemplateViewModel.ID );
      }

      [TestMethod]
      public void MealTemplateViewModelTitle()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         // NOTE: The title being assigned properly out of construction is actually tested via the constructor tests.
         //       This test just ensure the title changes after a save, but not before.
         MealTemplateViewModel mealTemplateViewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );

         Assert.AreEqual( DisplayStrings.NewMealTemplateTitle, mealTemplateViewModel.Title );
         mealTemplateViewModel.Name = "Test the Title";
         mealTemplateViewModel.Description = "This is just to test the title";
         mealTemplateViewModel.TimeOfMeal = DateTime.Now;
         mealTemplateViewModel.TypeOfMeal = dataRepository.GetAllMealTypes()[0];
         mealTemplateViewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( dataRepository.GetAllFoodItems()[0], 1 ) );
         Assert.AreEqual( DisplayStrings.NewMealTemplateTitle, mealTemplateViewModel.Title );
         Assert.IsTrue( mealTemplateViewModel.SaveCommand.CanExecute( null ) );
         mealTemplateViewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "Test the Title", mealTemplateViewModel.Title );
      }
      #endregion

      #region Command Tests
      // NOTE: The Undo and Redo are tested in the MealViewModelTest as they are primarily base class tests.
      [TestMethod]
      public void SaveCalledForValidNewMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( false );

         var mealTemplateViewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );
         mealTemplateViewModel.Name = "New Meal Template";
         mealTemplateViewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Test Type", "Description", DateTime.Now, false );
         mealTemplateViewModel.TimeOfMeal = DateTime.Now;
         mealTemplateViewModel.FoodItemServings.Add(
            new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );
         Assert.IsTrue( mealTemplateViewModel.IsValid );
         Assert.IsTrue( mealTemplateViewModel.IsNew );

         mealTemplateViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidNewMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( false );

         var mealTemplateViewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );
         mealTemplateViewModel.Name = "New Meal Template";
         mealTemplateViewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "Test Type", "Description", DateTime.Now, false );
         mealTemplateViewModel.TimeOfMeal = DateTime.Now;
         Assert.IsFalse( mealTemplateViewModel.IsValid );
         Assert.IsTrue( mealTemplateViewModel.IsNew );

         mealTemplateViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveCalledForValidChangedMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var mealTemplate = new MealTemplate( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Template", "" );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );

         var mealTemplateViewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );
         mealTemplateViewModel.Description = "Something New";

         Assert.IsTrue( mealTemplateViewModel.IsValid );
         Assert.IsTrue( mealTemplateViewModel.IsDirty );
         Assert.IsFalse( mealTemplateViewModel.IsNew );

         mealTemplateViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidChangedMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var mealTemplate = new MealTemplate( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Template", "" );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );

         var mealTemplateViewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );
         mealTemplateViewModel.Description = "Something New";

         Assert.IsFalse( mealTemplateViewModel.IsValid );
         Assert.IsTrue( mealTemplateViewModel.IsDirty );
         Assert.IsFalse( mealTemplateViewModel.IsNew );

         mealTemplateViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveNotCalledForNonChangedMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var mealTemplate = new MealTemplate( Guid.NewGuid(), new MealType( Guid.NewGuid(), "Meal Type", "", DateTime.Now, false ), DateTime.Now, "Template", "" );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "Test Item", "", 420 ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );

         var mealTemplateViewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         mealTemplateViewModel.OnNavigatedTo( navigationContext );

         Assert.IsTrue( mealTemplateViewModel.IsValid );
         Assert.IsFalse( mealTemplateViewModel.IsDirty );
         Assert.IsFalse( mealTemplateViewModel.IsNew );

         mealTemplateViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Never() );
      }


      [TestMethod]
      public void CannotDeleteMealTemplateIfNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealTemplate>() ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( It.IsAny<MealTemplate>() ) ).Returns( false );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplate", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         viewModel.Name = "test";
         viewModel.Description = "This is a test";
         viewModel.TimeOfMeal = DateTime.Now;
         viewModel.TypeOfMeal = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false );
         viewModel.FoodItemServings.Add( new ServingViewModel<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 90.0M ), 1.25M ) );

         // Setup the regions so we can determine if the view has been removed or not
         var view = new UserControl();
         view.DataContext = viewModel;

         var views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         var regionMock = new Mock<IRegion>();
         var regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         Assert.IsTrue( viewModel.IsNew );
         Assert.IsFalse( viewModel.IsUsed );
         Assert.IsTrue( viewModel.IsValid );
         Assert.IsFalse( viewModel.DeleteCommand.CanExecute( null ) );

         interactionServiceMock.Verify(
            x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }

      [TestMethod]
      public void MealTemplateIsDeletedIfAnswerIsYes()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var mealTemplate = new MealTemplate( Guid.NewGuid(), new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false ), DateTime.Now, "test", "This is a test" );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 100.0M ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( mealTemplate ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealTemplate ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
                     new NavigationContext(
                        regionNavigationServiceMock.Object, new Uri( "MealTempalate?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         // Setup the regions so we can determine if the view has been removed or not
         var view = new UserControl();
         view.DataContext = viewModel;

         var views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         var regionMock = new Mock<IRegion>();
         var regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox(
               Messages.Question_MealTemplate_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.Yes );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( mealTemplate ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Exactly( 1 ) );
      }

      [TestMethod]
      public void MealTemplateIsNotDeletedIfAnswerIsNo()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var mealTemplate = new MealTemplate( Guid.NewGuid(), new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false ), DateTime.Now, "test", "This is a test" );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( new FoodItem( Guid.NewGuid(), "test", "test", 100.0M ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( mealTemplate ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealTemplate ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );
         dataRepositoryMock
            .Setup( x => x.GetAllMealTypes() )
            .Returns( new ReadOnlyCollection<MealType>( new List<MealType>() ) );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodItems() )
            .Returns( new ReadOnlyCollection<FoodItem>( new List<FoodItem>() ) );

         var viewModel = new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext(
               regionNavigationServiceMock.Object, new Uri( "MealTempalate?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         // Setup the regions so we can determine if the view has been removed or not
         var view = new UserControl();
         view.DataContext = viewModel;

         var views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         var regionMock = new Mock<IRegion>();
         var regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox(
               Messages.Question_MealTemplate_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.No );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( mealTemplate ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }

      /// <summary>
      /// Close test template.  All of the close tests follow this based pattern, so they all call this rather than repeating everything
      /// </summary>
      private void RunCloseTest( Boolean makeDirty, Boolean makeInvalid, MessageBoxResult messageResponse, Boolean expectRemove, Boolean expectSave )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         Mock<IDataRepository> dataRepositoryMock = new Mock<IDataRepository>();
         Mock<IRegionNavigationService> regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         Mock<IRegionManager> regionManagerMock = new Mock<IRegionManager>();
         Mock<IRegion> regionWithoutViewMock = new Mock<IRegion>();
         Mock<IRegion> regionMock = new Mock<IRegion>();
         Mock<IInteractionService> interactionServiceMock = new Mock<IInteractionService>();

         // TODO: Will need to build a food group list (done) and a food mealType list, then build the mealTemplate template to test.

         // Set up the food group list in the data repository mock.
         FoodGroup testFoodGroup = new FoodGroup( Guid.NewGuid(), "test", "The only food group in the mock data repository" );
         List<FoodGroup> foodGroups = new List<FoodGroup>();
         foodGroups.Add( testFoodGroup );
         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() ).Returns( new ReadOnlyCollection<FoodGroup>( foodGroups ) );
         FoodItem testFoodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "Test Food Item Description", 42.0M );
         testFoodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( testFoodGroup, 5.0M ) );
         Assert.IsTrue( testFoodItem.IsValid );
         List<FoodItem> foodItems = new List<FoodItem>();
         foodItems.Add( testFoodItem );
         dataRepositoryMock.Setup( x => x.GetAllFoodItems() ).Returns( new ReadOnlyCollection<FoodItem>( foodItems ) );
         MealType testMealType = new MealType( Guid.NewGuid(), "Lunch", "A lunch for testing", DateTime.Now, false );
         Assert.IsTrue( testMealType.IsValid );
         List<MealType> mealTypes = new List<MealType>();
         mealTypes.Add( testMealType );
         dataRepositoryMock.Setup( x => x.GetAllMealTypes() ).Returns( new ReadOnlyCollection<MealType>( mealTypes ) );

         MealTemplate mealTemplate = new MealTemplate( Guid.NewGuid(), testMealType, DateTime.Now, "test mealTemplate", "This is a test" );
         mealTemplate.FoodItemServings.Add( new Serving<FoodItem>( testFoodItem, 1.5M ) );

         dataRepositoryMock.Setup( x => x.GetMealTemplate( mealTemplate.ID ) ).Returns( mealTemplate );
         dataRepositoryMock.Setup( x => x.Contains( mealTemplate ) ).Returns( true );

         // Create the view model under test and associate it with a view
         MealTemplateViewModel viewModel =
            new MealTemplateViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         UserControl view = new UserControl();
         view.DataContext = viewModel;

         // Set up two regions each with their own set of views.
         List<UserControl> views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         List<UserControl> viewsWithoutView = new List<UserControl>();
         viewsWithoutView.Add( new UserControl() );
         viewsWithoutView.Add( new UserControl() );

         List<IRegion> regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionWithoutViewMock.Setup( x => x.Views.GetEnumerator() ).Returns( viewsWithoutView.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         // Navigate to the view that "displays" our food mealType.  This loads the view model
         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView?ID=" + mealTemplate.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         if (makeDirty)
         {
            if (makeInvalid)
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_MealTemplate_Close, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsFalse( viewModel.IsValid );
            }
            else
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_MealTemplate_Save, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "Something Else";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsTrue( viewModel.IsValid );
            }
         }
         else
         {
            // This will fail if we have passed in the non-sensical makeDirty == false, makeInvalid == true
            Assert.AreEqual( makeDirty, viewModel.IsDirty );
         }

         // Attempt a close.
         viewModel.CloseCommand.Execute( null );

         // If we were dirty, then we need to verify that the correct interaction was done, otherwise, that no interaction was done
         if (makeDirty)
         {
            interactionServiceMock.VerifyAll();
         }
         else
         {
            interactionServiceMock.Verify(
               x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ), Times.Never() );
         }

         if (expectRemove)
         {
            regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
            regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Exactly( 1 ) );
         }
         else
         {
            regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Never() );
         }

         if (expectSave)
         {
            dataRepositoryMock.Verify( x => x.SaveItem( mealTemplate ), Times.Exactly( 1 ) );
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Exactly( 1 ) );
         }
         else
         {
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealTemplate>() ), Times.Never() );
         }
      }

      [TestMethod]
      public void MealTemplateViewModelCloseDirtyAnswerCancel()
      {
         RunCloseTest( true, false, MessageBoxResult.Cancel, false, false );
      }

      [TestMethod]
      public void MealTemplateViewModelCloseDirtyAnswerNo()
      {
         RunCloseTest( true, false, MessageBoxResult.No, true, false );
      }

      [TestMethod]
      public void MealTemplateViewModelCloseDirtyAnswerYes()
      {
         RunCloseTest( true, false, MessageBoxResult.Yes, true, true );
      }

      [TestMethod]
      public void MealTemplateViewModelCloseInvalidAnswerNo()
      {
         RunCloseTest( true, true, MessageBoxResult.No, false, false );
      }

      [TestMethod]
      public void MealTemplateViewModelCloseInvalidAnswerYes()
      {
         RunCloseTest( true, true, MessageBoxResult.Yes, true, false );
      }

      [TestMethod]
      public void MealTemplateViewModelCloseRemovesCleanView()
      {
         RunCloseTest( false, false, MessageBoxResult.None, true, false );
      }
      #endregion

      #region Other Tests
      [TestMethod]
      public void NavigateToMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateViewModel viewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object,
               new Uri( "DoesItMatter?ID=" + FullTestData.CheeseBurgerLunchID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.CheeseBurgerLunchID, viewModel.ID );
         Assert.AreEqual( "Cheeseburger Lunch", viewModel.Name );
         Assert.AreEqual( "A typical cheese burger based lunch.", viewModel.Description );
         Assert.AreEqual( 3, viewModel.FoodItemServings.Count );
         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void NavigateToNewMealTemplate()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         var viewModel =
            new MealTemplateViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTemplateView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         foreach (var mealTemplate in dataRepository.GetAllMealTemplates())
         {
            Assert.AreNotEqual( mealTemplate.ID, viewModel.ID );
         }
         Assert.IsNull( viewModel.Name );
         Assert.IsNull( viewModel.Description );
         Assert.AreEqual( 0, viewModel.FoodItemServings.Count );
         Assert.IsFalse( viewModel.IsDirty );
         Assert.AreEqual( DateTime.Now.Date, viewModel.TimeOfMeal );
      }
      #endregion
   }
}
