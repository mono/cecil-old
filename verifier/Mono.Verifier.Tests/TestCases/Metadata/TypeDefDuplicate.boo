"""
[MD] (0x8013120E) Error: TypeDef has a duplicate based on name+namespace, token=0x0a000002
"""

def Types():
	yield TypeDefinition("Bar", "Foo", TypeAttributes.Public, Import(typeof(object)))
	yield TypeDefinition("Bar", "Foo", TypeAttributes.Public, Import(typeof(object)))

for t in Types():
	ASM.MainModule.Types.Add(t)
