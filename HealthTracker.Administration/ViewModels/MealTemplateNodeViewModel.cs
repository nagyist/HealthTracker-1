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
   public class MealTemplateNodeViewModel : DataObjectCollectionNodeViewModel
   {
      #region Constructors
      public MealTemplateNodeViewModel( IDataRepository dataRepository, ICommand childClickCommand )
         : base( DisplayStrings.AdminMealTemplatesTitle, dataRepository, childClickCommand )
      {
         _children =
            new ObservableCollection<TreeNodeViewModel>(
            (from mealTemplate in dataRepository.GetAllMealTemplates()
             select new ClickableTreeNodeViewModel( mealTemplate.Name, childClickCommand, mealTemplate.ID )).ToList() );
         Children = new ReadOnlyObservableCollection<TreeNodeViewModel>( _children );
      }
      #endregion

      #region Event Handlers
      protected override void OnItemAdded( object sender, RepositoryObjectEventArgs e )
      {
         MealTemplate dataObject = e.Item as MealTemplate;

         if (dataObject != null)
         {
            _children.Add( new ClickableTreeNodeViewModel( dataObject.Name, _childClickCommand, dataObject.ID ) );
         }
      }
      #endregion
   }
}
