using System;

public class Foo : IFoo {
}

public interface IFoo : IBar {
}

public interface IBar {
}

[NotLinked] public class Baz : IBaz {
}

[NotLinked] public interface IBaz {
}

[NotLinked, AttributeUsage (AttributeTargets.All)]
public class NotLinkedAttribute : Attribute {
}
