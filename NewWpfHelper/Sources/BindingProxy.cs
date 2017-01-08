using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfHelper.Sources
{
    /// <summary>
    /// Copied from this blog: http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
    /// 
    /// It enables certain WPF objects (like a context menu item) to bind against the data context of the containing element, 
    /// even though the element itself is not part of the visual or logical tree.
    /// 
    /// It can be used like this:
    /// <ContextMenu.Resources>
    ///    <BindingProxy x:Key="proxy" Data="{Binding}" />
    /// </ContextMenu.Resources>
    /// 
    /// <ContextMenuItem Header="Price" Command="{Binding DeleteVariantCommand, Source={StaticResource proxy}}"/>
    /// </summary>
    public class BindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
