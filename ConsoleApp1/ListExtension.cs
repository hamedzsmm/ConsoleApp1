namespace ConsoleApp1
{
    public class ListDifference<T>
    {
        public IEnumerable<T>? Added { get; set; }
        public IEnumerable<T>? Deleted { get; set; }
    }

    public static class ListComparer
    {
        public static ListDifference<T> FindDifferences<T, TKey>(List<T> originalList, List<T> newList, Func<T, TKey> keySelector)
        {
            var originalSet = new HashSet<TKey>(originalList.Select(keySelector));
            var newSet = new HashSet<TKey>(newList.Select(keySelector));

            var added = newList.Where(item => !originalSet.Contains(keySelector(item)));
            var deleted = originalList.Where(item => !newSet.Contains(keySelector(item)));

            return new ListDifference<T>
            {
                Added = added,
                Deleted = deleted
            };
        }
    }
}
