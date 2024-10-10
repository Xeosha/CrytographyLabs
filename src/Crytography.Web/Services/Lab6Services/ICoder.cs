namespace Crytography.Web.Services.Lab6Services
{
    public interface ICoder
    {
        public string Decode(byte[] input);
        public byte[] Encode(string input);
    }
}
