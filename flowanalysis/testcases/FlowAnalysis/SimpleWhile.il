.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void Main() cil managed
	{
		IL_0000: newobj void [mscorlib]System.Random::.ctor()
		IL_0005: stloc.0
		IL_0006: br.s IL_0012
		IL_0008: ldstr "funny..."
		IL_000d: call void [mscorlib]System.Console::WriteLine(string)
		IL_0012: ldloc.0
		IL_0013: ldc.i4.0
		IL_0014: ldc.i4.s 10
		IL_0016: callvirt int32 [mscorlib]System.Random::Next(int32, int32)
		IL_001b: ldc.i4.5
		IL_001c: blt.s IL_0008
		IL_001e: ret
	}
}
