using OcenaPracownicza.API.Enums;
using OcenaPracownicza.API.Exceptions.BaseExceptions;

namespace OcenaPracownicza.API.Services;

public static class Stage2TransitionValidator
{
    public static void EnsureCanReview(EvaluationStageStatus currentStatus)
    {
        if (currentStatus != EvaluationStageStatus.PendingStage2)
        {
            throw new ForbiddenException("Rekord nie oczekuje na weryfikację etapu 2.");
        }
    }

    public static void EnsureRejectComment(string? comment)
    {
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ForbiddenException("Komentarz jest wymagany przy odrzuceniu.");
        }
    }
}
