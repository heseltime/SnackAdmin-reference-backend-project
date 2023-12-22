using Microsoft.AspNetCore.Mvc;

namespace SnackAdmin.StatusInfo
{
    public static class RestaurantStatusInfo
    {
        public static ProblemDetails InvalidLatitudeValue(double latitude, double min, double max)
        {
            return new ProblemDetails
            {
                Title = "Invalid latitude",
                Detail = $"Latitude {latitude} must be between {min} and {max}"
            };
        }

        public static ProblemDetails InvalidLongitudeValue(double longitude, double min, double max)
        {
            return new ProblemDetails
            {
                Title = "Invalid longitude",
                Detail = $"Longitude {longitude} must be between {min} and {max}"
            };
        }

        public static ProblemDetails InvalidRadiusValue(double radius, double min, double max)
        {
            return new ProblemDetails
            {
                Title = "Invalid radius",
                Detail = $"Radius {radius} must be between {min} and {max}"
            };
        }

    }
}
