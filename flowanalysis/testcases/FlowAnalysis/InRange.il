.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 'value') cil managed
	{
		IL_0000: ldarg.0
		IL_0001: ldc.i4.5
		IL_0002: blt.s IL_000c
		IL_0004: ldarg.0
		IL_0005: ldc.i4.8
		IL_0006: cgt
		IL_0008: ldc.i4.0
		IL_0009: ceq
		IL_000b: ret
		IL_000c: ldc.i4.0
		IL_000d: ret
	}
}
