using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.DataRepository.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UnitTests.Support;

namespace UnitTests.DataRepository
{
   [TestClass]
   public class AllMealTemplatesViewModelTest
   {
      #region Private Helpers
      private Mock<IDataRepository> CreateMockDataRepository( List<MealTemplate> mealTemplates )
      {
         var dataRespositoryMock = new Mock<IDataRepository>();
         dataRespositoryMock.Setup( x => x.GetAllMealTemplates() )
            .Returns( new ReadOnlyCollection<MealTemplate>( (from mealTemplate in mealTemplates
                                                             select new MealTemplate( mealTemplate )).ToList() ) );

         return dataRespositoryMock;
      }

      private void AssertViewModelContents( AllMealTemplatesViewModel viewModel, List<MealTemplate> mealTemplates )
      {
         Assert.AreEqual( mealTemplates.Count, viewModel.Items.Count );

         foreach (var template in viewModel.Items)
         {
            var templateFromRepository = mealTemplates.Find( mt => mt.ID == template.ID );

            Assert.IsNotNull( template );
            Assert.AreEqual( templateFromRepository.Name, template.Name );
            Assert.AreEqual( templateFromRepository.Calories, template.Calories );
         }
      }
      #endregion

      #region Tests
      [TestMethod]
      public void AllMealTemplatesViewModelContainsAllTemplates()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTemplates() );

         var viewModel = new AllMealTemplatesViewModel( dataRespositoryMock.Object );

         AssertViewModelContents( viewModel, data.MealTemplates() );
         dataRespositoryMock.VerifyAll();
      }

      [TestMethod]
      public void MealTemplateRemovedFromViewModelWhenRemovedFromRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTemplates() );

         var viewModel = new AllMealTemplatesViewModel( dataRespositoryMock.Object );
         var mealTemplate = data.MealTemplates().Find( mt => mt.ID == MockData.CheeseburgerLunchID );
         data.MealTemplates().Remove( mealTemplate );
         dataRespositoryMock.Raise( e => e.ItemDeleted += null, new RepositoryObjectEventArgs( mealTemplate ) );

         AssertViewModelContents( viewModel, data.MealTemplates() );
      }

      [TestMethod]
      public void MealTemplateAddedToViewModelWhenAddedToRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTemplates() );

         var viewModel = new AllMealTemplatesViewModel( dataRespositoryMock.Object );
         var mealTemplate = new MealTemplate( Guid.NewGuid(), data.MealTypes().Find( mt => mt.ID == MockData.dinnerID ), DateTime.Today.AddHours( 18 ), "Dine-in", "Eatme" );
         data.MealTemplates().Add( mealTemplate );
         dataRespositoryMock.Raise( e => e.ItemAdded += null, new RepositoryObjectEventArgs( mealTemplate ) );

         AssertViewModelContents( viewModel, data.MealTemplates() );
      }

      [TestMethod]
      public void MealTemplateModifiedInViewModelWhenChangedInRepository()
      {
         var data = new MockData();
         var dataRespositoryMock = CreateMockDataRepository( data.MealTemplates() );
         var viewModel = new AllMealTemplatesViewModel( dataRespositoryMock.Object );

         var mealTemplate = data.MealTemplates().Find( mt => mt.ID == MockData.CheeseburgerLunchID );
         mealTemplate.Name += " Unit Test";
         mealTemplate.FoodItemServings[0].Quantity += 1;
         dataRespositoryMock.Raise( e => e.ItemModified += null, new RepositoryObjectEventArgs( mealTemplate ) );

         AssertViewModelContents( viewModel, data.MealTemplates() );
      }
      #endregion
   }
}
