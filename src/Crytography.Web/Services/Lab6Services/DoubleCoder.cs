using System.Text;

namespace Crytography.Web.Services.Lab6Services
{
    public class DoubleCoder : ICoder
    {
        private ICoder _arithmeticCoder, _Lz77Coder;

        public DoubleCoder(ICoder arithmeticCode, ICoder Lz77Coder)
        {
            _arithmeticCoder = arithmeticCode;
            _Lz77Coder = Lz77Coder;
        }


        public byte[] Encode(byte[] input)
        {
            var res1 = _arithmeticCoder.Encode(input);

            var res2 = _Lz77Coder.Encode(res1);

            return res2;
        }

        public byte[] Decode(byte[] input)
        {
            var res2 = _Lz77Coder.Decode(input);

            var res1 = _arithmeticCoder.Decode(res2);

            return res1;
        }

      
    }
}
