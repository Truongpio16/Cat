using System;
using System.IO;

namespace Cat.Core
{
    // Pure System.IO helpers for save data. No UnityEngine (Cat.Core is compiled with noEngineReferences).
    public static class SafeFile
    {
        // Writes text so that a crash or power loss never leaves a half-written file at 'path':
        // the text goes to "<path>.tmp" first and only then replaces the real file.
        // On failure the old file (if any) is untouched and the exception propagates to the caller.
        public static void WriteAtomic(string path, string text)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, text);

            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
            }
            else
            {
                File.Move(tempPath, path);
            }
        }

        // False when the file does not exist or cannot be read; never throws for ordinary IO problems.
        public static bool TryRead(string path, out string text)
        {
            text = null;
            try
            {
                if (!File.Exists(path)) return false;
                text = File.ReadAllText(path);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // Moves a damaged file aside to "<path>.corrupt" (replacing an older .corrupt copy) so it is kept
        // for inspection instead of being overwritten. Returns false when there is nothing to move or it fails.
        public static bool Quarantine(string path)
        {
            try
            {
                if (!File.Exists(path)) return false;

                string corruptPath = path + ".corrupt";
                if (File.Exists(corruptPath))
                {
                    File.Delete(corruptPath);
                }
                File.Move(path, corruptPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
