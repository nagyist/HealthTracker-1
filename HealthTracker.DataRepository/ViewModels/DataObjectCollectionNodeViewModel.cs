using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using HealthTracker.BaseClasses.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using HealthTracker.DataRepository.Models;

namespace HealthTracker.DataRepository.ViewModels
{
   /// <summary>
   /// Base class for nodes that represent collections of data in the repository.
   /// 
   /// If the node in the tree is the header node for a collection of items in the data
   /// repository (such as food groups) then it should derive from this class.
   /// </summary>
   public abstract class DataObjectCollectionNodeViewModel : TreeNodeViewModel
   {
      #region Constructors
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name">Name of the collection</param>
      /// <param name="dataRepository">The data repository</param>
      /// <param name="childClickCommand">The command that should be executed if a child is clicked on.</param>
      public DataObjectCollectionNodeViewModel( String name, IDataRepository dataRepository, ICommand childClickCommand )
         : base( name )
      {

         dataRepository.ItemAdded += this.OnItemAdded;
         dataRepository.ItemDeleted += this.OnItemRemoved;
         dataRepository.ItemModified += this.OnItemModified;

         _childClickCommand = childClickCommand;
      }
      #endregion

      #region Event Handlers
      protected abstract void OnItemAdded( Object sender, RepositoryObjectEventArgs e );

      private void OnItemRemoved( Object sender, RepositoryObjectEventArgs e )
      {
         TreeNodeViewModel child = _children.ToList().Find( f => ((Guid)((ClickableTreeNodeViewModel)f).Parameter) == e.Item.ID );
         if (child != null)
         {
            _children.Remove( child );
         }
      }

      private void OnItemModified( Object sender, RepositoryObjectEventArgs e )
      {
         TreeNodeViewModel child = _children.ToList().Find( f => ((Guid)((ClickableTreeNodeViewModel)f).Parameter) == e.Item.ID );
         if (child != null)
         {
            child.Name = ((DataObject)e.Item).Name;
         }
      }
      #endregion

      #region Protected Data
      protected ICommand _childClickCommand;
      #endregion
   }
}
