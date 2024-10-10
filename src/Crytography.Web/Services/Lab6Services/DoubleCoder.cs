using System.Text;

namespace Crytography.Web.Services.Lab6Services
{
    public class DoubleCoder : ICoder
    {
        private int _inputLenght;
        public DoubleCoder(int inputLenght)
        {
            _inputLenght = inputLenght;
            _arithmeticCoder = new ArithmeticCoder(_inputLenght);
            _Lz77Coder = new Lz77Coder();
        }


        public byte[] Encode(string input)
        {
            var res1 = _arithmeticCoder.Encode(input);

            var binary = GlobalService.binaryToString(res1);

            var res2 = _Lz77Coder.Encode(binary);

            return res2;
        }

        public string Decode(byte[] input)
        {
            var res2 = _Lz77Coder.Decode(input);

            var str = GlobalService.binaryStringToByteArray(res2);

            var res1 = _arithmeticCoder.Decode(str);

            return res1;
        }

      
    }
}
