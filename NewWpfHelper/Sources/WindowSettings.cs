using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Automation;


namespace NGMP.WPF
{
    /// <summary>
    /// Persists a Window’s Size, Location and WindowState to UserScopeSettings
    /// </summary>
    public class WindowSettings
    {
        #region WindowApplicationSettings Helper Class
        public class WindowApplicationSettings : ApplicationSettingsBase
        {
            public WindowApplicationSettings(WindowSettings windowSettings)
                : base(AutomationProperties.GetAutomationId(windowSettings._window))
            {
            }

            [UserScopedSetting]
            public Rect Location
            {
                get
                {
                    if (this["Location"] != null)
                    {
                        return ((Rect)this["Location"]);
                    }
                    return Rect.Empty;
                }
                set
                {
                    this["Location"] = value;
                }
            }

            [UserScopedSetting]
            public WindowState WindowState
            {
                get
                {
                    if (this["WindowState"] != null)
                    {
                        return (WindowState)this["WindowState"];
                    }
                    return WindowState.Normal;
                }
                set
                {
                    this["WindowState"] = value;
                }
            }

        }
        #endregion

        #region Constructor
        private readonly Window _window;

        public WindowSettings(Window window)
        {
            this._window = window;
        }

        #endregion

        #region Attached “Save” Property Implementation
        /// <summary>
        /// Register the “Save” attached property and the “OnSaveInvalidated” callback
        /// </summary>
        public static readonly DependencyProperty SaveProperty
        = DependencyProperty.RegisterAttached("Save", typeof(bool), typeof(WindowSettings),
        new FrameworkPropertyMetadata(OnSaveInvalidated));

        public static void SetSave(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(SaveProperty, enabled);
        }

        /// <summary>
        /// Called when Save is changed on an object.
        /// </summary>
        private static void OnSaveInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Window window = dependencyObject as Window;
            if (window != null)
            {
                if ((bool)e.NewValue)
                {
                    WindowSettings settings = new WindowSettings(window);
                    settings.Attach();
                }
            }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// Load the Window Size Location and State from the settings object
        /// </summary>
        protected virtual void LoadWindowState()
        {
            this.Settings.Reload();
            if (this.Settings.Location != Rect.Empty)
            {
                this._window.Left = this.Settings.Location.Left;
                this._window.Top = this.Settings.Location.Top;
                this._window.Width = this.Settings.Location.Width;
                this._window.Height = this.Settings.Location.Height;
            }

            if (this.Settings.WindowState != WindowState.Maximized)
            {
                this._window.WindowState = this.Settings.WindowState;
            }
        }

        /// <summary>
        /// Save the Window Size, Location and State to the settings object
        /// </summary>
        protected virtual void SaveWindowState()
        {
            this.Settings.WindowState = this._window.WindowState;
            this.Settings.Location = this._window.RestoreBounds;
            this.Settings.Save();
        }
        #endregion

        #region Private Methods

        private void Attach()
        {
            if (this._window != null)
            {
                this._window.Closing += window_Closing;
                this._window.Initialized += window_Initialized;
                this._window.Loaded += window_Loaded;
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Settings.WindowState == WindowState.Maximized)
            {
                this._window.WindowState = this.Settings.WindowState;
            }
        }

        private void window_Initialized(object sender, EventArgs e)
        {
            LoadWindowState();
        }

        private void window_Closing(object sender, CancelEventArgs e)
        {
            SaveWindowState();
        }
        #endregion

        #region Settings Property Implementation
        private WindowApplicationSettings _windowApplicationSettings;

        protected virtual WindowApplicationSettings CreateWindowApplicationSettingsInstance()
        {
            return new WindowApplicationSettings(this);
        }

        [Browsable(false)]
        public WindowApplicationSettings Settings
        {
            get
            {
                if (_windowApplicationSettings == null)
                {
                    this._windowApplicationSettings = CreateWindowApplicationSettingsInstance();
                }
                return this._windowApplicationSettings;
            }
        }
        #endregion
    }
}