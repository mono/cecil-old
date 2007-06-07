using System;

public class BarAttribute : Attribute {

	public BarAttribute ()
	{
	}

	public BarAttribute (Type type)
	{
	}

	public Type FieldType;

	public Type PropertyType {
		[NotLinked] get { return null; }
		set {}
	}
}

[Bar (typeof (Guy_A))]
public class Foo {

	[Bar (FieldType = typeof (Guy_B))]
	public Foo a;

	[Bar (PropertyType = typeof (Guy_C))]
	public Foo b;
}

public class Guy_A {

	[NotLinked] public Guy_A ()
	{
	}
}

public class Guy_B {

	[NotLinked] public Guy_B ()
	{
	}
}

public class Guy_C {

	[NotLinked] public Guy_C ()
	{
	}
}

[NotLinked] public class Baz {
}

[NotLinked, AttributeUsage (AttributeTargets.All)]
public class NotLinkedAttribute : Attribute {
}
