.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static void  Main() cil managed
	{
	.entrypoint
		.locals init (int32 V_0)
		IL_0000:  nop
		IL_0001:  ldc.i4.1
		IL_0002:  stloc.0
		IL_0003:  ldloc.0
		IL_0004:  ldc.i4.2
		IL_0005:  add
		IL_0006:  stloc.0
		IL_0007:  ldloc.0
		IL_0008:  ldc.i4.3
		IL_0009:  sub
		IL_000a:  stloc.0
		IL_000b:  ldloc.0
		IL_000c:  ldc.i4.4
		IL_000d:  mul
		IL_000e:  stloc.0
		IL_000f:  ldloc.0
		IL_0010:  ldc.i4.5
		IL_0011:  div
		IL_0012:  stloc.0
		IL_0013:  ldloc.0
		IL_0014:  ldc.i4.6
		IL_0015:  shr
		IL_0016:  stloc.0
		IL_0017:  ldloc.0
		IL_0018:  ldc.i4.7
		IL_0019:  shl
		IL_001a:  stloc.0
		IL_001b:  ldloc.0
		IL_001c:  ldc.i4.8
		IL_001d:  or
		IL_001e:  stloc.0
		IL_001f:  ldloc.0
		IL_0020:  ldc.i4.s   9
		IL_0022:  and
		IL_0023:  stloc.0
		IL_0024:  ldloc.0
		IL_0025:  ldc.i4.s   10
		IL_0027:  xor
		IL_0028:  stloc.0
		IL_0029:  ldloc.0
		IL_002a:  ldc.i4.s   11
		IL_002c:  rem
		IL_002d:  stloc.0
		IL_002e:  ldloc.0
		IL_002f:  not
		IL_0030:  stloc.0
		IL_0031:  ret
	}
}
