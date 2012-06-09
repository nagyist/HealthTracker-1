using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Administration.ViewModels;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.Configuration.Interfaces;
using HealthTracker.DataRepository.Services;
using HealthTracker.DataRepository.Models;
using HealthTracker.Infrastructure.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;

namespace UnitTests.Administration
{
   [TestClass]
   public class MealTemplateNodeViewModelTest
   {
      #region Constructors
      public MealTemplateNodeViewModelTest()
      { }
      #endregion

      #region Constructor Tests
      [TestMethod]
      public void MealTemplateNodeViewModelDefault()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateNodeViewModel mealTemplateNodeViewModel = new MealTemplateNodeViewModel( dataRepository, null );

         Assert.AreEqual( DisplayStrings.AdminMealTemplatesTitle, mealTemplateNodeViewModel.Name );

         // If the counts are the same, and every child is in the repository, then the data should be fine.
         Assert.AreEqual( dataRepository.GetAllMealTemplates().Count, mealTemplateNodeViewModel.Children.Count );
         foreach (ClickableTreeNodeViewModel node in mealTemplateNodeViewModel.Children)
         {
            MealTemplate mealTemplate = dataRepository.GetMealTemplate( (Guid)node.Parameter );
            Assert.IsNotNull( mealTemplate );
            Assert.AreEqual( mealTemplate.Name, node.Name );
         }
      }
      #endregion

      #region Event Handling Tests
      [TestMethod]
      public void MealTemplateAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();

         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateNodeViewModel mealTemplateNodeViewModel = new MealTemplateNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTemplateNodeViewModel.Children.Count;

         MealTemplate newMealTemplate = new MealTemplate(
            Guid.NewGuid(), dataRepository.GetAllMealTypes()[0], DateTime.Now, "Test Meal Template", "Just a test" );
         newMealTemplate.FoodItemServings.Add( new Serving<FoodItem>( dataRepository.GetAllFoodItems()[0], 1 ) );
         dataRepository.SaveItem( newMealTemplate );
         Assert.AreEqual( originalChildCount + 1, mealTemplateNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTemplateNodeViewModel.Children)
         {
            MealTemplate mealTemplate = dataRepository.GetMealTemplate( (Guid)node.Parameter );
            Assert.IsNotNull( mealTemplate );
            Assert.AreEqual( mealTemplate.Name, node.Name );
         }
      }

      [TestMethod]
      public void NonMealTemplateNotAddedToChildrenWhenAddedToRepository()
      {
         var configurationMock = new Mock<IConfiguration>();

         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateNodeViewModel mealTemplateNodeViewModel = new MealTemplateNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTemplateNodeViewModel.Children.Count;

         var mealType = new MealType( Guid.NewGuid(), "A New Meal Type", "This should not already exist", DateTime.Now, false );
         Assert.IsTrue( mealType.IsValid );
         dataRepository.SaveItem( mealType );

         Assert.AreEqual( originalChildCount, mealTemplateNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTemplateNodeViewModel.Children)
         {
            MealTemplate mealTemplate = dataRepository.GetMealTemplate( (Guid)node.Parameter );
            Assert.IsNotNull( mealTemplate );
            Assert.AreEqual( mealTemplate.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealTemplateNodeViewModelRemoveMealTemplate()
      {
         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateNodeViewModel mealTemplateNodeViewModel = new MealTemplateNodeViewModel( dataRepository, null );

         Int32 originalChildCount = mealTemplateNodeViewModel.Children.Count;

         MealTemplate removedMealTemplate = dataRepository.GetAllMealTemplates().ElementAt( 0 );
         dataRepository.Remove( removedMealTemplate );
         Assert.AreEqual( originalChildCount - 1, mealTemplateNodeViewModel.Children.Count );

         foreach (ClickableTreeNodeViewModel node in mealTemplateNodeViewModel.Children)
         {
            MealTemplate mealTemplate = dataRepository.GetMealTemplate( (Guid)node.Parameter );
            Assert.IsNotNull( mealTemplate );
            Assert.AreEqual( mealTemplate.Name, node.Name );
         }
      }

      [TestMethod]
      public void MealTemplateNodeVewModelModifyMealTemplate()
      {
         PropertyChangedHandler propertyChangedHandler = new PropertyChangedHandler();

         var configurationMock = new Mock<IConfiguration>();
         configurationMock.Setup( x => x.DataSource ).Returns( DataSourceType.XMLFile );
         configurationMock.Setup( x => x.FileName ).Returns( FullTestData.DataFileName );

         FullTestData.Reset();
         DataRepository dataRepository = new DataRepository( configurationMock.Object );

         MealTemplateNodeViewModel mealTemplateNodeViewModel = new MealTemplateNodeViewModel( dataRepository, null );
         Int32 originalChildCount = mealTemplateNodeViewModel.Children.Count;

         MealTemplate mealTemplate = dataRepository.GetAllMealTemplates().ElementAt( 0 );
         String originalName = mealTemplate.Name;

         ClickableTreeNodeViewModel mealTemplateInTree = null;
         foreach (ClickableTreeNodeViewModel child in mealTemplateNodeViewModel.Children)
         {
            child.PropertyChanged += propertyChangedHandler.OnPropertyChanged;
            if ((Guid)child.Parameter == mealTemplate.ID)
            {
               mealTemplateInTree = child;
            }
         }

         Assert.IsNotNull( mealTemplateInTree );
         Assert.AreEqual( originalName, mealTemplateInTree.Name );

         mealTemplate.Name += "Modified";
         dataRepository.SaveItem( mealTemplate );

         Assert.AreEqual( originalName + "Modified", mealTemplateInTree.Name );
         Assert.AreEqual( 1, propertyChangedHandler.PropertiesChanged.Count );
         Assert.AreEqual( mealTemplateInTree, propertyChangedHandler.Sender );
         Assert.IsTrue( propertyChangedHandler.PropertiesChanged.Contains( "Name" ) );
      }
      #endregion
   }
}
