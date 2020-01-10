using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Label : IDBObject
    {
        public Label(int imageId, string name)
            : this(DBContext.Images.Where(x => x.Id == imageId).First(), name) { }

        public Label(Image image, string name)
            : this(DBContext.Labels.Max(x => x.Id) + 1, image, name) { }

        public Label(int id, int imageId, string name)
            : this(id, DBContext.Images.Where(x => x.Id == imageId).First(), name) { }

        public Label(int id, Image image, string name)
        {
            Id = id;
            Image = image;
            Name = name;
        }

        public int Id { private set; get; }
        public int ImageId { get => Image.Id; }
        public string Name { set; get; }

        public Image Image { private set; get; }
    }
}