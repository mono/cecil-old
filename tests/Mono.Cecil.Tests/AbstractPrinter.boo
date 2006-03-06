namespace Mono.Cecil.Tests

import System
import System.IO

class AbstractPrinter:
	
	[getter(Writer)]
	_writer = StringWriter()
	
	def Write(s as string):
		_writer.Write(s)
		
	def WriteLine(s as string):
		_writer.WriteLine(s)
	
	
