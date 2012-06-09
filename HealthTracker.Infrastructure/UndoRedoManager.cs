using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Interfaces;

namespace HealthTracker.Infrastructure
{
   public class UndoRedoManager
   {
      #region Constructors
      public UndoRedoManager()
      {
         _undoItems = new Stack<IUndoRedo>();
         _redoItems = new Stack<IUndoRedo>();

         IsPerformingUndoOrRedo = false;
      }
      #endregion

      #region Public Interface
      public void Add<T>( T propertyUndoValue ) where T : IUndoRedo
      {
         _undoItems.Push( propertyUndoValue );
      }

      public Boolean IsPerformingUndoOrRedo { get; private set; }

      public void Undo()
      {
         IUndoRedo undoItem;

         IsPerformingUndoOrRedo = true;
         if (CanUndo)
         {
            undoItem = _undoItems.Pop();
            undoItem.Undo();
            _redoItems.Push( undoItem );
         }
         IsPerformingUndoOrRedo = false;
      }

      public Boolean CanUndo
      {
         get
         {
            return (_undoItems.Count > 0);
         }
      }

      public void Redo()
      {
         IUndoRedo redoItem;

         IsPerformingUndoOrRedo = true;
         if (CanRedo)
         {
            redoItem = _redoItems.Pop();
            redoItem.Redo();
            _undoItems.Push( redoItem );
         }
         IsPerformingUndoOrRedo = false;
      }

      public Boolean CanRedo
      {
         get
         {
            return (_redoItems.Count > 0);
         }
      }
      #endregion

      #region Private Data
      private Stack<IUndoRedo> _undoItems;
      private Stack<IUndoRedo> _redoItems;
      #endregion
   }
}
