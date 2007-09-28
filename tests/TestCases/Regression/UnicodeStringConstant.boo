"""
Password
True
"""

constant = "Password"

t = ASM.MainModule.Types ["ClassLibrary1.Class1"]

sc = t.Fields.GetField ("fn_Password").Constant as string

print sc
print sc.Length == constant.Length
