namespace DataProcessing.Tests;

using Xunit;
using Microsoft.AspNetCore.Mvc;
using DataProcessing.Controllers;
using FluentAssertions;

public class DataProcessingControllerTests
{
    [Fact]
    public void Get_ReturnsOkWithMessage()
    {
        var controller = new DataProcessing();

        var result = controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Should().Be("DataProcessing Service hit");
    } 
}
