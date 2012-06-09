using System.Collections.ObjectModel;
using System.Linq;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Represents a read-only container of all MealType objects currently in the system.
   /// Used to represent non-modifiable lists of mealTemplate types, such as a list of values.
   /// </summary>
   public class AllMealTypesViewModel : AllDataObjectViewModelBase
   {
      #region Constructors
      public AllMealTypesViewModel( IDataRepository dataRepository )
      {
         _mealTypes = new ObservableCollection<MealType>( dataRepository.GetAllMealTypes() );
         Items = new ReadOnlyObservableCollection<MealType>( _mealTypes );

         _dataRepository = dataRepository;
         _dataRepository.ItemAdded += this.OnMealTypeAddedToRepository;
         _dataRepository.ItemDeleted += this.OnMealTypeRemovedFromRepository;
         _dataRepository.ItemModified += this.OnMealTypeModified;
      }
      #endregion

      #region Public Interface
      private ObservableCollection<MealType> _mealTypes;
      public ReadOnlyObservableCollection<MealType> Items { get; private set; }
      #endregion

      #region IDisposable Members
      protected override void OnDispose( bool disposing )
      {
         base.OnDispose( disposing );

         _dataRepository.ItemAdded -= this.OnMealTypeAddedToRepository;
         _dataRepository.ItemDeleted -= this.OnMealTypeRemovedFromRepository;
         _dataRepository.ItemModified -= this.OnMealTypeModified;
      }
      #endregion

      #region Private Event Handlers
      private IDataRepository _dataRepository;

      private void OnMealTypeRemovedFromRepository( object sender, RepositoryObjectEventArgs e )
      {
         MealType mealType = e.Item as MealType;
         if (mealType != null)
         {
            _mealTypes.Remove( _mealTypes.ToList().Find( x => x.ID == mealType.ID ) );
         }
      }

      private void OnMealTypeAddedToRepository( object sender, RepositoryObjectEventArgs e )
      {
         MealType mealType = e.Item as MealType;
         if (mealType != null)
         {
            this._mealTypes.Add( mealType );
         }
      }

      private void OnMealTypeModified( object sender, RepositoryObjectEventArgs e )
      {
         MealType mealType = e.Item as MealType;
         if (mealType != null)
         {
            var index = _mealTypes.IndexOf( _mealTypes.ToList().Find( x => x.ID == mealType.ID ) );
            _mealTypes.RemoveAt( index );
            _mealTypes.Insert( index, mealType );
         }
      }
      #endregion
   }
}
