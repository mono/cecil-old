"""
System.Int32 CustomMarshalerNamespace.CustomMarshalerInterface::CustomMarshalerMethod()
System.Int32 CustomMarshalerMethod()
"""

iface = ASM.MainModule.Types ["CustomMarshalerNamespace.CustomMarshalerInterface"]
meth = iface.Methods.GetMethod ("CustomMarshalerMethod") [0]

print meth
print meth.Clone ()

