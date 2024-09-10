using System.Numerics;

namespace Crytography.Web.Models
{
    public class Lab3Model
    {
        public BigInteger N { get; set; } = 0;
        public BigInteger Q { get; set; } = 0;
        public BigInteger X { get; set; } = 0;
        public BigInteger Y { get; set; } = 0;
        public BigInteger Kx { get; set; } = 0;
        public BigInteger Ky { get; set; } = 0;

        public BigInteger A { get; set; } = 0;
        public BigInteger B { get; set; } = 0;

        public Lab3Model() { }  
        public Lab3Model(BigInteger n, BigInteger q, BigInteger x, BigInteger y, BigInteger kx, BigInteger ky, BigInteger a, BigInteger b)
        {
            N = n;
            Q = q;
            X = x;
            Y = y;
            Kx = kx;
            Ky = ky;
            A = a;
            B = b;
        }

        public override string ToString()
        {
            return $"N:{N}\n" +
                $"Q:{Q}\n" +
                $"X:{X}\n" +
                $"Y:{Y}\n" +
                $"Kx:{Kx}\n" +
                $"Ky: {Ky}\n" +
                $"A: {A}\n" +
                $"B: {B}";
        }
    }
}
