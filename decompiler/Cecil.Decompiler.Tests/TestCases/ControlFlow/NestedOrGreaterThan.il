.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 'value') cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.5
		IL_0002: bge.s IL_0009
		IL_0004: ldarg.0
		IL_0005: ldc.i4.s 2
		IL_0007: bne.un.s IL_0010
		IL_0009: ldarg.0
		IL_000a: ldc.i4.s 10
		IL_000c: clt
		IL_000e: br.s IL_0011
		IL_0010: ldc.i4.0
		IL_0011: stloc.0
		IL_0012: br.s IL_0014
		IL_0014: ldloc.0
		IL_0015: ret
	}
}
