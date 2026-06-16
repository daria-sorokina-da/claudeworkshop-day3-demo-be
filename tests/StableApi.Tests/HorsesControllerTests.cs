namespace StableApi.Tests;

using StableApi.Controllers;
using StableApi.Models;
using StableApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class HorsesControllerTests
{
    private readonly Mock<IHorseService> _service = new();
    private readonly HorsesController _controller;

    public HorsesControllerTests()
    {
        _controller = new HorsesController(_service.Object);
    }

    [Fact]
    public void GetById_ExistingHorse_ReturnsOk()
    {
        var horse = new Horse { Id = 1, Name = "Moonbeam" };
        _service.Setup(s => s.GetById(1)).Returns(horse);

        var result = _controller.GetById(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Create_ValidRequest_ReturnsOk()
    {
        var request = new CreateHorseRequest("Moonbeam", "moonbeam@enchantedstables.com", "Thoroughbred");
        var horse = new Horse { Id = 1, Name = "Moonbeam" };
        _service.Setup(s => s.Create(request)).Returns(horse);

        var result = _controller.Create(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Create_ShortName_ReturnsBadRequest()
    {
        var result = _controller.Create(new CreateHorseRequest("X", "x@enchantedstables.com", "Arabian"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsBadRequest()
    {
        var result = _controller.Create(new CreateHorseRequest("Moonbeam", "not-an-email", "Thoroughbred"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Update_ExistingHorse_ReturnsOk()
    {
        var request = new UpdateHorseRequest("Moonbeam II", "moonbeam@enchantedstables.com", "Thoroughbred");
        var horse = new Horse { Id = 1, Name = "Moonbeam II" };
        _service.Setup(s => s.Update(1, request)).Returns(horse);

        var result = _controller.Update(1, request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Update_NonExistentHorse_ReturnsNotFound()
    {
        _service.Setup(s => s.Update(99, It.IsAny<UpdateHorseRequest>())).Returns((Horse?)null);

        var result = _controller.Update(99, new UpdateHorseRequest("X", "x@enchantedstables.com", "Shetland"));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ExistingHorse_ReturnsOk()
    {
        _service.Setup(s => s.Delete(1)).Returns(true);

        var result = _controller.Delete(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void Delete_NonExistentHorse_ReturnsNotFound()
    {
        _service.Setup(s => s.Delete(99)).Returns(false);

        var result = _controller.Delete(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetAll_InvalidPage_ReturnsBadRequest()
    {
        var result = _controller.GetAll(page: 0);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
