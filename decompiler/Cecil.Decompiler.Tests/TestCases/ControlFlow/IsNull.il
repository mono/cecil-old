.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.field object _field

	.method public hidebysig bool Main() cil managed
	{
		.locals init (bool local0)

		IL_0000: ldarg.0
		IL_0001: ldfld object TestCase::_field
		IL_0006: ldnull
		IL_0007: ceq
		IL_0009: stloc.0
		IL_000a: br.s IL_000c
		IL_000c: ldloc.0
		IL_000d: ret
	}
}
