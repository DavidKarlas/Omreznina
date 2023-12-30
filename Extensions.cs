using LiveChartsCore.SkiaSharpView.Painting;
using System.Collections.ObjectModel;

namespace Omreznina
{
    public static class Extensions
    {
        public static void SyncCollections<T>(this ObservableCollection<T> observable, IList<T> newCollection) where T : IEquatable<T>
        {
            for (int i = 0; i < newCollection.Count && i < observable.Count; i++)
            {
                if (!observable[i].Equals(newCollection[i]))
                {
                    observable[i] = newCollection[i];
                }
            }
            while (observable.Count > newCollection.Count)
            {
                observable.RemoveAt(observable.Count - 1);
            }
            for (int i = observable.Count; i < newCollection.Count; i++)
            {
                observable.Add(newCollection[i]);
            }
        }

        public static SolidColorPaint ToPaint(this string hex, float strokeWidth = 1)
        {
            if (!hex.StartsWith("#"))
                throw new Exception("Hex color must start with #");
            if (hex.Length != 7)
                throw new Exception("Hex color must be 7 characters long");
            return new SolidColorPaint(new SkiaSharp.SKColor(Convert.ToUInt32(hex.Remove(0, 1), 16) | 0xff000000), strokeWidth);
        }
    }
}
