"""
42000
"""

worker = Main.CilWorker

worker.Emit(OpCodes.Ldc_I4, 42000)
worker.Emit(OpCodes.Call, ImportConsoleWriteLine(int))
worker.Emit(OpCodes.Ret)

Main.Optimize()
