using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;

namespace Offers.Tests.Middleware
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _mockLogger;
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _mockNext = new Mock<RequestDelegate>();
            _middleware = new GlobalExceptionMiddleware(_mockNext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoException_CallsNextMiddleware()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _mockNext.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            _mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReturnsJsonResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = new InvalidOperationException("Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(500);
            context.Response.ContentType.Should().Be("application/json");
            
            // Verify logging
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled exception occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionThrown_ReturnsProperJsonStructure()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            var exception = new ArgumentException("Test argument exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            
            responseText.Should().NotBeEmpty();
            responseText.Should().Contain("StatusCode");
            responseText.Should().Contain("Message");
            responseText.Should().Contain("An unexpected error occurred");
        }

        [Theory]
        [InlineData(typeof(ArgumentNullException))]
        [InlineData(typeof(InvalidOperationException))]
        [InlineData(typeof(NotSupportedException))]
        public async Task InvokeAsync_WithDifferentExceptionTypes_HandlesAllCorrectly(Type exceptionType)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            var exception = (Exception)Activator.CreateInstance(exceptionType, "Test exception");
            _mockNext.Setup(x => x(It.IsAny<HttpContext>())).ThrowsAsync(exception);

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be(500);
            context.Response.ContentType.Should().Be("application/json");
            
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
