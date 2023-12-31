using Moq;
using System.Net;
using FluentAssertions;
using Moq.Protected;
using ApiGateway.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Tests;
public class GatewayControllerTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly GatewayController _controller;

    public GatewayControllerTests()
{
    _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
    mockHttpMessageHandler.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("Test response")
        });

    var client = new HttpClient(mockHttpMessageHandler.Object)
    {
        BaseAddress = new Uri("http://localhost/") // Setting a base address here
    };
    _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

    _controller = new GatewayController(_mockHttpClientFactory.Object);
}

    [Fact]
    public async Task AuthAgentTest_ReturnsOkResponse()
    {
        var result = await _controller.AuthAgentTest();

        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Should().Be("Test response");
    }
}
