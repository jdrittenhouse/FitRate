using Microsoft.Maui.Controls;
using FitRate.Mobile.Components.Auth;
using FitRate.Mobile.Components.Profile;
using FitRate.Mobile.Components.Dashboard;
//using FitRate.Core.Models.UserModels;

namespace FitRate.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register routes for navigation.
            Routing.RegisterRoute("Login", typeof(Login));
            Routing.RegisterRoute("Signup", typeof(Signup));
            Routing.RegisterRoute("UserProfile", typeof(UserProfilePage));
            Routing.RegisterRoute("MainDashboard", typeof(MainDashboard));
        }
    }
}
