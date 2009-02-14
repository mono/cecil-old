using System;

class Casts {

	public short ToShort (int a)
	{
		return (short) a;
	}

	public class Bar {
	}

	public Bar ToBar (object o)
	{
		return (Bar) o;
	}

	public Bar AsBar (object o)
	{
		return o as Bar;
	}

	public bool IsBar (object o)
	{
		return o is Bar;
	}
}
