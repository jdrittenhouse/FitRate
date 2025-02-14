using System;
using System.Collections.Generic;

namespace FitRate.Core.Models.UserModels
{
    public class UserProfile
    {
        // Unique identifier for the user (from Firebase)
        public string UserId { get; set; }

        // Basic profile information
        public string Email { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Units { get; set; } = "imperial";//metric or imperial
        public int Age { get; set; }
        public string Gender { get; set; } // Consider using an enum for Gender if needed

        // Physical attributes (if you plan to use them)
        public double Height { get; set; } // in centimeters or inches – document the unit
        public double Weight { get; set; } // in kilograms or pounds – document the unit

        // Goals and preferences
        public List<Goal> Goals { get; set; } = new List<Goal>();

        // Timestamp for when the profile was created/last updated
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
