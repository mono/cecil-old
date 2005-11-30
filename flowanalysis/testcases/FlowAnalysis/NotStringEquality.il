.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(string s) cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldstr "foo"
		IL_0006: call bool string::op_Equality(string, string)
		IL_000b: ldc.i4.0
		IL_000c: ceq
		IL_000e: stloc.0
		IL_000f: br.s IL_0011
		IL_0011: ldloc.0
		IL_0012: ret
	}
}
