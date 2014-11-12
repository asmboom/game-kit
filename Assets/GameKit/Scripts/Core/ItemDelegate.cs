namespace Beetle23
{
    public class ItemDelegate<T>
    {
        public ItemDelegate(T context)
        {
            _context = context;
        }

        protected T _context;
    }
}
