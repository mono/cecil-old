.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void Main() cil managed
	{
		IL_0000: ldc.i4.1
		IL_0001: stloc.0
		IL_0002: br.s IL_002b
		IL_0004: ldloc.0
		IL_0005: ldc.i4.2
		IL_0006: rem
		IL_0007: brtrue.s IL_0027
		IL_0009: ldstr "Even number"
		IL_000e: call void [mscorlib]System.Console::WriteLine(string)
		IL_0013: ldc.i4.0
		IL_0014: stloc.1
		IL_0015: ldstr "!"
		IL_001a: call void [mscorlib]System.Console::WriteLine(string)
		IL_001f: ldloc.1
		IL_0020: ldc.i4.1
		IL_0021: add
		IL_0022: stloc.1
		IL_0023: ldloc.1
		IL_0024: ldloc.0
		IL_0025: blt.s IL_0015
		IL_0027: ldloc.0
		IL_0028: ldc.i4.1
		IL_0029: add
		IL_002a: stloc.0
		IL_002b: ldloc.0
		IL_002c: ldc.i4.5
		IL_002d: blt.s IL_0004
		IL_002f: ret
	}
}
