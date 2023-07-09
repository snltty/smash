using System;
using System.IO;

namespace common.libs.extends
{
    public static class FileExtends
    {
        public static bool TryDeleteFile(this string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return true;
        }
    }
}
