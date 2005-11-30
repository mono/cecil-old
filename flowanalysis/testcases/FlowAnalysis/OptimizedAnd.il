.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(string s) cil managed
	{
		IL_0000: ldarg.0
		IL_0001: brfalse.s IL_0010
		IL_0003: ldarg.0
		IL_0004: callvirt int32 string::get_Length()
		IL_0009: ldc.i4.3
		IL_000a: clt
		IL_000c: ldc.i4.0
		IL_000d: ceq
		IL_000f: ret
		IL_0010: ldc.i4.0
		IL_0011: ret
	}
}
