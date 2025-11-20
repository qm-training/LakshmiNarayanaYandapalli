namespace WelfareWorkTracker.Tests.Controllers;
public class JobsControllerTests
{
    private readonly JobsController _controller;

    public JobsControllerTests()
    {
        GlobalConfiguration.Configuration.UseMemoryStorage();
        JobStorage.Current = new MemoryStorage();

        _controller = new JobsController();
    }

    [Fact]
    public void StartJobAssignDailyComplaints_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StartJobAssignDailyComplaints();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job scheduled to run daily at 12:00 AM.", ok.Value);
    }

    [Fact]
    public void StopJobAssignDailyComplaints_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StopJobAssignDailyComplaints();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job has been stopped.", ok.Value);
    }

    [Fact]
    public void StartJobEvaluateDailyComplaintFeedback_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StartJobEvaluateDailyComplaintFeedback();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job scheduled to run daily at 12:00 AM.", ok.Value);
    }

    [Fact]
    public void StopJobEvaluateDailyComplaintFeedback_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StopJobEvaluateDailyComplaintFeedback();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job has been stopped.", ok.Value);
    }

    [Fact]
    public void StartJobCheckDailyComplaintStatus_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StartJobCheckDailyComplaintStatus();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job scheduled to run daily at 12:00 PM.", ok.Value);
    }

    [Fact]
    public void StopJobCheckDailyComplaintStatus_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StopJobCheckDailyComplaintStatus();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job has been stopped.", ok.Value);
    }

    [Fact]
    public void StartJobCheckComplaintsForLeaderApproval_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StartJobCheckComplaintsForLeaderApproval();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job scheduled to run every 6 hrs.", ok.Value);
    }

    [Fact]
    public void StopJobCheckComplaintsForLeaderApproval_WithNoInput_ReturnsOkWithMessage()
    {
        // Arrange

        // Act
        var result = _controller.StopJobCheckComplaintsForLeaderApproval();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Job has been stopped.", ok.Value);
    }
}
