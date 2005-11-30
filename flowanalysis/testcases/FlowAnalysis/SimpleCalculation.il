.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void Main(int32 x) cil managed
	{
		.locals init (int32 local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.2
		IL_0002: mul
		IL_0003: stloc.0
		IL_0004: ldloc.0
		IL_0005: call void [mscorlib]System.Console::WriteLine(int32)
		IL_000a: ret
	}
}
