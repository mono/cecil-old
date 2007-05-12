"""
Hello, world!
"""

worker = Main.CilWorker

worker.Emit(OpCodes.Ldstr, "Hello, world!")
worker.Emit(OpCodes.Call, ImportConsoleWriteLine())
worker.Emit(OpCodes.Ret)
