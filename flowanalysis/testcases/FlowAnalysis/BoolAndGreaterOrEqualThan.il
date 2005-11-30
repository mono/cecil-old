.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(string s) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldstr "A"
		IL_0006: callvirt bool string::StartsWith(string)
		IL_000b: brfalse.s IL_001b
		IL_000d: ldarg.0
		IL_000e: callvirt int32 string::get_Length()
		IL_0013: ldc.i4.3
		IL_0014: clt
		IL_0016: ldc.i4.0
		IL_0017: ceq
		IL_0019: br.s IL_001c
		IL_001b: ldc.i4.0
		IL_001c: stloc.0
		IL_001d: br.s IL_001f
		IL_001f: ldloc.0
		IL_0020: ret
	}
}
