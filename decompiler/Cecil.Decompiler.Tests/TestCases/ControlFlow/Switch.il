.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static int32 Main(int32 'value') cil managed
	{
		.locals init (int32 local0, int32 local1)
		IL_0000:  nop
		IL_0001:  ldarg.0
		IL_0002:  stloc.1
		IL_0003:  ldloc.1
		IL_0004:  ldc.i4.1
		IL_0005:  sub
		IL_0006:  switch (IL_001d, IL_0021, IL_0025, IL_0029)
		IL_001b:  br.s       IL_002d
		IL_001d:  ldc.i4.1
		IL_001e:  stloc.0
		IL_001f:  br.s       IL_0031
		IL_0021:  ldc.i4.2
		IL_0022:  stloc.0
		IL_0023:  br.s       IL_0031
		IL_0025:  ldc.i4.3
		IL_0026:  stloc.0
		IL_0027:  br.s       IL_0031
		IL_0029:  ldc.i4.4
		IL_002a:  stloc.0
		IL_002b:  br.s       IL_0031
		IL_002d:  ldc.i4.m1
		IL_002e:  stloc.0
		IL_002f:  br.s       IL_0031
		IL_0031:  ldloc.0
		IL_0032:  ret
	}
}
