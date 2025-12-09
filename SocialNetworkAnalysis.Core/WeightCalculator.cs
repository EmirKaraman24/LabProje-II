using System;

namespace SocialNetworkAnalysis.Core
{
    public static class WeightCalculator
    {
        public static double CalculateWeight(Node n1, Node n2)
        {
            // Formula: 1 / (1 + Sqrt((Act1 - Act2)^2 + (Int1 - Int2)^2 + (Conn1 - Conn2)^2))
            
            double diffActivity = n1.Activity - n2.Activity;
            double diffInteraction = n1.Interaction - n2.Interaction;
            double diffConnection = n1.ConnectionCountProperty - n2.ConnectionCountProperty;

            double sumOfSquares = (diffActivity * diffActivity) + 
                                  (diffInteraction * diffInteraction) + 
                                  (diffConnection * diffConnection);

            double sqrtVal = Math.Sqrt(sumOfSquares);
            
            return 1.0 / (1.0 + sqrtVal);
        }
    }
}
