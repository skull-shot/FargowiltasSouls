namespace FargowiltasSouls.Common.InverseKinematics
{
    public interface IJointConstraint
    {
        public double ApplyPenaltyLoss(Joint owner, float gradientDescentCompletion);
    }
}
