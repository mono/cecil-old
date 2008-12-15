.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.field int32 _field

	.method public hidebysig int32 Main() cil managed
	{
		.locals init (bool local0)
		IL_0000: ldarg.0
		IL_0001: ldfld int32 TestCase::_field
		IL_0006: stloc.0
		IL_0007: br.s IL_0009
		IL_0009: ldloc.0
		IL_000a: ret
	}
}
