using System;
using System.Windows;
using System.Windows.Controls;
using NGMP.WPF;

namespace WpfHelper.Sources
{
    public partial class ResourceHelperDictionary : ResourceDictionary
    {
        /// <summary>
        ///     Registers the non-routed event to the combobox.
        ///     Since OnDropDownOpened is not a routed event, it cannot be set in the style eventsetter and has to be done in this
        ///     roundabout way.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            ((ComboBox) sender).DropDownOpened -= EventSetter_OnDropdownOpened;
            ((ComboBox) sender).DropDownOpened += EventSetter_OnDropdownOpened;
        }

        /// <summary>
        ///     Resets the info symbol that shows, that you missed a warning or error message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventSetter_OnDropdownOpened(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (combo != null)
            {
                StatusBarAccessor sba = combo.Tag as StatusBarAccessor;

                if (sba != null)
                {
                    sba.MissedStatus = String.Empty;
                }
            }
        }
    }
}