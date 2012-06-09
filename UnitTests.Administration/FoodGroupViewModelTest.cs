using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HealthTracker.Administration.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.Infrastructure;
using HealthTracker.Infrastructure.Interfaces;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.Practices.Prism.Regions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;
using Microsoft.Practices.Prism.Logging;

namespace UnitTests.Administration
{
   [TestClass]
   public class FoodGroupViewModelTest
   {
      #region Constructors
      public FoodGroupViewModelTest()
      { }
      #endregion

      #region Contructor Tests
      [TestMethod]
      public void FoodGroupViewModelDefault()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         var dataRepository = new DataRepository( configurationMock.Object );

         FoodGroupViewModel foodGroupViewModel = new FoodGroupViewModel( dataRepository, null, null, loggerMock.Object );

         // This is really the only property that should be set at this point.
         // There is no model set until the view/view model is navigated to.
         Assert.AreEqual( DisplayStrings.NewFoodGroupTitle, foodGroupViewModel.Title );
      }
      #endregion

      #region Public Interface Tests
      [TestMethod]
      public void FoodGroupViewModelIsDirty()
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

         // Existing Food group
         //   Starts clean
         //   Change Name - dirty
         //   Undo - clean
         //   Change Description - dirty
         //   Undo - clean
         FoodGroup vegetableFoodGroup = dataRepository.GetFoodGroup( FullTestData.VegetableID );
         Assert.IsNotNull( vegetableFoodGroup );
         String originalName = vegetableFoodGroup.Name;
         String originalDescription = vegetableFoodGroup.Description;

         FoodGroupViewModel vegetableFoodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "DoesItMatter?ID=" + vegetableFoodGroup.ID.ToString(), UriKind.Relative ) );
         vegetableFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Name += "Modified";
         Assert.IsTrue( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Name = originalName;
         Assert.IsFalse( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Description += "Different";
         Assert.IsTrue( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Description = originalDescription;
         Assert.IsFalse( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Name = null;
         Assert.IsTrue( vegetableFoodGroupViewModel.IsDirty );

         vegetableFoodGroupViewModel.Name = originalName;
         Assert.IsFalse( vegetableFoodGroupViewModel.IsDirty );
      }

      [TestMethod]
      public void FoodGroupViewModelIsValid()
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

         FoodGroupViewModel testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodGroupViewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, testFoodGroupViewModel.Error );
         Assert.AreEqual( Messages.Error_No_Name, testFoodGroupViewModel["Name"] );
         Assert.IsNull( testFoodGroupViewModel["Description"] );

         testFoodGroupViewModel.Name = "Fruit";
         Assert.IsFalse( testFoodGroupViewModel.IsValid );
         Assert.AreEqual( Messages.Error_FoodGroup_Exists, testFoodGroupViewModel.Error );
         Assert.AreEqual( Messages.Error_FoodGroup_Exists, testFoodGroupViewModel["Name"] );
         Assert.IsNull( testFoodGroupViewModel["Description"] );

         testFoodGroupViewModel.Name = "Some Unique Name";
         Assert.IsTrue( testFoodGroupViewModel.IsValid );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodGroupViewModel.Error ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodGroupViewModel["Name"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( testFoodGroupViewModel["Description"] ) );
      }

      [TestMethod]
      public void NewFoodGroupViewModelIsNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );

         var testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsTrue( testFoodGroupViewModel.IsNew );
      }

      [TestMethod]
      public void ExistingFoodGroupViewModelIsNotNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test", "" );

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );

         var testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodGroupViewModel.IsNew );
      }

      [TestMethod]
      public void UsedFoodGroupIsUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test", "" );

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodGroup ) ).Returns( true );

         var testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsTrue( testFoodGroupViewModel.IsUsed );
      }

      [TestMethod]
      public void UnusedFoodGroupIsNotUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test", "" );

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodGroup ) ).Returns( false );

         var testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodGroupViewModel.IsUsed );
      }

      [TestMethod]
      public void NewFoodGroupIsNotUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );

         var testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView", UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( testFoodGroupViewModel.IsUsed );
      }

      [TestMethod]
      public void FoodGroupViewModelName()
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

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();
         FoodGroup testFoodGroup = dataRepository.GetFoodGroup( FullTestData.MeatID );

         // Tests:
         //   o Name is the same as the object from the repository
         //   o Changing the name causes appropriate properties to be marked as changed
         FoodGroupViewModel testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + FullTestData.MeatID, UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         testFoodGroupViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( testFoodGroup.Name, testFoodGroupViewModel.Name );

         testFoodGroupViewModel.Name = "Some other name";
         Assert.AreEqual( "Some other name", testFoodGroupViewModel.Name );

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void FoodGroupViewModelDescription()
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

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         FoodGroup testFoodGroup = dataRepository.GetFoodGroup( FullTestData.MeatID );

         // Tests:
         //   o Name is the same as the object from the repository
         //   o Changing the name causes appropriate properties to be marked as changed

         FoodGroupViewModel testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + testFoodGroup.ID.ToString(), UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );
         testFoodGroupViewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         Assert.AreEqual( testFoodGroup.Description, testFoodGroupViewModel.Description );

         testFoodGroupViewModel.Description = "Some other description";
         Assert.AreEqual( "Some other description", testFoodGroupViewModel.Description );

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Description" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
      }

      [TestMethod]
      public void FoodGroupViewModelID()
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

         FoodGroupViewModel testFoodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + FullTestData.VegetableID.ToString(), UriKind.Relative ) );
         testFoodGroupViewModel.OnNavigatedTo( navigationContext );

         Assert.AreEqual( FullTestData.VegetableID, testFoodGroupViewModel.ID );
      }

      [TestMethod]
      public void FoodGroupViewModelTitle()
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

         FoodGroupViewModel foodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         Assert.AreEqual( DisplayStrings.NewFoodGroupTitle, foodGroupViewModel.Title );

         // Need to navigate to a new food group in order to makes changes and save them.
         NavigationContext navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );

         Assert.AreEqual( DisplayStrings.NewFoodGroupTitle, foodGroupViewModel.Title );
         foodGroupViewModel.Name = "Test the Title";
         foodGroupViewModel.Description = "This is just to test the title";
         Assert.AreEqual( DisplayStrings.NewFoodGroupTitle, foodGroupViewModel.Title );
         Assert.IsTrue( foodGroupViewModel.SaveCommand.CanExecute( null ) );
         foodGroupViewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "Test the Title", foodGroupViewModel.Title );
      }
      #endregion

      #region Command Tests
      [TestMethod]
      public void SaveCalledForValidNewFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );

         var foodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         foodGroupViewModel.Name = "New Food Group";
         Assert.IsTrue( foodGroupViewModel.IsValid );
         Assert.IsTrue( foodGroupViewModel.IsNew );

         foodGroupViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidNewFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );

         var foodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( foodGroupViewModel.IsValid );
         Assert.IsTrue( foodGroupViewModel.IsNew );

         foodGroupViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveCalledForValidChangedFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "FoodGroup", "" );

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );

         var foodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         foodGroupViewModel.Description = "Something New";

         Assert.IsTrue( foodGroupViewModel.IsValid );
         Assert.IsTrue( foodGroupViewModel.IsDirty );
         Assert.IsFalse( foodGroupViewModel.IsNew );

         foodGroupViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidChangedFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup();

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );

         var foodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         foodGroupViewModel.Description = "Something New";

         Assert.IsFalse( foodGroupViewModel.IsValid );
         Assert.IsTrue( foodGroupViewModel.IsDirty );
         Assert.IsFalse( foodGroupViewModel.IsNew );

         foodGroupViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveNotCalledForNonChangedFoodGroup()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "FoodGroup", "" );

         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );

         var foodGroupViewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object,
            new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );

         Assert.IsTrue( foodGroupViewModel.IsValid );
         Assert.IsFalse( foodGroupViewModel.IsDirty );
         Assert.IsFalse( foodGroupViewModel.IsNew );

         foodGroupViewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Never() );
      }

      [TestMethod]
      public void CannotDeleteFoodGroupIfNew()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         // If any and all calls to Contains() returns false, everything will look like it is new.
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( It.IsAny<FoodGroup>() ) ).Returns( false );

         var viewModel =
             new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         viewModel.Name = "New Food Group, Delete Test #1";
         viewModel.Description = "New valid food group.";

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

         interactionServiceMock.Verify(
            x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ),
            Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Never() );
      }

      [TestMethod]
      public void CannotDeleteFoodGroupIfUsed()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test Me", "This is a test" );
         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodGroup ) ).Returns( true );

         var viewModel =
             new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
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
            x => x.ShowMessageBox( It.IsAny<String>(), It.IsAny<String>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>() ),
            Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Never() );
      }

      [TestMethod]
      public void FoodGroupIsDeletedIfAnswerIsYes()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test Me", "This is a test" );
         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodGroup ) ).Returns( false );

         var viewModel =
             new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
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
               Messages.Question_FoodGroup_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.Yes );
         dataRepositoryMock.Setup( x => x.Remove( foodGroup ) );

         Assert.IsFalse( viewModel.IsNew );
         Assert.IsFalse( viewModel.IsUsed );
         Assert.IsTrue( viewModel.IsValid );
         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );

         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( It.IsAny<UserControl>() ), Times.Exactly( 1 ) );
      }

      [TestMethod]
      public void FoodGroupIsNotDeletedIfAnswerIsNo()
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var foodGroup = new FoodGroup( Guid.NewGuid(), "Test Me", "This is a test" );
         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( foodGroup ) ).Returns( false );

         var viewModel =
             new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
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
               Messages.Question_FoodGroup_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.No );

         Assert.IsFalse( viewModel.IsNew );
         Assert.IsFalse( viewModel.IsUsed );
         Assert.IsTrue( viewModel.IsValid );
         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );

         viewModel.DeleteCommand.Execute( null );

         interactionServiceMock.VerifyAll();
         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( It.IsAny<FoodGroup>() ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<FoodGroup>() ), Times.Never() );
      }


      [TestMethod]
      public void FoodGroupViewModelUndoRedo()
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

         // Should not be able to undo or redo on a new object.
         FoodGroupViewModel foodGroupViewModel =
            new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         NavigationContext navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Set initial data and save then load into new view model to reset the undo/redo cache
         foodGroupViewModel.Name = "Bob";
         foodGroupViewModel.Description = "Battery Operated Buddy";
         foodGroupViewModel.SaveCommand.Execute( null );
         navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + foodGroupViewModel.ID.ToString(), UriKind.Relative ) );
         foodGroupViewModel = new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
         foodGroupViewModel.OnNavigatedTo( navigationContext );
         Assert.IsFalse( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Make changes as such:
         //   o name changed from Bob to Pete
         //   o name changed from Pete to Peter
         //   o Description changed from "Battery Operated Buddy" to "The Rock"
         //   o name changed from Peter to Simon
         //   o name changed from Simon to Saul
         //   o description changed from "The Rock" to "The Persecutor"
         //   o description changed from "The Persecutor" to "The Apostle"
         //   o name changed from Saul to Paul
         // Verify can undo, cannot redo at each step
         foodGroupViewModel.Name = "Pete";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Name += "r";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Description = "The Rock";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Name = "Simon";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Name = "Saul";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Description = "The Persecutor";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Description = "The Apostle";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.Name = "Paul";
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         Assert.AreEqual( "Paul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Apostle", foodGroupViewModel.Description );

         // Undo once.  Verify last thing done is undone, and we can redo.
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Apostle", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Redo.  Verify last thing undone is redone, can no longer redo, can still undo.
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Paul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Apostle", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Undo 4 times, verify undo worked
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Apostle", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Persecutor", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Redo 2 times, verify
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Persecutor", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Undo 6 times.  Back to original, cannot undo, can redo
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Saul", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Simon", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodGroupViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Pete", foodGroupViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.UndoCommand.Execute( null );
         Assert.AreEqual( "Bob", foodGroupViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodGroupViewModel.Description );
         Assert.IsFalse( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );

         // Redo 3 times, verify
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Pete", foodGroupViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodGroupViewModel.Name );
         Assert.AreEqual( "Battery Operated Buddy", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
         foodGroupViewModel.RedoCommand.Execute( null );
         Assert.AreEqual( "Peter", foodGroupViewModel.Name );
         Assert.AreEqual( "The Rock", foodGroupViewModel.Description );
         Assert.IsTrue( foodGroupViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsTrue( foodGroupViewModel.RedoCommand.CanExecute( null ) );
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

         FoodGroupViewModel viewModel =
            new FoodGroupViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
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
         regions.Add( regionWithoutViewMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionWithoutViewMock.Setup( x => x.Views.GetEnumerator() ).Returns( viewsWithoutView.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         // Setup a food group in the mock repository, navigate to it
         FoodGroup foodGroup = new FoodGroup( Guid.NewGuid(), "Test Food Group", "Test Food Group Description" );
         dataRepositoryMock.Setup( x => x.GetFoodGroup( foodGroup.ID ) ).Returns( foodGroup );
         dataRepositoryMock.Setup( x => x.Contains( foodGroup ) ).Returns( true );
         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + foodGroup.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         if (makeDirty)
         {
            if (makeInvalid)
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_FoodGroup_Close, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsFalse( viewModel.IsValid );
            }
            else
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_FoodGroup_Save, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ) )
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
            dataRepositoryMock.Verify( x => x.SaveItem( foodGroup ), Times.Exactly( 1 ) );
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Exactly( 1 ) );
         }
         else
         {
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<FoodGroup>() ), Times.Never() );
         }
      }

      [TestMethod]
      public void FoodGroupViewModelCloseDirtyAnswerCancel()
      {
         RunCloseTest( true, false, MessageBoxResult.Cancel, false, false );
      }

      [TestMethod]
      public void FoodGroupViewModelCloseDirtyAnswerNo()
      {
         RunCloseTest( true, false, MessageBoxResult.No, true, false );
      }

      [TestMethod]
      public void FoodGroupViewModelCloseDirtyAnswerYes()
      {
         RunCloseTest( true, false, MessageBoxResult.Yes, true, true );
      }

      [TestMethod]
      public void FoodGroupViewModelCloseInvalidAnswerNo()
      {
         RunCloseTest( true, true, MessageBoxResult.No, false, false );
      }

      [TestMethod]
      public void FoodGroupViewModelCloseInvalidAnswerYes()
      {
         RunCloseTest( true, true, MessageBoxResult.Yes, true, false );
      }

      [TestMethod]
      public void FoodGroupViewModelCloseRemovesCleanView()
      {
         RunCloseTest( false, false, MessageBoxResult.None, true, false );
      }
      #endregion

      #region Other Tests
      [TestMethod]
      public void NavigateToExistingFoodGroup()
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

         var viewModel = new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext =
           new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView?ID=" + FullTestData.DairyID, UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.DairyID, viewModel.ID );
         Assert.AreEqual( "Dairy", viewModel.Name );
         Assert.AreEqual( "Mostly stuff from cows.", viewModel.Description );
         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void NavigateToNewFoodGroup()
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

         var viewModel = new FoodGroupViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "FoodGroupView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreNotEqual( FullTestData.DairyID, viewModel.ID );
         Assert.IsNull( viewModel.Name );
         Assert.IsNull( viewModel.Description );
         Assert.IsFalse( viewModel.IsDirty );
      }
      #endregion
   }
}
