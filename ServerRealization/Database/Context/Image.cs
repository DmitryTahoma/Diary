using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Image : IDBObject
    {
        public Image(byte[] data, int height, int width)
            : this(DBContext.Images.Max(x => x.Id) + 1, data, height, width) { }

        public Image(int id, byte[] data, int height, int width)
        {
            Id = id;
            Data = data;
            Height = height;
            Width = width;
        }

        public int Id { private set; get; }
        public byte[] Data { set; get; }
        public int Height { set; get; }
        public int Width { set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Image))
                return false;
            Image other = (Image)obj;
            if (this.Data.Length != other.Data.Length)
                return false;
            for (int i = 0; i < this.Data.Length; ++i)
                if (this.Data[i] != other.Data[i])
                    return false;
            return this.Id == other.Id
                && this.Height == other.Height
                && this.Width == other.Width;
        }
    }
}