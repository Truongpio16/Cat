using System;
using System.IO;
using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class SafeFileTests
    {
        private sealed class TempDir : IDisposable
        {
            public readonly string Path;

            public TempDir()
            {
                Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "cat-safefile-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }

            public string File(string name) => System.IO.Path.Combine(Path, name);

            public void Dispose()
            {
                try { Directory.Delete(Path, true); } catch (IOException) { }
            }
        }

        [Test]
        public void WriteAtomic_CreatesFileWithContent()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "{\"a\":1}");
            Assert.AreEqual("{\"a\":1}", System.IO.File.ReadAllText(path));
        }

        [Test]
        public void WriteAtomic_OverwritesExistingFile()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "old");
            SafeFile.WriteAtomic(path, "new");
            Assert.AreEqual("new", System.IO.File.ReadAllText(path));
        }

        [Test]
        public void WriteAtomic_LeavesNoTempFileBehind()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "one");
            SafeFile.WriteAtomic(path, "two");
            Assert.IsFalse(System.IO.File.Exists(path + ".tmp"));
        }

        [Test]
        public void WriteAtomic_CreatesMissingDirectory()
        {
            using var dir = new TempDir();
            string path = System.IO.Path.Combine(dir.Path, "sub", "deeper", "save.json");
            SafeFile.WriteAtomic(path, "x");
            Assert.AreEqual("x", System.IO.File.ReadAllText(path));
        }

        [Test]
        public void WriteAtomic_IgnoresStaleTempFileFromEarlierCrash()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "good");
            System.IO.File.WriteAllText(path + ".tmp", "half-writt");
            SafeFile.WriteAtomic(path, "newer");
            Assert.AreEqual("newer", System.IO.File.ReadAllText(path));
            Assert.IsFalse(System.IO.File.Exists(path + ".tmp"));
        }

        [Test]
        public void WriteAtomic_WhenTempCannotBeWritten_KeepsOldFile()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "precious");
            Directory.CreateDirectory(path + ".tmp"); // a directory where the temp file must go => the write fails
            bool threw = false;
            try
            {
                SafeFile.WriteAtomic(path, "should-not-land");
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                threw = true;
            }
            Assert.IsTrue(threw);
            Assert.AreEqual("precious", System.IO.File.ReadAllText(path));
        }

        [Test]
        public void TryRead_MissingFile_ReturnsFalse()
        {
            using var dir = new TempDir();
            Assert.IsFalse(SafeFile.TryRead(dir.File("nope.json"), out string text));
            Assert.IsNull(text);
        }

        [Test]
        public void TryRead_ExistingFile_ReturnsContent()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            SafeFile.WriteAtomic(path, "hello");
            Assert.IsTrue(SafeFile.TryRead(path, out string text));
            Assert.AreEqual("hello", text);
        }

        [Test]
        public void TryRead_EmptyFile_ReturnsTrueWithEmptyText()
        {
            using var dir = new TempDir();
            string path = dir.File("empty.json");
            System.IO.File.WriteAllText(path, "");
            Assert.IsTrue(SafeFile.TryRead(path, out string text));
            Assert.AreEqual("", text);
        }

        [Test]
        public void Quarantine_MovesFileAsideAndKeepsContent()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            System.IO.File.WriteAllText(path, "{broken");
            Assert.IsTrue(SafeFile.Quarantine(path));
            Assert.IsFalse(System.IO.File.Exists(path));
            Assert.AreEqual("{broken", System.IO.File.ReadAllText(path + ".corrupt"));
        }

        [Test]
        public void Quarantine_ReplacesOlderCorruptCopy()
        {
            using var dir = new TempDir();
            string path = dir.File("save.json");
            System.IO.File.WriteAllText(path + ".corrupt", "older");
            System.IO.File.WriteAllText(path, "newer-broken");
            Assert.IsTrue(SafeFile.Quarantine(path));
            Assert.AreEqual("newer-broken", System.IO.File.ReadAllText(path + ".corrupt"));
        }

        [Test]
        public void Quarantine_MissingFile_ReturnsFalse()
        {
            using var dir = new TempDir();
            Assert.IsFalse(SafeFile.Quarantine(dir.File("nope.json")));
        }
    }
}
