using System;
using Microsoft.Maui.Controls;
using FitRate.Core.Models;
using FitRate.Core.Services;
using FitRate.Core.Models.UserModels;

namespace FitRate.Mobile.Components.Profile
{
    [QueryProperty(nameof(UserId), "userId")]
    [QueryProperty(nameof(Email), "email")]
    [QueryProperty(nameof(IsUpdate),"isUpdate")]
    public partial class UserProfilePage : ContentPage
    {
        // Assume you have a FirebaseDataService instance injected via DI or retrieved from App.Services
        private readonly FirebaseDataService _dataService;
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsUpdate { get; set; }
        public UserProfilePage()
        {
            InitializeComponent();
            _dataService = App.Services.GetService<FirebaseDataService>();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Pre-fill the form fields with the passed-in data
            EmailEntry.Text = Email;
            // You can store the UserId for later saving.
        }
        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            // Basic input validation
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
                string.IsNullOrWhiteSpace(AgeEntry.Text))
            {
                await DisplayAlert("Error", "Please fill in all required fields.", "OK");
                return;
            }

            // Construct a user profile from the entered data
            var profile = new UserProfile
            {
                UserId = UserId, 
                Email = Email,
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text,
                Age = int.Parse(AgeEntry.Text),
                Gender = GenderPicker.SelectedItem?.ToString(),
                Height = double.TryParse(HeightEntry.Text, out double h) ? h : 0,
                Weight = double.TryParse(WeightEntry.Text, out double w) ? w : 0,
                CreatedOn = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
            };

            // Map the selected goal from the picker to your Goal model
            // This example assumes the order in the picker matches your enum order, or you can map explicitly.
            if (GoalPicker.SelectedIndex >= 0)
            {
                var goalName = GoalPicker.Items[GoalPicker.SelectedIndex];
                // Create a new Goal with a default priority (you might later let the user rank multiple goals)
                profile.Goals.Add(new Goal
                {
                    Type = MapGoalNameToEnum(goalName),
                    Priority = 1
                });
            }

            // Save the profile to Firebase
            var success = await _dataService.SaveUserProfileAsync(profile,IsUpdate);
            if (success)
            {
                // Navigate to the main/dashboard page upon successful save
                await Shell.Current.GoToAsync("//MainDashboard");
            }
            else
            {
                await DisplayAlert("Error", "Unable to save profile. Please try again.", "OK");
            }
        }

        // Helper method to map the selected goal string to the GoalType enum.
        private GoalType MapGoalNameToEnum(string goalName)
        {
            return goalName switch
            {
                "Lose Weight" => GoalType.LoseWeight,
                "Gain Strength" => GoalType.GainStrength,
                "Long Distance Training" => GoalType.EnduranceTraining,
                "Maintain Weight" => GoalType.MaintainWeight,
                "Increase Vertical" => GoalType.IncreaseVertical,
                _ => GoalType.MaintainWeight,
            };
        }
    }
}
