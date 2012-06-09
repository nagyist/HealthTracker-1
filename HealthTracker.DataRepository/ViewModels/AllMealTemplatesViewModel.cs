using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;
using System.Collections.ObjectModel;

namespace HealthTracker.DataRepository.ViewModels
{
   public class AllMealTemplatesViewModel : AllDataObjectViewModelBase
   {
      private IDataRepository _dataRepository;

      public AllMealTemplatesViewModel( IDataRepository dataRepository )
      {
         _dataRepository = dataRepository;

         _dataRepository.ItemAdded += this.OnMealTemplateAdded;
         _dataRepository.ItemDeleted += this.OnMealTemplateRemoved;
         _dataRepository.ItemModified += this.OnMealTemplateModified;

         _mealTemplates = new ObservableCollection<MealTemplate>( dataRepository.GetAllMealTemplates() );
         Items = new ReadOnlyObservableCollection<MealTemplate>( _mealTemplates );
      }

      #region Public Interface
      private ObservableCollection<MealTemplate> _mealTemplates;
      public ReadOnlyObservableCollection<MealTemplate> Items { get; private set; }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         _dataRepository.ItemAdded -= this.OnMealTemplateAdded;
         _dataRepository.ItemDeleted -= this.OnMealTemplateRemoved;
         _dataRepository.ItemModified -= this.OnMealTemplateModified;
      }
      #endregion

      #region Private Event Handlers
      private void OnMealTemplateRemoved( object sender, RepositoryObjectEventArgs e )
      {
         MealTemplate mealTemplate = e.Item as MealTemplate;
         if (mealTemplate != null)
         {
            _mealTemplates.Remove( _mealTemplates.ToList().Find( x => x.ID == mealTemplate.ID ) );
         }
      }

      private void OnMealTemplateAdded( object sender, RepositoryObjectEventArgs e )
      {
         MealTemplate mealTemplate = e.Item as MealTemplate;
         if (mealTemplate != null)
         {
            _mealTemplates.Add( mealTemplate );
         }
      }

      private void OnMealTemplateModified( object sender, RepositoryObjectEventArgs e )
      {
         MealTemplate mealTemplate = e.Item as MealTemplate;
         if (mealTemplate != null)
         {
            var index = _mealTemplates.IndexOf( _mealTemplates.ToList().Find( x => x.ID == mealTemplate.ID ) );
            _mealTemplates.RemoveAt( index );
            _mealTemplates.Insert( index, mealTemplate );
         }
      }
      #endregion
   }
}
