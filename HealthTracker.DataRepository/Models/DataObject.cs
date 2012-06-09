using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HealthTracker.Infrastructure.Properties;

namespace HealthTracker.DataRepository.Models
{
   /// <summary>
   /// Base class for all data objects in the Health Tracker system.  It contains the base
   /// data that all of the data model classes should have.
   /// 
   /// TODO: This is a candidate to move down into the RepositoryObjectBase.  I believe that now
   ///       all objects in the repository will have these attributes.  I believe that was my original
   ///       design, and I moved away from it for some reason.
   ///       
   /// After this is done, the DataRepositoryObjectViewModel class probably ought to have the MessageBox related
   /// stuf included in the usings...
   /// </summary>
   public abstract class DataObject : RepositoryObjectBase
   {
      #region Constructors
      public DataObject()
         : this( System.Guid.NewGuid(), null, null )
      {
      }

      public DataObject( Guid id, String name, String description )
         : base( id )
      {
         Name = name;
         Description = description;
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
            _name = (String.IsNullOrEmpty( value ) || value.Trim() == String.Empty) ? null : value.Trim();
         }
      }

      private String _description;
      public String Description
      {
         get
         {
            return _description;
         }
         set
         {
            _description = (String.IsNullOrEmpty( value ) || value.Trim() == String.Empty) ? null : value.Trim();
         }
      }

      /// <summary>
      /// Initialize this object using the contents of another object.  Essentially, this creates
      /// a deep copy of the source object.
      /// </summary>
      /// <param name="source">The source object for the deep copy</param>
      public override void InitializeData( RepositoryObjectBase source )
      {
         base.InitializeData( source );

         DataObject dataObject = source as DataObject;
         this.Description = dataObject.Description;
         this.Name = dataObject.Name;
      }


      /// <summary>
      /// Determine if the object is valid or not.
      /// </summary>
      /// <returns>true if the object is valid, false otherwise</returns>
      public override Boolean IsValid
      {
         get
         {
            String error = null;

            foreach (String propertyName in ValidatedProperties)
            {
               switch (propertyName)
               {
                  case propertyNameName:
                     error = ValidateName();
                     break;

                  default:
                     Debug.Fail( "Unexpected property: " + propertyName );
                     break;
               }

               if (error != null)
               {
                  return false;
               }
            }

            return base.IsValid;
         }
      }
      #endregion

      #region IDataErrorInfo Memebers
      public override String this[String PropertyName]
      {
         get
         {
            if (PropertyName == propertyNameName)
            {
               return ValidateName();
            }

            return null;
         }
      }
      #endregion

      #region Validations
      private const String propertyNameName = "Name";

      static readonly String[] ValidatedProperties = 
      {
         propertyNameName
      };

      private String ValidateName()
      {
         if (Name == null)
         {
            return Messages.Error_No_Name;
         }

         return null;
      }
      #endregion
   }
}
