.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(int32 x) cil managed
	{
		.locals init(bool local0)
		IL_0000: ldarg.0
		IL_0001: ldc.i4.3
		IL_0002: ble.s IL_000b
		IL_0004: ldarg.0
		IL_0005: ldc.i4.s 42
		IL_0007: clt
		IL_0009: br.s IL_000c
		IL_000b: ldc.i4.0
		IL_000c: stloc.0
		IL_000d: br.s IL_000f
		IL_000f: ldloc.0
		IL_0010: ret
	}
}
