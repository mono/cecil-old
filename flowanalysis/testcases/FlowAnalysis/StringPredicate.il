.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(string 'value') cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldstr "foo"
		IL_0006: call bool string::op_Equality(string, string)
		IL_000b: brtrue.s IL_001a
		IL_000d: ldarg.0
		IL_000e: ldstr "bar"
		IL_0013: call bool string::op_Equality(string, string)
		IL_0018: br.s IL_001b
		IL_001a: ldc.i4.1
		IL_001b: stloc.0
		IL_001c: br.s IL_001e
		IL_001e: ldloc.0
		IL_001f: ret
	}
}
