using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace Controls
{
   /// <summary>
   /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
   ///
   /// Step 1a) Using this custom control in a XAML file that exists in the current project.
   /// Add this XmlNamespace attribute to the root element of the markup file where it is 
   /// to be used:
   ///
   ///     xmlns:MyNamespace="clr-namespace:Controls"
   ///
   ///
   /// Step 1b) Using this custom control in a XAML file that exists in a different project.
   /// Add this XmlNamespace attribute to the root element of the markup file where it is 
   /// to be used:
   ///
   ///     xmlns:MyNamespace="clr-namespace:Controls;assembly=Controls"
   ///
   /// You will also need to add a project reference from the project where the XAML file lives
   /// to this project and Rebuild to avoid compilation errors:
   ///
   ///     Right click on the target project in the Solution Explorer and
   ///     "Add Reference"->"Projects"->[Select this project]
   ///
   ///
   /// Step 2)
   /// Go ahead and use your control in the XAML file.
   ///
   ///     <MyNamespace:CustomControl1/>
   ///
   /// </summary>
   [TemplatePartAttribute( Name = "PART_TimeTextBox", Type = typeof( TextBox ) )]
   [TemplatePartAttribute( Name = "PART_IncreaseTime", Type = typeof( ButtonBase ) )]
   [TemplatePartAttribute( Name = "PART_DecreaseTime", Type = typeof( ButtonBase ) )]
   public class TimeEditor : Control
   {
      #region Constructors
      static TimeEditor()
      {
         CurrentTimeProperty = DependencyProperty.Register( "CurrentTime", typeof( DateTime ), typeof( TimeEditor ),
            new FrameworkPropertyMetadata( new PropertyChangedCallback( OnCurrentTimeChanged ) ) );

         TextProperty = DependencyProperty.Register( "Text", typeof( String ), typeof( TimeEditor ),
            new FrameworkPropertyMetadata( new PropertyChangedCallback( OnTimeTextChanged ) ) );

         DefaultStyleKeyProperty.OverrideMetadata( typeof( TimeEditor ), new FrameworkPropertyMetadata( typeof( TimeEditor ) ) );
      }
      #endregion

      #region Public Interface
      public static DependencyProperty TextProperty;
      public static DependencyProperty CurrentTimeProperty;

      public String Text
      {
         get { return (String)GetValue( TextProperty ); }
         set { SetValue( TextProperty, value ); }
      }

      public DateTime CurrentTime
      {
         get { return (DateTime)GetValue( CurrentTimeProperty ); }
         set { SetValue( CurrentTimeProperty, value ); }
      }
      #endregion

      #region Event Handling
      private static void OnTimeTextChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
      {
         DateTime enteredTime;
         DateTime currentTime;
         String theText = (String)e.NewValue;
         TimeEditor timeEditor = sender as TimeEditor;

         // Grab the current time, and set the time component to midnight.  We will add the entered time
         // on to this to preserve the date component, just in case it is important.
         currentTime = timeEditor.CurrentTime;
         currentTime = currentTime.Subtract( currentTime.TimeOfDay );

         // Try to parse the entered time string.
         if (!TryParseTime( theText, out enteredTime ))
         {
            // Could not parse, reset the entered time to current.
            enteredTime = timeEditor.CurrentTime;
         }

         currentTime = currentTime.Add( enteredTime.TimeOfDay );
         timeEditor.CurrentTime = currentTime;
         timeEditor.Text = timeEditor.CurrentTime.ToShortTimeString();

         // TODO: Return the time here...
      }

      private static void OnCurrentTimeChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
      {
         DateTime currentTime = (DateTime)e.NewValue;
         TimeEditor timeEditor = sender as TimeEditor;

         timeEditor.Text = currentTime.ToShortTimeString();

         // TODO: Raise a routed event here.
      }

      private void OnIncreaseTimeClicked( Object sender, RoutedEventArgs e )
      {
         ChangeTime( 1.0 );
      }

      private void OnDecreaseTimeClicked( Object sender, RoutedEventArgs e )
      {
         ChangeTime( -1.0 );
      }
      #endregion

      #region Overrides
      public override void OnApplyTemplate()
      {
         base.OnApplyTemplate();

         _timeTextBox = GetTemplateChild( "PART_TimeTextBox" ) as TextBox;
         if (_timeTextBox != null)
         {
            Binding binding = new Binding( "Text" );
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            _timeTextBox.SetBinding( TextBox.TextProperty, binding );
         }

         ButtonBase incrementButton = GetTemplateChild( "PART_IncreaseTime" ) as ButtonBase;
         if (incrementButton != null)
         {
            incrementButton.Click += OnIncreaseTimeClicked;
         }

         ButtonBase decrementButton = GetTemplateChild( "PART_DecreaseTime" ) as ButtonBase;
         if (decrementButton != null)
         {
            decrementButton.Click += OnDecreaseTimeClicked;
         }
      }
      #endregion

      #region Private Helpers
      private void ChangeTime( Double increment )
      {
         // For now, just increment either hours or minutes.
         // Do it in a way that we do not change the date if we go past mid-night
         // NOTE: We assume a valid date here...
         if (_timeTextBox != null)
         {
            Int32 idx = _timeTextBox.CaretIndex;
            String timeText = _timeTextBox.Text;
            DateTime newTime;

            // If we cannot parse the current time, just leave, there is no point doing anything
            if (!TryParseTime( timeText, out newTime ))
            {
               return;
            }

            // Format will be either hh:mm or h:mm
            if (idx == 1 || (timeText[2] == ':' && idx == 2))
            {
               newTime = newTime.AddHours( increment );
               if (idx == 2 && newTime.ToShortTimeString()[1] == ':')
               {
                  idx = 1;
               }
            }
            else
            {
               newTime = newTime.AddMinutes( increment );
            }

            // Adjust the current time based on the new time, in a way in which the date does
            // not change.
            this.CurrentTime = this.CurrentTime.Subtract( this.CurrentTime.TimeOfDay ).Add( newTime.TimeOfDay );
            _timeTextBox.CaretIndex = idx;
         }
         else
         {
            CurrentTime = CurrentTime.AddMinutes( increment );
         }
      }

      private static Boolean TryParseTime( String theTime, out DateTime dateTime )
      {
         // Grab the entered text and parse it out, if we have one of the key words that we
         // recognize, use it.
         if (String.Equals( theTime, "now", StringComparison.CurrentCultureIgnoreCase ))
         {
            dateTime = DateTime.Now;
            return true;
         }

         if (String.Equals( theTime, "noon", StringComparison.CurrentCultureIgnoreCase ))
         {
            dateTime = DateTime.Parse( "12:00" );
            return true;
         }

         if (String.Equals( theTime, "midnight", StringComparison.CurrentCultureIgnoreCase ) ||
             String.Equals( theTime, "mid-night", StringComparison.CurrentCultureIgnoreCase ))
         {
            dateTime = DateTime.Parse( "00:00" );
            return true;
         }

         return DateTime.TryParse( theTime, out dateTime );
      }
      #endregion

      #region Private Data
      TextBox _timeTextBox;
      #endregion
   }
}
