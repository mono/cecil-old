.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 id) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.s 42
		IL_0003: bgt.s IL_000f
		IL_0005: ldarg.0
		IL_0006: ldc.i4.s 10
		IL_0008: cgt
		IL_000a: ldc.i4.0
		IL_000b: ceq
		IL_000d: br.s IL_0010
		IL_000f: ldc.i4.1
		IL_0010: stloc.0
		IL_0011: br.s IL_0013
		IL_0013: ldloc.0
		IL_0014: ret
	}
}
