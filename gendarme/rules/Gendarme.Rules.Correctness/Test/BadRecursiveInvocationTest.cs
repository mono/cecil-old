class BadRec {

    /* This should be an error. */
    public int Foo {
        get { return Foo; }
    }

    /* This should be fine, as it uses 'base.' */
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static void Main(string[] args)
    {
    }
}
