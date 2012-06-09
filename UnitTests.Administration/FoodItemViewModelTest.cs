using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
   public class FoodItemViewModelTest
   {
      #region Constructors
      public FoodItemViewModelTest()
      { }
      #endregion

      #region Contructor Tests
      [TestMethod]
      public void FoodItemViewModelDefault()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         FoodItemViewModel foodItemViewModel = new FoodItemViewModel( dataRepository, null, null, loggerMock.Object );

         Assert.AreEqual( DisplayStrings.NewFoodItemTitle, foodItemViewModel.Title );
      }
      #endregion

      #region Public Interface Tests
      [TestMethod]
      public void FoodItemViewModelIsDirty()
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

         // Existing Food Item
         //   Starts clean
         //   Change Name - dirty
         //   Undo - clean
         //   Change Description - dirty
         //   Undo - clean
         //   Change Calories - dirty
         //   Undo - clean
         //   Change Food Group Serving - dirty
         //   Undo - clean
         //   Add food group - dirty
         //   undo - clean
         //   
         FoodItem cheeseBurger = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         Assert.IsNotNull( cheeseBurger );
         String originalName = cheeseBurger.Name;
         String originalDescription = cheeseBurger.Description;

         FoodItemViewModel cheeseBurgerViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + cheeseBurger.ID.ToString(), UriKind.Relative ) );
         cheeseBurgerViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Name += "Modified";
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Name = originalName;
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Description += "Different";
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Description = originalDescription;
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Name = null;
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.Name = originalName;
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.CaloriesPerServing += 90;
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.CaloriesPerServing -= 90;
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.FoodGroupsPerServing[0].Quantity += 1;
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.FoodGroupsPerServing[0].Quantity -= 1;
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         ServingViewModel<FoodGroup> serving = cheeseBurgerViewModel.FoodGroupsPerServing[0];
         cheeseBurgerViewModel.FoodGroupsPerServing.Remove( serving );
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         // Create an exact duplicate of the servings.
         ServingViewModel<FoodGroup> copyOfServing = new ServingViewModel<FoodGroup>( serving.Entity, serving.Quantity );

         cheeseBurgerViewModel.FoodGroupsPerServing.Add( copyOfServing );
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );

         serving = new ServingViewModel<FoodGroup>(
            dataRepository.FindFoodGroup( fg => fg.Name == "Water" ), 1 );
         cheeseBurgerViewModel.FoodGroupsPerServing.Add( serving );
         Assert.IsTrue( cheeseBurgerViewModel.IsDirty );

         cheeseBurgerViewModel.FoodGroupsPerServing.Remove( serving );
         Assert.IsFalse( cheeseBurgerViewModel.IsDirty );
      }

      [TestMethod]
      public void FoodItemViewModelIsValid()
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

         FoodItemViewModel testFoodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodItemViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, testFoodItemViewModel["Name"] );
         Assert.AreEqual( Messages.Error_No_FoodGroups, testFoodItemViewModel["FoodGroupsPerServing"] );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["Description"] ) );

         testFoodItemViewModel.Name = "Fruit Salad";
         testFoodItemViewModel.FoodGroupsPerServing.Add(
            new ServingViewModel<FoodGroup>(
               dataRepository.FindFoodGroup( fg => fg.Name == "Fruit" ), 1 ) );
         Assert.IsFalse( testFoodItemViewModel.IsValid );
         Assert.AreEqual( Messages.Error_FoodItem_Exists, testFoodItemViewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["FoodGroupsPerServing"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["Description"] ) );

         testFoodItemViewModel.Name = "Some Unique Name";
         Assert.IsTrue( testFoodItemViewModel.IsValid );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel.Error ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["Name"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["FoodGroupsPerServing"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodItemViewModel["Description"] ) );
      }

      [TestMethod]
      public void NewFoodItemViewModelIsNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
           .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodItem>() ) ).Returns( false );

         var testFoodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsTrue( testFoodItemViewModel.IsNew );
      }

      [TestMethod]
      public void ExistingFoodItemViewModelIsNotNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "", 420 );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
           .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );

         var testFoodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodItemViewModel.IsNew );
      }

      [TestMethod]
      public void UsedFoodItemIsUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "", 420 );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
           .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodItem ) ).Returns( true );

         var testFoodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsTrue( testFoodItemViewModel.IsUsed );
      }

      [TestMethod]
      public void UnusedFoodItemIsNotUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "", 420 );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
           .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodItem ) ).Returns( false );

         var testFoodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodItemViewModel.IsUsed );
      }

      [TestMethod]
      public void NewFoodItemIsNotUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
           .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodItem>() ) ).Returns( false );

         var testFoodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView", UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodItemViewModel.IsUsed );
      }

      [TestMethod]
      public void FoodItemViewModelName()
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
         FoodItem testFoodItem = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         FoodItemViewModel testFoodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         // Tests:
         //   o Name is the same as the object from the repository
         //   o Changing the name causes appropriate properties to be marked as changed
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + testFoodItem.ID.ToString(), UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         testFoodItemViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( testFoodItem.Name, testFoodItemViewModel.Name );

         testFoodItemViewModel.Name = "Some other name";
         Assert.AreEqual( "Some other name", testFoodItemViewModel.Name );

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void FoodItemViewModelDescription()
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
         FoodItem testFoodItem = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         FoodItemViewModel testFoodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         // Tests:
         //   o Description is the same as the object from the repository
         //   o Changing the description causes appropriate properties to be marked as changed
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + testFoodItem.ID.ToString(), UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         testFoodItemViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( testFoodItem.Description, testFoodItemViewModel.Description );

         testFoodItemViewModel.Description = "Some other description";
         Assert.AreEqual( "Some other description", testFoodItemViewModel.Description );

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Description" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
      }

      [TestMethod]
      public void FoodItemViewModelID()
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

         FoodItemViewModel testFoodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + FullTestData.FruitSaladID, UriKind.Relative ) );
         testFoodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.FruitSaladID, testFoodItemViewModel.ID );
      }

      [TestMethod]
      public void FoodItemViewModelTitle()
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

         FoodItemViewModel foodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         Assert.AreEqual( DisplayStrings.NewFoodItemTitle, foodItemViewModel.Title );

         // Need to navigate to a new food group in order to makes changes and save them.
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );

         Assert.AreEqual( DisplayStrings.NewFoodItemTitle, foodItemViewModel.Title );
         foodItemViewModel.Name = "Test the Title";
         foodItemViewModel.Description = "This is just to test the title";
         foodItemViewModel.CaloriesPerServing = 90;
         foodItemViewModel.FoodGroupsPerServing.Add( new ServingViewModel<FoodGroup>( dataRepository.GetAllFoodGroups()[0], 1 ) );
         Assert.AreEqual( DisplayStrings.NewFoodItemTitle, foodItemViewModel.Title );
         Assert.IsTrue( foodItemViewModel.SaveCommand.CanExecute( null ) );
         foodItemViewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "Test the Title", foodItemViewModel.Title );
      }

      [TestMethod]
      public void FoodItemViewModelValidFoodGroups()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         FoodItemViewModel foodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.IsNotNull( foodItemViewModel.ValidFoodGroups );
         Assert.AreEqual( dataRepository.GetAllFoodGroups().Count, foodItemViewModel.ValidFoodGroups.Items.Count );
      }

      [TestMethod]
      public void FoodItemViewModelCaloriesPerServing()
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

         // Construct a known food group mealType and make sure the food group servings makes sense.
         FoodItem foodItem = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         FoodItemViewModel foodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( foodItem.CaloriesPerServing, foodItemViewModel.CaloriesPerServing );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );

         // Change the calories
         foodItemViewModel.CaloriesPerServing = 700;
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "CaloriesPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         foodItemViewModel.CaloriesPerServing = 650;
         Assert.IsFalse( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );
         propertyChangedHandler.Reset();

         // Make the calories invalid
         foodItemViewModel.CaloriesPerServing = -700;
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsValid );
         Assert.AreEqual( Messages.Error_Negative_Calories, foodItemViewModel.Error );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "CaloriesPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
      }

      [TestMethod]
      public void FoodItemViewModelFoodGroupServings()
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

         // Navigate to a known food group mealType and make sure the food group servings makes sense.
         // NOTE: No need to get too fancy, the model tests makes sure the data loaded properly.
         FoodItem foodItem = dataRepository.GetFoodItem( FullTestData.CheeseBurgerID );
         FoodItemViewModel foodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( foodItem.FoodGroupsPerServing.Count, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );

         // Add a food group
         ServingViewModel<FoodGroup> testFoodServings = new ServingViewModel<FoodGroup>(
            dataRepository.GetFoodGroup( FullTestData.JunkFoodID ), 1 );
         foodItemViewModel.FoodGroupsPerServing.Add( testFoodServings );

         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupsPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         foodItemViewModel.FoodGroupsPerServing.Remove( testFoodServings );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         propertyChangedHandler.Reset();

         // Delete a food group
         testFoodServings = foodItemViewModel.FoodGroupsPerServing.ElementAt( 0 );
         foodItemViewModel.FoodGroupsPerServing.Remove( testFoodServings );
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupsPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         foodItemViewModel.FoodGroupsPerServing.Add( testFoodServings );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         propertyChangedHandler.Reset();

         // Modify a food group
         foodItemViewModel.FoodGroupsPerServing[0].Quantity += 1;
         Assert.AreEqual( foodItem.FoodGroupsPerServing.Count, foodItemViewModel.FoodGroupsPerServing.Count );
         Serving<FoodGroup> modelServing = foodItem.FoodGroupsPerServing.Find( f => f.Entity.ID == foodItemViewModel.FoodGroupsPerServing[0].Entity.ID );
         Assert.IsNotNull( modelServing );
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupsPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         foodItemViewModel.FoodGroupsPerServing[0].Quantity -= 1;
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         propertyChangedHandler.Reset();

         // Make a serving invalid.
         foodItemViewModel.FoodGroupsPerServing[0].Quantity *= -1;
         Assert.AreEqual( foodItem.FoodGroupsPerServing.Count, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Quantity, foodItemViewModel.Error );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupsPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );

         foodItemViewModel.FoodGroupsPerServing[0].Quantity *= -1;
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         propertyChangedHandler.Reset();

         // Remove all food groups
         foodItemViewModel.FoodGroupsPerServing.Clear();
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_FoodGroups, foodItemViewModel.Error );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "FoodGroupsPerServing" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
      }
      #endregion

      #region Command Tests
      [TestMethod]
      public void SaveCalledForValidNewFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodItem>() ) ).Returns( false );

         var foodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.Name = "New Food Item";
         foodItemViewModel.CaloriesPerServing = 420;
         foodItemViewModel.FoodGroupsPerServing.Add(
            new ServingViewModel<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test Group", "" ), 1 ) );
         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( foodItemViewModel.IsNew );

         foodItemViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidNewFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodItem>() ) ).Returns( false );

         var foodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.Name = "New Food Item";
         foodItemViewModel.CaloriesPerServing = 420;
         Assert.IsFalse( foodItemViewModel.IsValid );
         Assert.IsTrue( foodItemViewModel.IsNew );

         foodItemViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveCalledForValidChangedFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "FoodItem", "", 420 );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test Group", "" ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );

         var foodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.Description = "Something New";

         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsNew );

         foodItemViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidChangedFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "FoodItem", "", 420 );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );

         var foodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         foodItemViewModel.Description = "Something New";

         Assert.IsFalse( foodItemViewModel.IsValid );
         Assert.IsTrue( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsNew );

         foodItemViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveNotCalledForNonChangedFoodItem()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodItem = new FoodItem( Guid.NewGuid(), "FoodItem", "", 420 );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "Test Group", "" ), 1 ) );

         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );

         var foodItemViewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );

         Assert.IsTrue( foodItemViewModel.IsValid );
         Assert.IsFalse( foodItemViewModel.IsDirty );
         Assert.IsFalse( foodItemViewModel.IsNew );

         foodItemViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Never() );
      }

      [TestMethod]
      public void CannotDeleteFoodItemIfNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodItem>() ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( It.IsAny<FoodItem>() ) ).Returns( false );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );

         var viewModel = new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItem", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         viewModel.Name = "test";
         viewModel.Description = "This is a test";
         viewModel.FoodGroupsPerServing.Add( new ServingViewModel<FoodGroup>( new FoodGroup( Guid.NewGuid(), "test", "test" ), 1.5M ) );

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
      public void CannotDeleteFoodItemIfUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test", "This is a test", 90.5M );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "test", "test" ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );

         var viewModel = new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItem?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
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

         Assert.IsFalse( viewModel.DeleteCommand.CanExecute( null ) );
         dataRepositoryMock.VerifyAll();
         interactionServiceMock.Verify(
            x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }

      [TestMethod]
      public void FoodItemIsDeletedIfAnswerIsYes()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test", "This is a test", 90.5M );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "test", "test" ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodItem ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );

         var viewModel = new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
                     new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItem?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
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
               Messages.Question_FoodItem_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.Yes );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( foodItem ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Exactly( 1 ) );
      }

      [TestMethod]
      public void FoodItemIsNotDeletedIfAnswerIsNo()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();

         var foodItem = new FoodItem( Guid.NewGuid(), "Test", "This is a test", 90.5M );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( new FoodGroup( Guid.NewGuid(), "test", "test" ), 1.5M ) );

         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodItem ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock
            .Setup( x => x.GetAllFoodGroups() )
            .Returns( new ReadOnlyCollection<FoodGroup>( new List<FoodGroup>() ) );

         var viewModel = new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
                     new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItem?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
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
               Messages.Question_FoodItem_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.No );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( foodItem ), Times.Never() );
         regionMock.Verify( x => x.Remove( view ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }


      [TestMethod]
      public void FoodItemViewModelUndoRedo()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         Mock<IRegionNavigationService> regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         Mock<IRegionManager> regionManagerMock = new Mock<IRegionManager>();
         Mock<IInteractionService> interactionServiceMock = new Mock<IInteractionService>();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         // TODO: Update this test when the undo and redo is expanded.
         // TODO: Update this test to include Calories (can be done before expansion)
         // Should not be able to undo or redo on a new object.
         ServingViewModel<FoodGroup> waterServing =
            new ServingViewModel<FoodGroup>( dataRepository.GetFoodGroup( FullTestData.WaterID ), 1.0M );
         ServingViewModel<FoodGroup> meatServing =
            new ServingViewModel<FoodGroup>( dataRepository.GetFoodGroup( FullTestData.MeatID ), 1.5M );
         ServingViewModel<FoodGroup> fruitServing =
            new ServingViewModel<FoodGroup>( dataRepository.GetFoodGroup( FullTestData.FruitID ), 2.5M );
         FoodItemViewModel foodItemViewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView", UriKind.Relative ) );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Assign values to properties, save, and the load into new view model to reset the undo/redo log
         foodItemViewModel.Name = "Bob";
         foodItemViewModel.Description = "Battery Operated Buddy";
         foodItemViewModel.CaloriesPerServing = 42.0M;
         foodItemViewModel.FoodGroupsPerServing.Add( waterServing );
         navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + foodItemViewModel.ID.ToString(), UriKind.Relative ) );
         foodItemViewModel.SaveCommand.Execute( null );
         foodItemViewModel = new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         foodItemViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Make changes as such:
         //   o name changed from Bob to Pete
         //   o name changed from Pete to Peter
         //   o Description changed from "Battery Operated Buddy" to "The Rock"
         //   o Add Meat food group serving (1.5)
         //   o Change Calroies to 69
         //   o Add Fruit food group serving (2.5)
         //   o Change Meat servings to 3
         //   o Remove the Fruit food serving
         //   o name changed from Peter to Simon
         //   o name changed from Simon to Saul
         //   o description changed from "The Rock" to "The Persecutor"
         //   o description changed from "The Persecutor" to "The Apostle"
         //   o name changed from Saul to Paul
         // Verify can undo, cannot redo at each step
         foodItemViewModel.Name = "Pete";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Name += "r";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Description = "The Rock";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.FoodGroupsPerServing.Add( meatServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.CaloriesPerServing = 69;
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.FoodGroupsPerServing.Add( fruitServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         meatServing.Quantity = 3;
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.FoodGroupsPerServing.Remove( fruitServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Name = "Simon";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Name = "Saul";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Description = "The Persecutor";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Description = "The Apostle";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.Name = "Paul";
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );

         Assert.AreEqual( "Paul", foodItemViewModel.Name );
         Assert.AreEqual( "The Apostle", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );

         // Undo once.  Verify last thing done is undone, and we can redo.
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Apostle", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Redo.  Verify last thing undone is redone, can no longer redo, can still undo.
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Paul", foodItemViewModel.Name );
         Assert.AreEqual( "The Apostle", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Undo 4 times, verify undo worked
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Apostle", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Persecutor", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Redo 2 times, verify
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Persecutor", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Undo until back to original, cannot undo, can redo
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 3, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 3, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 3, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 3, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( fruitServing ) );
         Assert.AreEqual( 3, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 3, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( fruitServing ) );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 69, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Pete", foodItemViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Bob", foodItemViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 1.5M, meatServing.Quantity );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsFalse( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );

         // Redo 4 times, verify
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Pete", foodItemViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 1, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
         foodItemViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodItemViewModel.Name );
         Assert.AreEqual( "The Rock", foodItemViewModel.Description );
         Assert.AreEqual( 2, foodItemViewModel.FoodGroupsPerServing.Count );
         Assert.IsTrue( foodItemViewModel.FoodGroupsPerServing.Contains( meatServing ) );
         Assert.AreEqual( 42, foodItemViewModel.CaloriesPerServing );
         Assert.IsTrue( foodItemViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodItemViewModel.RedoCommand.CanExecute( null ) );
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

         // Set up the food group list in the data repository mock.
         FoodGroup testFoodGroup = new FoodGroup( Guid.NewGuid(), "test", "The only food group in the mock data repository" );
         List<FoodGroup> foodGroups = new List<FoodGroup>();
         foodGroups.Add( testFoodGroup );
         dataRepositoryMock.Setup( x => x.GetAllFoodGroups() ).Returns( new ReadOnlyCollection<FoodGroup>( foodGroups ) );
         FoodItem foodItem = new FoodItem( Guid.NewGuid(), "Test Food Item", "Test Food Item Description", 42.0M );
         foodItem.FoodGroupsPerServing.Add( new Serving<FoodGroup>( testFoodGroup, 5.0M ) );
         Assert.IsTrue( foodItem.IsValid );
         dataRepositoryMock.Setup( x => x.GetFoodItem( foodItem.ID ) ).Returns( foodItem );
         dataRepositoryMock.Setup( x => x.Contains( foodItem ) ).Returns( true );

         // Create the view model under test and associate it with a view
         FoodItemViewModel viewModel =
            new FoodItemViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
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
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemView?ID=" + foodItem.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         if (makeDirty)
         {
            if (makeInvalid)
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_FoodItem_Close, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsFalse( viewModel.IsValid );
            }
            else
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_FoodItem_Save, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ) )
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
            dataRepositoryMock.Verify( x => x.SaveItem( foodItem ), Times.Exactly( 1 ) );
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Exactly( 1 ) );
         }
         else
         {
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodItem>() ), Times.Never() );
         }
      }

      [TestMethod]
      public void FoodItemViewModelCloseDirtyAnswerCancel()
      {
         RunCloseTest( true, false, MessageBoxResult.Cancel, false, false );
      }

      [TestMethod]
      public void FoodItemViewModelCloseDirtyAnswerNo()
      {
         RunCloseTest( true, false, MessageBoxResult.No, true, false );
      }

      [TestMethod]
      public void FoodItemViewModelCloseDirtyAnswerYes()
      {
         RunCloseTest( true, false, MessageBoxResult.Yes, true, true );
      }

      [TestMethod]
      public void FoodItemViewModelCloseInvalidAnswerNo()
      {
         RunCloseTest( true, true, MessageBoxResult.No, false, false );
      }

      [TestMethod]
      public void FoodItemViewModelCloseInvalidAnswerYes()
      {
         RunCloseTest( true, true, MessageBoxResult.Yes, true, false );
      }

      [TestMethod]
      public void FoodItemViewModelCloseRemovesCleanView()
      {
         RunCloseTest( false, false, MessageBoxResult.None, true, false );
      }
      #endregion

      #region Other Tests
      [TestMethod]
      public void NavigateToFoodItem()
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

         FoodItemViewModel viewModel =
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "Something?ID=" + FullTestData.CheeseBurgerID, UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.CheeseBurgerID, viewModel.ID );
         Assert.AreEqual( "Deluxe Bacon Cheese Burger", viewModel.Name );
         Assert.AreEqual(
            "Ground up cow, topped with curdled milk and salted pig fat.  Add lettuce, tomato, and onion for health.", viewModel.Description );
         Assert.AreEqual( 4, viewModel.FoodGroupsPerServing.Count );
         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void NavigateToNewFoodItem()
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
            new FoodItemViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodItemViewModel", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         foreach (var foodItem in dataRepository.GetAllFoodItems())
         {
            Assert.AreNotEqual( foodItem.ID, viewModel.ID );
         }
         Assert.IsNull( viewModel.Name );
         Assert.IsNull( viewModel.Description );
         Assert.AreEqual( 0, viewModel.FoodGroupsPerServing.Count );
         Assert.IsFalse( viewModel.IsDirty );
      }
      #endregion
   }
}
