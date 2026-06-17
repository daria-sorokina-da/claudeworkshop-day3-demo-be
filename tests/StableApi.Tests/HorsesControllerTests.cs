namespace StableApi.Tests;

using FluentValidation;
using FluentValidation.Results;
using StableApi.Controllers;
using StableApi.Models;
using StableApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class HorsesControllerTests
{
    private readonly Mock<IHorseService> _service = new();
    private readonly Mock<IValidator<CreateHorseRequest>> _createValidator = new();
    private readonly Mock<IValidator<UpdateHorseRequest>> _updateValidator = new();
    private readonly Mock<IValidator<RetireHorseRequest>> _retireValidator = new();
    private readonly HorsesController _controller;

    public HorsesControllerTests()
    {
        _controller = new HorsesController(_service.Object, _createValidator.Object, _updateValidator.Object, _retireValidator.Object);
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
    public void Create_ValidRequest_ReturnsCreated()
    {
        var request = new CreateHorseRequest("Moonbeam", "moonbeam@enchantedstables.com", "Thoroughbred");
        var horse = new Horse { Id = 1, Name = "Moonbeam" };
        _createValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
        _service.Setup(s => s.Create(request)).Returns(horse);

        var result = _controller.Create(request);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public void Create_ShortName_ReturnsBadRequest()
    {
        var request = new CreateHorseRequest("X", "x@enchantedstables.com", "Arabian");
        _createValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult(
            [new ValidationFailure("Name", "Name must not be empty.")]));

        var result = _controller.Create(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public void Create_InvalidEmail_ReturnsBadRequest()
    {
        var request = new CreateHorseRequest("Moonbeam", "not-an-email", "Thoroughbred");
        _createValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult(
            [new ValidationFailure("OwnerEmail", "OwnerEmail is not a valid email address.")]));

        var result = _controller.Create(request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public void Update_ExistingHorse_ReturnsOk()
    {
        var request = new UpdateHorseRequest("Moonbeam II", "moonbeam@enchantedstables.com", "Thoroughbred");
        var horse = new Horse { Id = 1, Name = "Moonbeam II" };
        _updateValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
        _service.Setup(s => s.Update(1, request)).Returns(horse);

        var result = _controller.Update(1, request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Update_NonExistentHorse_ReturnsNotFound()
    {
        var request = new UpdateHorseRequest("X", "x@enchantedstables.com", "Shetland");
        _updateValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
        _service.Setup(s => s.Update(99, request)).Returns((Horse?)null);

        var result = _controller.Update(99, request);

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

    [Fact]
    public void Retire_ValidRequest_ReturnsCreated()
    {
        var request = new RetireHorseRequest("Retired due to age");
        _retireValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
        _service.Setup(s => s.GetById(1)).Returns(new Horse { Id = 1, Name = "Moonbeam" });

        var result = _controller.Retire(1, request);

        Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
    }

    [Fact]
    public void Retire_ShortReason_ReturnsBadRequest()
    {
        var request = new RetireHorseRequest("No");
        _retireValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult(
            [new ValidationFailure("Reason", "The length of 'Reason' must be at least 5 characters.")]));

        var result = _controller.Retire(1, request);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public void Retire_NonExistentHorse_ReturnsNotFound()
    {
        var request = new RetireHorseRequest("Retired due to age");
        _retireValidator.Setup(v => v.Validate(request)).Returns(new ValidationResult());
        _service.Setup(s => s.GetById(99)).Returns((Horse?)null);

        var result = _controller.Retire(99, request);

        Assert.IsType<NotFoundResult>(result);
    }
}
