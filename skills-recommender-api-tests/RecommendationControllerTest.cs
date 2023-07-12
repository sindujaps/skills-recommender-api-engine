using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using skills_recommender_api.Controllers;
using skills_recommender_api.enums;
using System.Net;

namespace skills_recommender_api.Tests
{
    public class RecommendationControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IHttpClientWrapper> _httpClientWrapperMock;
        private readonly RecommendationController _recommendationController;

        public RecommendationControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(x => x["LEARN_NLP_ENGINE_URL"]).Returns("include the NLP Engine URL");

            _httpClientWrapperMock = new Mock<IHttpClientWrapper>();

            _recommendationController = new RecommendationController(_configurationMock.Object, _httpClientWrapperMock.Object);
        }

        [Fact]
        //returns ok result with valid sources
        public async Task ReturnsOk()
        {
            var query = new RecommendationQuery
            {
                Sources = new[] { (int)Sources.Learn }
            };

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            httpResponseMessage.Content = new StringContent("Learn NLP Engine Response");

            _httpClientWrapperMock
    .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
    .ReturnsAsync(httpResponseMessage);

            var result = await _recommendationController.Post(query);
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(new { message = "Recommendation request forwarded successfully" }.ToString(), okResult.Value.ToString());

        }

        [Fact]
        // returms internal server error with Sources = Learn failure
        public async Task ReturnsErrorWithLearnSourcesFailure()
        {
            var query = new RecommendationQuery
            {
                Sources = new[] { (int)Sources.Learn }
            };

            _httpClientWrapperMock
    .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
    .ThrowsAsync(new Exception("Some error occurred"));

            var result = await _recommendationController.Post(query);
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            Assert.Equal(new { error = "Internal server error" }.ToString(), objectResult.Value.ToString());

        }

        [Fact]
        // returns bad request with empty sources
        public async Task ReturnsBadRequest()
        {
            var query = new RecommendationQuery
            {
                Sources = new int[0]
            };

            var result = await _recommendationController.Post(query);
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal(new { error = "Sources array is empty or null" }.ToString(), badRequestResult.Value.ToString());
        }

        
    }
}
