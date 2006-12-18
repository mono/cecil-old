2006-12-18

Linker for the gcc4cil toolchain, heavily based on the merging tool by
Alex Prudiky.

The linker is not finished yet, the source is here so that those who are
interested can have a look at it and give suggestions...

TODO:

- Finish code generation for p/invokes.
- General rename-refactoring (change "merge" into "link"...), which means
  renaming some files and directories as well.
- Clean up Monodevelop project files, and check them in.
- Check makefiles in as well.
- Integrate with the *latest* gcc4cil.
- Testing...
- Also implement access to native global data.
