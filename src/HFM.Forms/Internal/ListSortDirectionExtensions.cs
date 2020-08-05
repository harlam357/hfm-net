using System.ComponentModel;

namespace HFM.Forms.Internal
{
    internal static class ListSortDirectionExtensions
    {
        /// <summary>
        /// Returns "ASC" or "DESC".
        /// </summary>
        internal static string ToBindingSourceSortString(this ListSortDirection direction)
        {
            return direction.Equals(ListSortDirection.Descending) ? "DESC" : "ASC";
        }
    }
}
