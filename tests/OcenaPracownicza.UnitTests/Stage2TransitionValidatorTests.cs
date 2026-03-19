using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Exceptions.BaseExceptions;
using OcenaPracownicza.API.Services;

namespace OcenaPracownicza.UnitTests;

public class Stage2TransitionValidatorTests
{
    [Fact]
    public void EnsureCanReview_AllowsPendingStage2()
    {
        Stage2TransitionValidator.EnsureCanReview(EvaluationStageStatus.PendingStage2);
    }

    [Fact]
    public void EnsureCanReview_ThrowsForApproved()
    {
        Assert.Throws<ForbiddenException>(() =>
            Stage2TransitionValidator.EnsureCanReview(EvaluationStageStatus.Stage2Approved));
    }

    [Fact]
    public void EnsureRejectComment_ThrowsWhenEmpty()
    {
        Assert.Throws<ForbiddenException>(() =>
            Stage2TransitionValidator.EnsureRejectComment(" "));
    }
}
