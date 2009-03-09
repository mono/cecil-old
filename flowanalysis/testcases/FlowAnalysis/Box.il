.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void Main(string[] args) cil managed
	{
		.locals init (int32 local0)
		IL_0000: nop
		IL_0001: ldc.i4.s 42
		IL_0003: box [mscorlib]System.Int32
		IL_0008: stloc.0
		IL_0009: ldloc.0
		IL_000a: call void [mscorlib]System.Console::WriteLine(object)
		IL_000f: nop
		IL_0010: ret
	}
}
