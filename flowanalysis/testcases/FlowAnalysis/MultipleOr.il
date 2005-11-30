.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 x) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.3
		IL_0002: beq.s IL_0014
		IL_0004: ldarg.0
		IL_0005: ldc.i4.5
		IL_0006: beq.s IL_0014
		IL_0008: ldarg.0
		IL_0009: ldc.i4.s 42
		IL_000b: beq.s IL_0014
		IL_000d: ldarg.0
		IL_000e: ldc.i4.s 100
		IL_0010: cgt
		IL_0012: br.s IL_0015
		IL_0014: ldc.i4.1
		IL_0015: stloc.0
		IL_0016: br.s IL_0018
		IL_0018: ldloc.0
		IL_0019: ret
	}
}
