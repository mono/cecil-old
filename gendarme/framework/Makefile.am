frameworkdir=$(pkglibdir)
framework_SCRIPTS = Gendarme.Framework.dll
EXTRA_DIST = $(framework_sources) $(framework_sources_in)
CLEANFILES = Gendarme.Framework.dll
DISTCLEANFILES = ../AssemblyInfo.cs Makefile.in

framework_sources_in = ../AssemblyInfo.cs.in
framework_generated_sources = $(framework_sources_in:.in=)
framework_sources = AssemblyManager.cs IRule.cs RuleCollection.cs \
		    RuleInformation.cs  RuleInformationManager.cs \
		    Rules.cs Runner.cs Violation.cs Violations.cs \
		    Message.cs Location.cs MinimalRunner.cs

framework_build_sources = $(addprefix $(srcdir)/, $(framework_sources))
framework_build_sources += $(framework_generated_sources)

Gendarme.Framework.dll: $(framework_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:System.Xml.dll -out:$@ $(framework_build_sources)

self-test: Gendarme.Framework.dll
	gendarme Gendarme.Framework.dll

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;
