using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerRealization.Database;
using ServerRealization.Database.Context;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ServerRealization.Test
{
    [TestClass]
    public class DBLoaderTest
    {
        /*
        Before running test, I have next values ​​in the tables of the test database
------- table 'users' ----------|--------------|-----------|---------------------|---------|-------------
        id | name               | login        | password  | registration        |         |
        1  | Alex 	            | Alex92 	   | pass1234  | 2020-01-10 21:33:11 |         |
------- table 'collections' ----|--------------|-----------|---------------------|---------|-------------
        id | count              |              |           |                     |         |
        1  | 1 	                |              |           |                     |         |
------- table 'images' ---------|--------------|-----------|---------------------|---------|-------------
        id | data               | height       | width     |                     |         |
        1  | [BLOB - 120 Bytes] | 16           | 16        |                     |         |
------- table 'labels' ---------|--------------|-----------|---------------------|---------|-------------
        id | imageId            | name         |           |                     |         |
        1  | 1                  | Red          |           |                     |         |
------- table 'labelCollections'|--------------|-----------|---------------------|---------|-------------
        id | labelId            | stereotypeId |           |                     |         |
        1  | 1                  | 1            |           |                     |         |
------- table 'notes' ----------|--------------|-----------|---------------------|---------|-------------
        id | userId             | stereotypeId | name      | text                | created | lastChanged 
        1  | 1                  | 1            | Note1     | Test note           | 2020-01-10 21:58:54
        2  | 1                  | 1            | MyNote    | Do smth             | 2020-01-10 21:59:32
        */
        [DataTestMethod]
        [DataRow("images")]
        [DataRow("progresses")]
        [DataRow("collections")]
        [DataRow("users")]
        [DataRow("labels")]
        [DataRow("points")]
        [DataRow("notes")]
        [DataRow("progressnotes")]
        [DataRow("actions")]
        [DataRow("missions")]
        [DataRow("labelcollections")]
        public void LoadTest(string tableName)
        {
            DBLoader loader = new DBLoader();
            object result = loader.Load(tableName);
            IList expectedResult = (IList)CreateExpectedResultForLoadTest(tableName);
            IList ilResult = (IList)result;

            Assert.AreEqual(expectedResult.Count, ilResult.Count);
            for(int i = 0; i < ilResult.Count; ++i)
            {
                Assert.AreEqual(expectedResult[i].GetType(), ilResult[i].GetType());
                Assert.AreEqual(expectedResult[i], ilResult[i]);
            }
        }

        private object CreateExpectedResultForLoadTest(string tablename)
        {
            byte[] imageBytes = null;
            if(tablename == "images")
            {
                System.Drawing.Image image = new System.Drawing.Bitmap(@"D:\Projects\Portfolio\Diary\ServerRealization.Test\bin\Debug\Red.png");

                using(MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }
            }

            switch(tablename)
            {
                case "missions":      return new List<Mission>();
                case "progresses":    return new List<Progress>();
                case "progressnotes": return new List<ProgressNote>();
                case "points":        return new List<Point>();
                case "actions":       return new List<Action>();
                case "images":            return new List<Image>()      {       new Image(1, imageBytes, 16, 16)};   
                case "collections":       return new List<Collection>() {       new Collection(1, 1)};  
                case "users":             return new List<User>()       {       new User(1, "Alex", "Alex92", "pass1234", new System.DateTime(2020,1,10,21,33,11))};
                case "labels":            return new List<Label>()      {       new Label(1, 1, "Red") };  
                case "notes":             return new List<Note>()       {       new Note(1, 1, 1, "Note1", "Test note", new System.DateTime(2020, 1, 10, 21, 58, 54), new System.DateTime(2020, 1, 10, 21, 58, 54)),
                                                                                new Note(2, 1, 1, "MyNote", "Do smth", new System.DateTime(2020, 1, 10, 21, 59, 32), new System.DateTime(2020, 1, 10, 21, 59, 32)) };  
                case "labelcollections":  return new List<LabelCollection>() {  new LabelCollection(1,1,1)};   
            }
            return null;
        }
    }
}