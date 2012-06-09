using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace HealthTracker.BaseClasses.ViewModels
{
   public class TreeNodeViewModel : NotificationObject
   {
      #region Constructors
      public TreeNodeViewModel( String name )
      {
         Name = name;
      }
      #endregion

      #region Public Interface
      private String _name;
      public String Name
      { 
         get
         {
            return _name;
         }
         set
         {
            _name = value;
            RaisePropertyChanged( "Name" );
         }
      }

      private Boolean _isSelected;
      public Boolean IsSelected
      {
         get
         {
            return _isSelected;
         }
         set
         {
            _isSelected = value;
            RaisePropertyChanged( "IsSelected" );
         }
      }

      private Boolean _isExpanded;
      public Boolean IsExpanded
      {
         get
         {
            return _isExpanded;
         }
         set
         {
            _isExpanded = value;
            RaisePropertyChanged( "IsExpanded" );
         }
      }

      protected ObservableCollection<TreeNodeViewModel> _children;
      public ReadOnlyObservableCollection<TreeNodeViewModel> Children { get; protected set; }
      #endregion
   }
}
