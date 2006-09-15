.assembly extern mscorlib {}
.assembly TestCase {}

.class public auto ansi beforefieldinit TestCase
       extends [mscorlib]System.Object
{
  .method public hidebysig instance bool
          Main(class Cat cat) cil managed
  {
    .locals init (bool V_0)
    IL_0000:  ldarg.1
    IL_0001:  callvirt   instance string Cat::get_Name()
    IL_0006:  ldstr      "Occam"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  stloc.0
    IL_0011:  br.s       IL_0013
    IL_0013:  ldloc.0
    IL_0014:  ret
  } // end of method TestCase::Main


  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestCase::.ctor
} // end of class TestCase

.class public auto ansi beforefieldinit Cat
       extends [mscorlib]System.Object
{
  .field public string _name

  .method public hidebysig specialname rtspecialname
          instance void  .ctor(string name) cil managed
  {
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      string Cat::_name
    IL_000d:  ret
  }

  .method public hidebysig specialname instance string
          get_Name() cil managed
  {
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string Cat::_name
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009
    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method Cat::get_Name

  .property instance string Name()
  {
    .get instance string Cat::get_Name()
  } // end of property Cat::Name
} // end of class Cat

