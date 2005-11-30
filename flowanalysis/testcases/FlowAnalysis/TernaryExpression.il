.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static int32 Main(bool) cil managed
	{
		IL_0000: ldarg.0
		IL_0001: brtrue.s IL_0006
		IL_0003: ldc.i4.m1
		IL_0004: br.s IL_0008
		IL_0006: ldc.i4.s 42
		IL_0008: stloc.0
		IL_0009: br.s IL_000b
		IL_000b: ldloc.0
		IL_000c: ret
	}
}
