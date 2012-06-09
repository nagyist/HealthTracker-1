using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthTracker.Infrastructure.Interfaces
{
   public interface IUndoRedo
   {
      /// <summary>
      /// An optional name.
      /// </summary>
      String Name { get; }

      /// <summary>
      /// The Undo operation code.
      /// </summary>
      void Undo();

      /// <summary>
      /// The Redo operation code.
      /// </summary>
      void Redo();
   }
}
