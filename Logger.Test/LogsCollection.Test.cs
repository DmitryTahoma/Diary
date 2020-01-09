using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logger.Test
{
    [TestClass]
    public class LogsCollectionTest
    {
        [TestMethod]
        public void LogsCollectionIsCollection()
        {
            LogsCollection logsCollection = new LogsCollection();
            logsCollection.Add(new Entry("log1"));
            logsCollection.Add(new Entry("log2", new DateTime(2019, 6, 7, 5, 0, 32)));

            Assert.AreEqual(2, logsCollection.Count());
        }

        [TestMethod]
        public void EnumerationCollection()
        {
            LogsCollection logsCollection = new LogsCollection();
            logsCollection.Add(new Entry("log..."));
            logsCollection.Add(new Entry("log.."));
            logsCollection.Add(new Entry("log."));
            logsCollection.Add(new Entry("log"));

            Assert.AreEqual("log...", logsCollection[0].Text);
            Assert.AreEqual("log..", logsCollection[1].Text);
            Assert.AreEqual("log.", logsCollection[2].Text);
            Assert.AreEqual("log", logsCollection[3].Text);
        }

        [TestMethod]
        public void LogsCollectionCountOfContains()
        {
            LogsCollection logsCollection = new LogsCollection();
            for(int i = 0; i < 10; ++i)
            {
                logsCollection.Add(new Entry("log" + i.ToString()));
                if(i % 2 == 0)
                {
                    logsCollection.Add(new Entry("Log" + i.ToString() + ".2"));
                    logsCollection.Add(new Entry("lo" + i.ToString() + "g"));
                }
            }

            Assert.AreEqual(20, logsCollection.Count());
            Assert.AreEqual(15, logsCollection.CountOfContains("log"));
            Assert.AreEqual(10, logsCollection.CountOfContains("log", true));
        }

        [TestMethod]
        public void LogsByEntryLevel()
        {
            LogsCollection logsCollection = new LogsCollection();
            List<string> results = new List<string>
            {
                "ban smwh",
                "add smth",
                "delete smth"
            };
            logsCollection.Add(new Entry(results[0], EntryLevel.Admin));
            logsCollection.Add(new Entry(results[1], EntryLevel.Admin));
            logsCollection.Add(new Entry(results[2], EntryLevel.Admin));
            logsCollection.Add(new Entry("try ban", EntryLevel.User));
            logsCollection.Add(new Entry("try add", EntryLevel.Undefined));
            logsCollection.Add(new Entry("try delete", EntryLevel.Server));

            List<Entry> entries = logsCollection.GetLogsByLevel(EntryLevel.Admin).ToList();

            Assert.AreEqual(3, entries.Count());
            Assert.AreEqual(results[0], entries[0].Text);
            Assert.AreEqual(EntryLevel.Admin, entries[0].Level);
            Assert.AreEqual(results[1], entries[1].Text);
            Assert.AreEqual(EntryLevel.Admin, entries[1].Level);
            Assert.AreEqual(results[2], entries[2].Text);
            Assert.AreEqual(EntryLevel.Admin, entries[2].Level);
        }

        [TestMethod]
        public void EventHandling()
        {
            LogsCollection logsCollection = new LogsCollection();
            bool isCalled = false;
            logsCollection.OnAdd += (entry) => { isCalled = !isCalled; };

            logsCollection.Add(new Entry("smth"));
            Assert.IsTrue(isCalled);
            logsCollection.Add(new Entry("smth2"));
            Assert.IsFalse(isCalled);
            logsCollection.Add(new Entry("smth3"));
            Assert.IsTrue(isCalled);

            LogsCollection logsCollection2 = new LogsCollection();
            logsCollection.OnAdd += (entry) => 
            { 
                logsCollection2.Add(new Entry(entry.Text, entry.Time, entry.Level)); 
            };

            logsCollection.Add(new Entry
                (
                "test2.0",
                new DateTime(1992, 6, 12, 12, 0, 0), 
                EntryLevel.System)
                );

            Entry expected = new Entry("test2.0", new DateTime(1992, 6, 12, 12, 0, 0), EntryLevel.System);
            Assert.AreEqual(expected, logsCollection2.LastAdded());
        }

        [TestMethod]
        public void LastLog()
        {
            LogsCollection logsCollection = new LogsCollection();
            Entry log1 = new Entry("smth", new DateTime(2203, 1, 1, 1, 1, 1));
            Entry log2 = new Entry("smth2");
            logsCollection.Add(log1);
            for(int i = 0; i < 10; ++i)
            {
                logsCollection.Add(new Entry("smth for " + i.ToString()));
            }
            logsCollection.Add(log2);

            Entry lastLog1 = logsCollection.LastByTime();
            Entry lastLog2 = logsCollection.LastAdded();

            Assert.AreEqual(log1, lastLog1);
            Assert.AreEqual(log2, lastLog2);
        }
    }
}
