.assembly extern mscorlib {}

.assembly TestCase {}

.class public auto ansi beforefieldinit Pilot
       extends [mscorlib]System.Object
{
  .field private int32 _points

  .method public hidebysig specialname rtspecialname
          instance void  .ctor(int32 points) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 Pilot::_points
    IL_000d:  ret
  }

  .method public hidebysig specialname instance int32
          get_Points() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 Pilot::_points
    IL_0006:  ret
  }

  .property instance int32 Points()
  {
    .get instance int32 Pilot::get_Points()
  }
}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
	.method public hidebysig static bool Main(class Pilot pilot) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance int32 Pilot::get_Points()
    IL_0006:  ldc.i4.s   100
    IL_0008:  ceq
    IL_000a:  ret
  }

}
