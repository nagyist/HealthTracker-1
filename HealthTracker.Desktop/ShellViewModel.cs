using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using HealthTracker.Administration.ViewModels;
using HealthTracker.DataRepository.Interfaces;
using Microsoft.Practices.Prism.ViewModel;

namespace HealthTracker.Desktop
{
   [Export]
   class ShellViewModel : NotificationObject
   {
      [ImportingConstructor]
      public ShellViewModel()
         : base()
      {
      }
   }
}
