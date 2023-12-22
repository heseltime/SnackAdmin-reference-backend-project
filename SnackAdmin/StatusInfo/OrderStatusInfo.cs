using Microsoft.AspNetCore.Mvc;
using SnackAdmin.Domain;
using SnackAdmin.Dtos;

namespace SnackAdmin.StatusInfo
{
    public static class OrderStatusInfo
    {
        public static ProblemDetails AddressIsNull()
        {
            return new ProblemDetails
            {
                Title = "Invalid address",
                Detail = $"Address must not be null"
            };
        }
        public static ProblemDetails RestaurantIsNull()
        {
            return new ProblemDetails
            {
                Title = "Invalid address",
                Detail = $"Restaurant must not be null"
            };
        }
        public static ProblemDetails ItemListIsNull()
        {
            return new ProblemDetails
            {
                Title = "Invalid items",
                Detail = $"List of items must not be null"
            };
        }
        public static ProblemDetails ItemListIsEmpty()
        {
            return new ProblemDetails
            {
                Title = "Invalid items",
                Detail = $"List of items must not be empty"
            };
        }
        public static ProblemDetails OrderNotFound(Guid orderId)
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"Order with ID {orderId.ToString()}, not found in DB"
            };
        }
        public static ProblemDetails RestaurantNotFound(int restaurantId)
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"Restaurant with ID {restaurantId}, not found in DB"
            };
        }
        public static ProblemDetails RestaurantHasNotOpened(int restaurantId)
        {
            return new ProblemDetails
            {
                Title = "Not open",
                Detail = $"Restaurant with ID {restaurantId} is closed at the moment"
            };
        }
        public static ProblemDetails OneOrMoreItemsNotFound()
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"One or more items not found in DB"
            };
        }
        public static ProblemDetails MatchingDeliveryConditionNotFound()
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"Matching delivery condition not found in DB"
            };
        }
        public static ProblemDetails TotalCostsBelowMinOrderValue()
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"Total costs less than the minimum order value"
            };
        }
        public static ProblemDetails DefaultCreationFailure()
        {
            return new ProblemDetails
            {
                Title = "Not found",
                Detail = $"Order could not be created"
            };
        }

    }
}
