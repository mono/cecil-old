.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 x) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.3
		IL_0002: ble.s IL_0009
		IL_0004: ldarg.0
		IL_0005: ldc.i4.s 42
		IL_0007: blt.s IL_0018
		IL_0009: ldarg.0
		IL_000a: ldc.i4.s 100
		IL_000c: blt.s IL_0015
		IL_000e: ldarg.0
		IL_000f: ldc.i4.s 115
		IL_0011: clt
		IL_0013: br.s IL_0016
		IL_0015: ldc.i4.0
		IL_0016: br.s IL_0019
		IL_0018: ldc.i4.1
		IL_0019: stloc.0
		IL_001a: br.s IL_001c
		IL_001c: ldloc.0
		IL_001d: ret
	}
}
