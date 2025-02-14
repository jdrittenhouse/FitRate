using Microsoft.Maui.Controls;
using System;

namespace FitRate.Mobile
{
    public partial class App : Application
    {
        // Expose the DI container.
        public static IServiceProvider Services { get; private set; }

        public App(IServiceProvider services)
        {
            InitializeComponent();
            Services = services;
            MainPage = new AppShell();
        }
    }
}
