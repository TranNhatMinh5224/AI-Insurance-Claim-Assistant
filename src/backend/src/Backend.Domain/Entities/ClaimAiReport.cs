using Backend.Domain.Enums;

namespace Backend.Domain.Entities;

public sealed class ClaimAiReport
{
    private ClaimAiReport() { }

    public Guid Id { get; private set; }
    public Guid ClaimRequestId { get; private set; }
    public string FraudAnalysis { get; private set; } = string.Empty;
    public string DamageAnalysis { get; private set; } = string.Empty;
    public string LogicAnalysis { get; private set; } = string.Empty;
    public string PolicyMatched { get; private set; } = string.Empty;
    public SuggestedStatus SuggestedStatus { get; private set; }
    public string FinalReportSummary { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static ClaimAiReport Create(
        Guid claimId,
        string fraudAnalysis,
        string damageAnalysis,
        string logicAnalysis,
        string policyMatched,
        SuggestedStatus suggestedStatus,
        string finalSummary)
    {
        return new ClaimAiReport
        {
            Id = Guid.NewGuid(),
            ClaimRequestId = claimId,
            FraudAnalysis = fraudAnalysis,
            DamageAnalysis = damageAnalysis,
            LogicAnalysis = logicAnalysis,
            PolicyMatched = policyMatched,
            SuggestedStatus = suggestedStatus,
            FinalReportSummary = finalSummary,
            CreatedAt = DateTime.UtcNow
        };
    }
}
