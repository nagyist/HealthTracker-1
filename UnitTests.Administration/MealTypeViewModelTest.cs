using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HealthTracker.Administration.ViewModels;
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

namespace UnitTests.Administration
{
   [TestClass]
   public class MealTypeViewModelTest
   {
      #region Private Helpers
      private MealTypeViewModel CreateEmptyViewModel( Mock<IDataRepository> dataRepositoryMock )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         var viewModel =
            new MealTypeViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTypeView", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         return viewModel;
      }

      private MealTypeViewModel CreateEmptyViewModel()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         return CreateEmptyViewModel( dataRepositoryMock );
      }


      private MealTypeViewModel CreateViewModelForMealType(
         MealType mealType, Mock<IDataRepository> dataRepositoryMock, Mock<IRegionManager> regionManagerMock, Mock<IInteractionService> interactionServiceMock )
      {
         var loggerMock = new Mock<ILoggerFacade>();
         var regionNavigationServiceMock = new Mock<IRegionNavigationService>();

         var viewModel =
            new MealTypeViewModel( dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         dataRepositoryMock.Setup( x => x.GetMealType( mealType.ID ) ).Returns( mealType );

         var navigationContext = new NavigationContext(
            regionNavigationServiceMock.Object, new Uri( "MealTypeView?ID=" + mealType.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         return viewModel;
      }

      private MealTypeViewModel CreateViewModelForMealType( MealType mealType, Mock<IDataRepository> dataRepositoryMock )
      {
         var regionManagerMock = new Mock<IRegionManager>();
         var interactionServiceMock = new Mock<IInteractionService>();

         return CreateViewModelForMealType( mealType, dataRepositoryMock, regionManagerMock, interactionServiceMock );
      }

      private MealTypeViewModel CreateViewModelForMealType( MealType mealType )
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         return CreateViewModelForMealType( mealType, dataRepositoryMock );
      }


      private UserControl CreateViewInRegion( MealTypeViewModel viewModel, Mock<IRegion> regionMock, Mock<IRegionManager> regionManagerMock )
      {
         // Set up region manager so we can determine if the view get removed or not.
         var view = new UserControl();
         view.DataContext = viewModel;

         var views = new List<UserControl>();
         views.Add( new UserControl() );
         views.Add( view );
         views.Add( new UserControl() );
         views.Add( new UserControl() );

         var regions = new List<IRegion>();
         regions.Add( regionMock.Object );

         regionManagerMock.Setup( x => x.Regions.GetEnumerator() ).Returns( regions.GetEnumerator() );
         regionMock.Setup( x => x.Views.GetEnumerator() ).Returns( views.GetEnumerator() );

         return view;
      }

      #endregion

      #region Public Interface Tests
      [TestMethod]
      public void MealTypeViewModelIsNotDirtyWhenLoaded()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void MealTypeViewModelIsDirtyWhenNameChanged()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Name += "Modified";

         Assert.IsTrue( viewModel.IsDirty );
      }

      [TestMethod]
      public void MealTypeViewModelIsDirtyWhenNameChangedBack()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Name += "Modified";
         viewModel.Name = "Test Type";

         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void MealTypeViewModelIsDirtyWhenDescriptionChanged()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Description += "Modified";

         Assert.IsTrue( viewModel.IsDirty );
      }

      [TestMethod]
      public void MealTypeViewModelIsDirtyWhenDescriptionChangedBack()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Description += "Modified";
         viewModel.Description = "Just for the unit test";

         Assert.IsFalse( viewModel.IsDirty );
      }

      [TestMethod]
      public void MealTypeViewModelIsValidWhenConstructedWithValidMealType()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.IsTrue( viewModel.IsValid );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Error ) );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Name"] ) );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTypeViewModelIsNotValidWhenConstructedWithInvalidMealType()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "", "No name, will be invalid", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.IsFalse( viewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, viewModel.Error );
         Assert.AreEqual( Messages.Error_No_Name, viewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTypeViewModelIsNotValidWhenConstructedForEntryOfNewMealType()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsFalse( viewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, viewModel.Error );
         Assert.AreEqual( Messages.Error_No_Name, viewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTypeViewModelIsNotValidWhenDuplicateName()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var data = new MockData();

         var mealType = new MealType( Guid.NewGuid(), "Test Meal Type", "Unit Test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );
         dataRepositoryMock.Setup( x => x.NameIsDuplicate( mealType ) ).Returns( true );

         Assert.IsFalse( viewModel.IsValid );
         Assert.AreEqual( Messages.Error_MealType_Exists, viewModel.Error );
         Assert.AreEqual( Messages.Error_MealType_Exists, viewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTypeViewModelIsNotValidWhenEmptyName()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Name = "";

         Assert.IsFalse( viewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, viewModel.Error );
         Assert.AreEqual( Messages.Error_No_Name, viewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void MealTypeViewModelIsNotValidWhenNullName()
      {
         MealType mealType = new MealType( Guid.NewGuid(), "Test Type", "Just for the unit test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.Name = null;

         Assert.IsFalse( viewModel.IsValid );
         Assert.AreEqual( Messages.Error_No_Name, viewModel.Error );
         Assert.AreEqual( Messages.Error_No_Name, viewModel["Name"] );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel["Description"] ) );
      }

      [TestMethod]
      public void NewMealTypeViewModelIsNew()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealType>() ) ).Returns( false );
         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         Assert.IsTrue( viewModel.IsNew );
      }

      [TestMethod]
      public void ExistingMealTypeViewModelIsNotNew()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Meal Type", "This is a test meal type", DateTime.Now, false );
         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         Assert.IsFalse( viewModel.IsNew );
      }

      [TestMethod]
      public void ViewModelIsUsedIfMealTypeIsUsedInRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealType ) ).Returns( true );

         Assert.IsTrue( viewModel.IsUsed );
      }

      [TestMethod]
      public void ViewModelIsNotUsedIfMealTypeIsNotUsedInRepository()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealType ) ).Returns( false );

         Assert.IsFalse( viewModel.IsUsed );
      }

      [TestMethod]
      public void NewMealTypeIsNotUsed()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<FoodGroup>() ) ).Returns( false );

         Assert.IsFalse( viewModel.IsUsed );
      }

      [TestMethod]
      public void MealTypeViewModelPublicPropertiesMatchMealType()
      {
         var mealType = new MealType( Guid.NewGuid(), "Cookie", "From the cookie jar", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.AreEqual( mealType.ID, viewModel.ID );
         Assert.AreEqual( mealType.Name, viewModel.Name );
         Assert.AreEqual( mealType.Description, viewModel.Description );
      }

      [TestMethod]
      public void ChangingMealTypeNameMarksNameIsDirtyIsValidChanged()
      {
         var mealType = new MealType( Guid.NewGuid(), "Peanut", "Butter Jelly Time", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.Name = "Jelly";

         Assert.AreEqual( 3, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsValid" ) );
      }

      [TestMethod]
      public void ChangingMealTypeDescriptionMarksDescriptionIsDirtyChanged()
      {
         var mealType = new MealType( Guid.NewGuid(), "Peanut", "Butter Jelly Time", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();
         viewModel.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
         viewModel.Description = "with a baseball bat";

         Assert.AreEqual( 2, propertyChangedHandler.PropertiesChanged.Count );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Description" ) );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "IsDirty" ) );
      }


      [TestMethod]
      public void TitleDefaultForEmptyMealType()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.AreEqual( DisplayStrings.NewMealTypeTitle, viewModel.Title );
      }

      [TestMethod]
      public void TitleChangedToNameAfterSave()
      {
         var viewModel = CreateEmptyViewModel();

         viewModel.Name = "Test";
         Assert.AreEqual( DisplayStrings.NewMealTypeTitle, viewModel.Title );

         viewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "Test", viewModel.Title );
      }

      [TestMethod]
      public void TitleMatchesName()
      {
         var mealType = new MealType( Guid.NewGuid(), "Test", "this is a test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.AreEqual( "Test", viewModel.Title );

         viewModel.Name = "Fuzzy";
         Assert.AreEqual( "Test", viewModel.Title );

         viewModel.SaveCommand.Execute( null );
         Assert.AreEqual( "Fuzzy", viewModel.Title );
      }

      [TestMethod]
      public void TimeDefaultsToNow()
      {
         var currentTime = DateTime.Now;
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( viewModel.DefaultTimeOfMeal >= currentTime );
         Assert.IsTrue( viewModel.DefaultTimeOfMeal < currentTime.AddSeconds( 0.5 ) );
      }

      [TestMethod]
      public void TimeMatchesMealTypeTime()
      {
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Today.AddDays( -1 ).AddHours( 8 ), false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.AreEqual( mealType.DefaultTimeOfMeal, viewModel.DefaultTimeOfMeal );
      }

      [TestMethod]
      public void UseAsDefaultOnMealDefaultsToTrue()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( viewModel.UseDefaultTimeOfMeal );
      }

      [TestMethod]
      public void UseAsDefaultOnMealMatchesMealType()
      {
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         Assert.IsFalse( viewModel.UseDefaultTimeOfMeal );

         mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, true );
         viewModel = CreateViewModelForMealType( mealType );

         Assert.IsTrue( viewModel.UseDefaultTimeOfMeal );
      }

      [TestMethod]
      public void SettingDateInViewModelSetsDateInModel()
      {
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         var newTime = DateTime.Now.AddDays( 3.25 );
         viewModel.DefaultTimeOfMeal = newTime;

         Assert.AreEqual( newTime, mealType.DefaultTimeOfMeal );
      }

      [TestMethod]
      public void SettingUseFlagInViewModelSetsUseFlagInModel()
      {
         var mealType = new MealType( Guid.NewGuid(), "test", "test", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType );

         viewModel.UseDefaultTimeOfMeal = true;
         Assert.IsTrue( mealType.UseDefaultMealTime );

         viewModel.UseDefaultTimeOfMeal = false;
         Assert.IsFalse( mealType.UseDefaultMealTime );
      }
      #endregion

      #region Command Tests
      [TestMethod]
      public void SaveCalledForValidNewMealType()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealType>() ) ).Returns( false );

         viewModel.Name = "New";
         viewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidNewMealType()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var viewModel = CreateEmptyViewModel( dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( It.IsAny<MealType>() ) ).Returns( false );

         viewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveCalledForValidChangedMealType()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );

         viewModel.Name = "Changed";
         viewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Once() );
      }

      [TestMethod]
      public void SaveNotCalledForInvalidChangedMealType()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );

         viewModel.Name = "";
         viewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Never() );
      }

      [TestMethod]
      public void SaveNotCalledForNonChangedMealType()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "Test", "", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );

         viewModel.SaveCommand.Execute( null );

         dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Never() );
      }


      [TestMethod]
      public void CannotDeleteMealTypeIfNew()
      {
         var viewModel = CreateEmptyViewModel();

         Assert.IsTrue( viewModel.IsNew );
         Assert.IsFalse( viewModel.DeleteCommand.CanExecute( null ) );
      }


      [TestMethod]
      public void CannotDeleteMealTypeIfUsed()
      {
         var dataRespositoryMock = new Mock<IDataRepository>();
         var mealType = new MealType( Guid.NewGuid(), "This is a test", "This is a test meal type", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRespositoryMock );

         dataRespositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         dataRespositoryMock.Setup( x => x.ItemIsUsed( mealType ) ).Returns( true );

         Assert.IsFalse( viewModel.IsNew );
         Assert.IsFalse( viewModel.DeleteCommand.CanExecute( null ) );
         dataRespositoryMock.VerifyAll();
      }


      [TestMethod]
      public void MealTypeIsDeletedIfAnswerIsYes()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var mealType = new MealType( Guid.NewGuid(), "Test Meal Type", "This is a test meal type", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock, regionManagerMock, interactionServiceMock );
         var view = CreateViewInRegion( viewModel, regionMock, regionManagerMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealType ) ).Returns( false );
         dataRepositoryMock.Setup( x => x.Remove( mealType ) );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox( Messages.Question_MealType_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.Yes );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         dataRepositoryMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( mealType ) );
         interactionServiceMock.VerifyAll();
         regionMock.Verify( x => x.Remove( view ), Times.Exactly( 1 ) );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Exactly( 1 ) );
      }

      [TestMethod]
      public void MealTypeIsNotDeletedIfAnswerIsNo()
      {
         var dataRepositoryMock = new Mock<IDataRepository>();
         var regionManagerMock = new Mock<IRegionManager>();
         var regionMock = new Mock<IRegion>();
         var interactionServiceMock = new Mock<IInteractionService>();
         var mealType = new MealType( Guid.NewGuid(), "Test Meal Type", "This is a test meal type", DateTime.Now, false );
         var viewModel = CreateViewModelForMealType( mealType, dataRepositoryMock, regionManagerMock, interactionServiceMock );
         var view = CreateViewInRegion( viewModel, regionMock, regionManagerMock );

         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         dataRepositoryMock.Setup( x => x.ItemIsUsed( mealType ) ).Returns( false );

         interactionServiceMock
            .Setup( x => x.ShowMessageBox( Messages.Question_MealType_Delete, DisplayStrings.DeleteCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
            .Returns( MessageBoxResult.No );

         Assert.IsTrue( viewModel.DeleteCommand.CanExecute( null ) );
         viewModel.DeleteCommand.Execute( null );

         dataRepositoryMock.VerifyAll();
         interactionServiceMock.VerifyAll();
         dataRepositoryMock.Verify( x => x.Remove( It.IsAny<MealType>() ), Times.Never() );
         regionMock.Verify( x => x.Remove( It.IsAny<Object>() ), Times.Never() );
      }


      private void AssertMealTypeContents( 
         MealTypeViewModel viewModel, String name, String description, DateTime defaultTimeOfMeal, Boolean useDefault, Boolean canUndo, Boolean canRedo, String message )
      {
         Assert.AreEqual( name, viewModel.Name, message );
         Assert.AreEqual( description, viewModel.Description, message );
         Assert.AreEqual( defaultTimeOfMeal, viewModel.DefaultTimeOfMeal, message );
         Assert.AreEqual( useDefault, viewModel.UseDefaultTimeOfMeal, message );
         Assert.AreEqual( canUndo, viewModel.UndoCommand.CanExecute( null ), message );
         Assert.AreEqual( canRedo, viewModel.RedoCommand.CanExecute( null ), message );
      }

      [TestMethod]
      public void MealTypeViewModelUndoRedo()
      {
         var startTime = DateTime.Now;
         var mealType = new MealType( Guid.NewGuid(), "Bob", "Battery Operated Buddy", startTime, false );
         var mealTypeViewModel = CreateViewModelForMealType( mealType );
         Assert.IsFalse( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );

         // Make changes as such:
         //   o Change Time
         //   o Toggle use flag
         //   o name changed from Bob to Pete
         //   o name changed from Pete to Peter
         //   o Description changed from "Battery Operated Buddy" to "The Rock"
         //   o name changed from Peter to Simon
         //   o name changed from Simon to Saul
         //   o description changed from "The Rock" to "The Persecutor"
         //   o description changed from "The Persecutor" to "The Apostle"
         //   o name changed from Saul to Paul
         // Verify can undo, cannot redo at each step
         var newTime = mealTypeViewModel.DefaultTimeOfMeal.AddHours( 6 );
         mealTypeViewModel.DefaultTimeOfMeal = newTime;
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.UseDefaultTimeOfMeal = true;
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Name = "Pete";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Name += "r";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Description = "The Rock";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Name = "Simon";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Name = "Saul";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Description = "The Persecutor";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Description = "The Apostle";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );
         mealTypeViewModel.Name = "Paul";
         Assert.IsTrue( mealTypeViewModel.UndoCommand.CanExecute( null ) );
         Assert.IsFalse( mealTypeViewModel.RedoCommand.CanExecute( null ) );

         Assert.AreEqual( "Paul", mealTypeViewModel.Name );
         Assert.AreEqual( "The Apostle", mealTypeViewModel.Description );

         // Undo once.  Verify last thing done is undone, and we can redo.
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Apostle", newTime, true, true, true, "Undo 1" );
         
         // Redo.  Verify last thing undone is redone, can no longer redo, can still undo.
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Paul", "The Apostle", newTime, true, true, false, "Redo 1" );

         // Undo 4 times, verify undo worked
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Apostle", newTime, true, true, true, "Undo 2.1" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Persecutor", newTime, true, true, true, "Undo 2.2" ); 
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Rock", newTime, true, true, true, "Undo 2.3" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Simon", "The Rock", newTime, true, true, true, "Undo 2.4" );

         // Redo 2 times, verify
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Rock", newTime, true, true, true, "Redo 2.1" );
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Persecutor", newTime, true, true, true, "Redo 2.2" );

         // Undo 8 times.  Back to original, cannot undo, can redo
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Saul", "The Rock", newTime, true, true, true, "Undo 3.1" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Simon", "The Rock", newTime, true, true, true, "Undo 3.2" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Peter", "The Rock", newTime, true, true, true, "Undo 3.3" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Peter", "Battery Operated Buddy", newTime, true, true, true, "Undo 3.4" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Pete", "Battery Operated Buddy", newTime, true, true, true, "Undo 3.5" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Bob", "Battery Operated Buddy", newTime, true, true, true, "Undo 3.6" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Bob", "Battery Operated Buddy", newTime, false, true, true, "Undo 3.7" );
         mealTypeViewModel.UndoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Bob", "Battery Operated Buddy", startTime, false, false, true, "Undo 3.8" );

         // Redo 3 times, verify
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Bob", "Battery Operated Buddy", newTime, false, true, true, "Redo 3.1" );
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Bob", "Battery Operated Buddy", newTime, true, true, true, "Redo 3.2" );
         mealTypeViewModel.RedoCommand.Execute( null );
         AssertMealTypeContents( mealTypeViewModel, "Pete", "Battery Operated Buddy", newTime, true, true, true, "Redo 3.3" );
      }
      #endregion

      #region Close Tests
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

         MealTypeViewModel viewModel = new MealTypeViewModel(
            dataRepositoryMock.Object, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );
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

         // Setup a mealTemplate type in the mock repository, navigate to it
         MealType mealType = new MealType( Guid.NewGuid(), "Test Meal Type", "Test Meal Type Description", DateTime.Now, false );
         dataRepositoryMock.Setup( x => x.GetMealType( mealType.ID ) ).Returns( mealType );
         dataRepositoryMock.Setup( x => x.Contains( mealType ) ).Returns( true );
         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealTypeView?ID=" + mealType.ID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );

         if (makeDirty)
         {
            if (makeInvalid)
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_MealType_Close, DisplayStrings.CloseCaption, MessageBoxButton.YesNo, MessageBoxImage.Question ) )
                  .Returns( messageResponse );
               viewModel.Name = "";
               Assert.IsTrue( viewModel.IsDirty );
               Assert.IsFalse( viewModel.IsValid );
            }
            else
            {
               interactionServiceMock
                  .Setup( x => x.ShowMessageBox( Messages.Question_MealType_Save, DisplayStrings.SaveChangesCaption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question ) )
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
            dataRepositoryMock.Verify( x => x.SaveItem( mealType ), Times.Exactly( 1 ) );
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Exactly( 1 ) );
         }
         else
         {
            dataRepositoryMock.Verify( x => x.SaveItem( It.IsAny<MealType>() ), Times.Never() );
         }
      }

      [TestMethod]
      public void MealTypeViewModelCloseDirtyAnswerCancel()
      {
         RunCloseTest( true, false, MessageBoxResult.Cancel, false, false );
      }

      [TestMethod]
      public void MealTypeViewModelCloseDirtyAnswerNo()
      {
         RunCloseTest( true, false, MessageBoxResult.No, true, false );
      }

      [TestMethod]
      public void MealTypeViewModelCloseDirtyAnswerYes()
      {
         RunCloseTest( true, false, MessageBoxResult.Yes, true, true );
      }

      [TestMethod]
      public void MealTypeViewModelCloseInvalidAnswerNo()
      {
         RunCloseTest( true, true, MessageBoxResult.No, false, false );
      }

      [TestMethod]
      public void MealTypeViewModelCloseInvalidAnswerYes()
      {
         RunCloseTest( true, true, MessageBoxResult.Yes, true, false );
      }

      [TestMethod]
      public void MealTypeViewModelCloseRemovesCleanView()
      {
         RunCloseTest( false, false, MessageBoxResult.None, true, false );
      }
      #endregion

      #region Other Tests
      [TestMethod]
      public void NavigateToExitingMealType()
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

         MealTypeViewModel viewModel = new MealTypeViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         // Navigate to an existing mealTemplate type
         NavigationContext navigationContext =
            new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealType?ID=" + FullTestData.BreakfastID.ToString(), UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreEqual( FullTestData.BreakfastID, viewModel.ID );
         Assert.AreEqual( "Breakfast", viewModel.Name );
         Assert.AreEqual( "The most important meal of the day.", viewModel.Description );
         Assert.IsFalse( viewModel.IsDirty );
      }


      [TestMethod]
      public void NavigateToNewMealType()
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

         var viewModel = new MealTypeViewModel( dataRepository, regionManagerMock.Object, interactionServiceMock.Object, loggerMock.Object );

         var navigationContext = new NavigationContext( regionNavigationServiceMock.Object, new Uri( "MealType", UriKind.Relative ) );
         viewModel.OnNavigatedTo( navigationContext );
         Assert.AreNotEqual( FullTestData.BreakfastID, viewModel.ID );
         Assert.IsNull( viewModel.Name );
         Assert.IsNull( viewModel.Description );
         Assert.IsFalse( viewModel.IsDirty );
      }
      #endregion
   }
}
