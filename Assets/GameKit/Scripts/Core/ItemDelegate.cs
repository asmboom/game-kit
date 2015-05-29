namespace Codeplay
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
