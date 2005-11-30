.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(float32 'value') cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.r4 1.1
		IL_0006: ceq
		IL_0008: stloc.0
		IL_0009: br.s IL_000b
		IL_000b: ldloc.0
		IL_000c: ret
	}
}
