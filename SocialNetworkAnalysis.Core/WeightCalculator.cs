using System;

namespace SocialNetworkAnalysis.Core
{
    /// <summary>
    /// Kenar ağırlıklarını hesaplayan yardımcı sınıf.
    /// </summary>
    public static class WeightCalculator
    {
        /// <summary>
        /// İki düğüm arasındaki benzerlik ağırlığını hesaplar.
        /// </summary>
        public static double CalculateWeight(Node n1, Node n2)
        {
            // Formula: 1 / (1 + Sqrt((Act1 - Act2)^2 + (Int1 - Int2)^2 + (Conn1 - Conn2)^2))
            
            double diffActivity = n1.Activity - n2.Activity;
            double diffInteraction = n1.Interaction - n2.Interaction;
            double diffConnection = n1.ConnectionCount - n2.ConnectionCount;

            double sumOfSquares = (diffActivity * diffActivity) + 
                                  (diffInteraction * diffInteraction) + 
                                  (diffConnection * diffConnection);

            double sqrtVal = Math.Sqrt(sumOfSquares);
            
            return 1.0 / (1.0 + sqrtVal);
        }
    }
}
