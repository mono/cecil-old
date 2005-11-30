.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32) cil managed
	{
		IL_0000: ldarg.0
		IL_0001: ldc.i4.1
		IL_0002: beq.s IL_000d
		IL_0004: ldarg.0
		IL_0005: ldc.i4.3
		IL_0006: ble.s IL_0011
		IL_0008: ldarg.0
		IL_0009: ldc.i4.s 42
		IL_000b: bge.s IL_0011
		IL_000d: ldc.i4.1
		IL_000e: stloc.0
		IL_000f: br.s IL_0015
		IL_0011: ldc.i4.0
		IL_0012: stloc.0
		IL_0013: br.s IL_0015
		IL_0015: ldloc.0
		IL_0016: ret
	}
}
