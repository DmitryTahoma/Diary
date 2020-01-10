﻿using System.Linq;

namespace ServerRealization.Database.Context
{
    public class Collection : IDBObject
    {
        public Collection()
            : this(DBContext.Collections.Max(x => x.Id) + 1, 0) { }

        public Collection(int id, int count)
        {
            Id = id;
            Count = count;
        }

        public int Id { private set; get; }
        public int Count { set; get; }
    }
}