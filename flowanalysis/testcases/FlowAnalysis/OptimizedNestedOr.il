.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 'value') cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.5
		IL_0002: bge.s IL_0008
		IL_0004: ldarg.0
		IL_0005: ldc.i4.2
		IL_0006: bne.un.s IL_000e
		IL_0008: ldarg.0
		IL_0009: ldc.i4.s 10
		IL_000b: clt
		IL_000d: ret
		IL_000e: ldc.i4.0
		IL_000f: ret
	}
}
