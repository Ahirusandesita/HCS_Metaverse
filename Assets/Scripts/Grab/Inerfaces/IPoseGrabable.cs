namespace Grab
{
    public interface IGrabable
    {
        public void Grab();
    }
    public interface IGrabableSelect
    {
        public void Select();
        public void UnSelect();
        public bool CanGrab { get; }
    }
}

