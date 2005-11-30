.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 x) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.3
		IL_0002: beq.s IL_000a
		IL_0004: ldarg.0
		IL_0005: ldc.i4.5
		IL_0006: ceq
		IL_0008: br.s IL_000b
		IL_000a: ldc.i4.1
		IL_000b: stloc.0
		IL_000c: br.s IL_000e
		IL_000e: ldloc.0
		IL_000f: ret
	}
}
