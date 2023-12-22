using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using SnackAdmin.Domain;
using SnackAdmin.Dal.Interface;
using SnackAdmin.BusinessLogic;

namespace SnackAdmin.BusinessLogic.Test
{
    
    public class TestRestaurantManagementLogic
    {

        [Theory]
        [InlineData(null, null, 0, new int[] { })]
        [InlineData(48.52231449370366, 14.294476125935876, 0, new int[] {  })]
        [InlineData(48.52231449370366, 14.294476125935876, 1, new int[] { 1 })]
        [InlineData(48.52231449370366, 14.294476125935876, 50, new int[] { 1,2 })]
        [InlineData(48.52231449370366, 14.294476125935876, 10000, new int[] { 1,2,3 })]
        public async void GetAllRestaurants_withValidData_returnsRestaurants(
            double gpsLat, double gpsLong, int radius, int[] expectedIds)
        {
            // Arrange
            var restaurantDaoMock = new Mock<IRestaurantDao>();
            var addressDaoMock = new Mock<IAddressDao>();
            var deliverConditionDaoMock = new Mock<IDeliveryCondition>();
            var openingHourDaoMock = new Mock<IOpeningHourDao>();
            var restaurants = new List<Restaurant>
            {
                new Restaurant(
                    1, "Latino Bar", 1,
                    48.52231449370366, 14.294476125935876, "http://test.url",
                    null, ""),
                new Restaurant(
                    2, "Linz", 1,
                    48.30639, 14.28611, "http://test.url",
                    null, ""),
                new Restaurant(
                    3, "Null Island", 1,
                    0, 0, "http://test.url",
                    null, "")
            };

            restaurantDaoMock.Setup(dao => dao.FindAllAsync()).ReturnsAsync(restaurants);
            
            var restaurantManager = new RestaurantManagementLogic(
                restaurantDaoMock.Object, addressDaoMock.Object, 
                deliverConditionDaoMock.Object, openingHourDaoMock.Object);

            // Act
            IEnumerable<Restaurant> result = await restaurantManager.GetRestaurantsAsync(gpsLat, gpsLong, radius);

            // Assert
            foreach (var expectedId in expectedIds)
            {
                Assert.Contains(result, r => r.Id == expectedId);
            }
        }
        
    }
}
