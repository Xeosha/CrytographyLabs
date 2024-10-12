namespace Crytography.Web.Services.Lab6Services
{
    public interface ICoder
    {
        public byte[] Decode(byte[] input);
        public byte[] Encode(byte[] input);
    }
}
