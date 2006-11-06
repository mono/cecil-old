2006-11-01
Mono CIL Merge is tool to create one assembly from many.
It can be used to merge results of mono linker

To use:

merge [options] -out result_file primary assemly [files]
   --about     About the Mono CIL Merge
   --version   Print the version number of the Mono CIL Merge
   -out        Specify the output directory, default to .

   Sample: merge -out output.exe input.exe input_lib.dll
