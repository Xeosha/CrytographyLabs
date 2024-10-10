using System.Numerics;

namespace Crytography.Web.Models
{
    public class Lab2Model
    {
        public BigInteger P { get; set; } = 0;
        public BigInteger Q { get; set; } = 0;
        public BigInteger N { get; set; } = 0;
        public BigInteger E { get; set; } = 0;
        public BigInteger D { get; set; } = 0;
        public BigInteger Y { get; set; } = 0;
        public Lab2Model(BigInteger p, BigInteger q, BigInteger n, BigInteger e, BigInteger d, BigInteger y)
        {
            P = p;
            Q = q;
            N = n;
            E = e;
            D = d;
            Y = y;
        }

        public Lab2Model() { }

        public override string ToString()
        {
            return $"P:{P}\n" +
                $"Q:{Q}\n" +
                $"N:{N}\n" +
                $"PubKey:{E}\n" +
                $"PrKey:{D}\n" +
                $"Y: {Y}";
        }
    }
}
