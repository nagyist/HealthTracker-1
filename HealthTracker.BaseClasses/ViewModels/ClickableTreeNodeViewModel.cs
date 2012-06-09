using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HealthTracker.BaseClasses.ViewModels
{
   /// <summary>
   /// A TreeNodeViewModel that is intended to be displayed as a button or
   /// hyperlink where an childClickCommand, such as opening a window or tab, is taken
   /// when the item is clicked.
   /// </summary>
   public class ClickableTreeNodeViewModel : TreeNodeViewModel
   {
      #region Constructors
      public ClickableTreeNodeViewModel( String name, ICommand clickCommand, Object parameter ) :
         base( name )
      {
         ClickCommand = clickCommand;
         Parameter = parameter;
      }
      #endregion

      #region Public Interface
      public ICommand ClickCommand { get; private set; }
      public Object Parameter { get; private set; }
      #endregion
   }
}
