.assembly extern mscorlib {}
.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void Main(string[] s) cil managed
	{
		IL_0000: nop
		IL_0001: ldarg.0
		IL_0002: isinst [mscorlib]System.Collections.IEnumerable
		IL_0007: callvirt instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
		IL_000c: pop
		IL_000d: br.s IL_000f
		IL_000f: ret
	}
}
