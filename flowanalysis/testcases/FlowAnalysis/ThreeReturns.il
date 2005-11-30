.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32, int32) cil managed
	{
		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ble.s IL_0008
		IL_0004: ldc.i4.1
		IL_0005: stloc.0
		IL_0006: br.s IL_0014
		IL_0008: ldarg.0
		IL_0009: ldarg.1
		IL_000a: bge.s IL_0010
		IL_000c: ldc.i4.1
		IL_000d: stloc.0
		IL_000e: br.s IL_0014
		IL_0010: ldc.i4.0
		IL_0011: stloc.0
		IL_0012: br.s IL_0014
		IL_0014: ldloc.0
		IL_0015: ret
	}
}
