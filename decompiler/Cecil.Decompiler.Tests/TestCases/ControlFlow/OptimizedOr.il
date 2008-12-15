.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(string s) cil managed
	{
		IL_0000: ldarg.0
		IL_0001: ldstr "foo"
		IL_0006: call bool string::op_Equality(string, string)
		IL_000b: brtrue.s IL_0019
		IL_000d: ldarg.0
		IL_000e: ldstr "bar"
		IL_0013: call bool string::op_Equality(string, string)
		IL_0018: ret
		IL_0019: ldc.i4.1
		IL_001a: ret
	}
}
