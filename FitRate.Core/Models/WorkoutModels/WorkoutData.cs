using System;

namespace FitRate.Core.Models
{
    public class WorkoutData
    {
        // Unique identifier for the workout session
        public string WorkoutId { get; set; }

        // Reference to the user who did the workout
        public string UserId { get; set; }

        // Type of workout (e.g., "Cardio", "Strength Training")
        public string WorkoutType { get; set; }

        // Date and time of the workout
        public DateTime WorkoutDate { get; set; }

        // Duration in minutes
        public double Duration { get; set; }

        // Additional metrics (calories, distance, etc.)
        public double CaloriesBurned { get; set; }
        public double Distance { get; set; } // in kilometers or miles

        
    }
}
