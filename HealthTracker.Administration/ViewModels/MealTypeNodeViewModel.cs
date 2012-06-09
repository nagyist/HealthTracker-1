using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.ViewModels;
using HealthTracker.Infrastructure.Properties;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.Administration.ViewModels
{
   public class MealTypeNodeViewModel : DataObjectCollectionNodeViewModel
   {
      #region Constructors
      public MealTypeNodeViewModel( IDataRepository dataRepository, ICommand childClickCommand )
         : base( DisplayStrings.AdminMealTypesTitle, dataRepository, childClickCommand )
      {
         _children =
             new ObservableCollection<TreeNodeViewModel>(
             (from mealType in dataRepository.GetAllMealTypes()
              select new ClickableTreeNodeViewModel( mealType.Name, childClickCommand, mealType.ID )).ToList() );
         Children = new ReadOnlyObservableCollection<TreeNodeViewModel>( _children );
      }
      #endregion

      #region Event Handlers
      protected override void OnItemAdded( object sender, RepositoryObjectEventArgs e )
      {
         MealType mealType = e.Item as MealType;

         if (mealType != null)
         {
            _children.Add( new ClickableTreeNodeViewModel( mealType.Name, _childClickCommand, mealType.ID ) );
         }
      }
      #endregion
   }
}
