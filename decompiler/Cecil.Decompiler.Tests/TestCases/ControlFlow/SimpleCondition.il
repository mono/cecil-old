.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 x) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.s 42
		IL_0003: ceq
		IL_0005: stloc.0
		IL_0006: br.s IL_0008
		IL_0008: ldloc.0
		IL_0009: ret
	}
}
