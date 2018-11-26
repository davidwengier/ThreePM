using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ThreePM.Utilities
{
    public interface IHaveAlbumArt
    {
        Bitmap AlbumArt
        {
            set;
        }
    }

    public class AlbumArtLoader
    {
        private class AlbumArtWaiter
        {
            public int Width;
            public int Height;
            public string Filename;
            public IHaveAlbumArt IHaveAlbumArt;
        }

        private Queue<AlbumArtWaiter> m_filesToLoad = new Queue<AlbumArtWaiter>();

        private Thread albumLoaderThread;

        public AlbumArtLoader()
        {
        }

        ~AlbumArtLoader()
        {
            if (albumLoaderThread != null && albumLoaderThread.IsAlive)
            {
                albumLoaderThread.Abort();
                albumLoaderThread.Join();
                albumLoaderThread = null;
            }
            lock (m_filesToLoad)
            {
                m_filesToLoad.Clear();
            }
            m_filesToLoad = null;
        }

        private void LoadNextAlbum()
        {
            while (true)
            {
                int i;
                lock (m_filesToLoad)
                {
                    i = m_filesToLoad.Count;
                }
                if (i > 0)
                {
                    AlbumArtWaiter waiter;
                    lock (m_filesToLoad)
                    {
                        waiter = m_filesToLoad.Dequeue();
                    }
                    waiter.IHaveAlbumArt.AlbumArt = AlbumArtHelper.GetAlbumArt(waiter.Filename, waiter.Width, waiter.Height);
                }
                else
                {
                    albumLoaderThread = null;
                    Thread.CurrentThread.Abort();
                }
            }
        }

        public void LoadAlbumArt(IHaveAlbumArt iHaveAlbumArt, string filename, int width, int height)
        {
            AlbumArtWaiter waiter = new AlbumArtWaiter();
            waiter.Filename = filename;
            waiter.Width = width;
            waiter.Height = height;
            waiter.IHaveAlbumArt = iHaveAlbumArt;
            iHaveAlbumArt.AlbumArt = new Bitmap(Properties.Resources.LoadingAlbumArt, new Size(width, height));
            lock (m_filesToLoad)
            {
                m_filesToLoad.Enqueue(waiter);
            }
            if (albumLoaderThread == null)
            {
                albumLoaderThread = new Thread(LoadNextAlbum);
                albumLoaderThread.IsBackground = true;
                albumLoaderThread.Start();
            }
        }


        public void Clear()
        {
            if (albumLoaderThread != null && albumLoaderThread.IsAlive)
            {
                albumLoaderThread.Abort();
                albumLoaderThread.Join();
                albumLoaderThread = null;
            }
            lock (m_filesToLoad)
            {
                m_filesToLoad.Clear();
            }
        }
    }

    public static class AlbumArtHelper
    {

        //// Have to do the desctructor this way, because static things dont have destructors
        //private class Destructor
        //{
        //    ~Destructor()
        //    {
        //        // This made things crash... is it bad to not dispose bitmaps :)

        //        //string[] keys = new string[cache.Count];
        //        //cache.Keys.CopyTo(keys, 0);
        //        //for (int i = cache.Count - 1; i >= 0; i--)
        //        //{
        //        //    string dir = keys[i];
        //        //    cache[dir].Dispose();
        //        //    cache[dir] = null;
        //        //    cache.Remove(dir);
        //        //}
        //    }
        //}

        //private static Destructor m_destructor = new Destructor();
        //private static Dictionary<string, Bitmap> cache = new Dictionary<string, Bitmap>();

        public static Bitmap GetAlbumArt(string filename, int width, int height)
        {
            if (width == 0 || height == 0) return null;
            if (filename.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
                return null;

            ThreePM.MusicPlayer.SongInfo info = new ThreePM.MusicPlayer.SongInfo(filename);
            if (info.HasFrontCover)
            {
                return info.GetFrontCover(width, height);
            }

            //if (CacheContains(filename))
            //{
            //    return GetCachedBitmap(filename, width, height);
            //}

            string imgFile = "";
            if (!String.IsNullOrEmpty(filename))
            {
                string dir = Path.GetDirectoryName(filename);
                bool found = false;
                foreach (string fold in MusicLibrary.Library.NonExistantFolders)
                {
                    if (dir.StartsWith(fold))
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    if (!dir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                    {
                        dir += Path.DirectorySeparatorChar;
                    }

                    if (Directory.Exists(dir))
                    {
                        string[] files = Directory.GetFiles(dir, "AlbumArt_*_Large.jpg", SearchOption.TopDirectoryOnly);
                        if (files.Length > 0)
                        {
                            imgFile = files[0];
                        }
                        else if (File.Exists(dir + "Folder.jpg"))
                        {
                            imgFile = dir + "Folder.jpg";
                        }
                    }
                }
            }
            if (String.IsNullOrEmpty(imgFile))
            {
                //AddCachedBitmap(filename, null);
                //return GetCachedBitmap(filename, width, height);
                return new Bitmap(Properties.Resources.NoAlbumArt, new Size(width, height));
            }
            else
            {
                using (Stream s = File.OpenRead(imgFile))
                {
                    using (Image img = Image.FromStream(s))
                    {
                        //AddCachedBitmap(filename, new Bitmap(img));
                        //return GetCachedBitmap(filename, width, height);
                        return new Bitmap(img, new Size(width, height));
                    }
                }
            }
        }

        //public static void InvalidateCache(string filename)
        //{
        //    string dir = Path.GetDirectoryName(filename);
        //    if (cache.ContainsKey(dir))
        //    {
        //        cache[dir].Dispose();
        //        cache[dir] = null;
        //        lock (cache)
        //        {
        //            cache.Remove(dir);
        //        }
        //    }
        //}

        //private static bool CacheContains(string filename)
        //{
        //    string dir = Path.GetDirectoryName(filename);
        //    return cache.ContainsKey(dir);
        //}

        //private static void AddCachedBitmap(string filename, Bitmap bitmap)
        //{
        //    string dir = Path.GetDirectoryName(filename);
        //    lock (cache)
        //    {
        //        cache.Add(dir, bitmap);
        //    }
        //}

        //private static Bitmap GetCachedBitmap(string filename, int width, int height)
        //{
        //    string dir = Path.GetDirectoryName(filename);

        //    if (cache[dir] == null)
        //    {
        //        return /*new Bitmap( */ Properties.Resources.NoAlbumArt /*, new Size(width, height))*/ ;
        //    }
        //    else
        //    {
        //        lock (cache)
        //        {
        //            return /*new Bitmap(*/ cache[dir] /*, new Size(width, height)) */;
        //        }
        //    }
        //}
    }
}
