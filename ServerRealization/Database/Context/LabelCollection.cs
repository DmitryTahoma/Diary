using System.Linq;

namespace ServerRealization.Database.Context
{
    public class LabelCollection : IDBObject
    {
        public LabelCollection(int labelId, int stereotypeId)
            : this(DBContext.Labels.Where(x => x.Id == labelId).First(),
                  DBContext.Collections.Where(x => x.Id == stereotypeId).First()) { }

        public LabelCollection(Label label, Collection stereotype)
            : this(DBContext.LabelCollections.Max(x => x.Id) + 1, label, stereotype) { }

        public LabelCollection(int id, int labelId, int stereotypeId)
            : this(id, DBContext.Labels.Where(x => x.Id == labelId).First(),
                  DBContext.Collections.Where(x => x.Id == stereotypeId).First()) { }

        public LabelCollection(int id, Label label, Collection stereotype)
        {
            Id = id;
            Label = label;
            Stereotype = stereotype;
        }

        public int Id { private set; get; }
        public int LabelId { get => Label.Id; }
        public int StereotypeId { get => Stereotype.Id; }

        public Label Label { private set; get; }
        public Collection Stereotype { private set; get; }

        public override bool Equals(object obj)
        {
            if (!(obj is LabelCollection))
                return false;
            LabelCollection other = (LabelCollection)obj;
            return this.Id == other.Id
                && this.Label.Equals(other.Label)
                && this.Stereotype.Equals(other.Stereotype);
        }
    }
}