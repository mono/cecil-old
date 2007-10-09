frameworkdir=$(pkglibdir)
framework_SCRIPTS = ../bin/Gendarme.Framework.dll
EXTRA_DIST = $(framework_sources) $(framework_sources_in) framework.mdp Gendarme.Framework.csproj gendarme-framework.pc.in
CLEANFILES = $(framework_SCRIPTS) $(framework_SCRIPTS).mdb
DISTCLEANFILES = ../AssemblyInfo.cs Makefile.in gendarme-framework.pc

pkgconfig_in_files = gendarme-framework.pc.in

pkgconfigdir= $(prefix)/lib/pkgconfig
pkgconfig_DATA = $(pkgconfig_in_files:.pc.in=.pc)

framework_sources_in = ../AssemblyInfo.cs.in
framework_generated_sources = $(framework_sources_in:.in=)
framework_sources = AssemblyManager.cs IRule.cs RuleCollection.cs \
		    RuleInformation.cs  RuleInformationManager.cs \
		    Rules.cs Runner.cs Violation.cs Violations.cs \
		    Message.cs Location.cs MinimalRunner.cs \
		    MessageCollection.cs MessageType.cs

framework_build_sources = $(addprefix $(srcdir)/, $(framework_sources))
framework_build_sources += $(framework_generated_sources)

../bin/:
	mkdir ../bin

../bin/Gendarme.Framework.dll: ../bin/ $(framework_build_sources)
	$(MCS) -debug -target:library -pkg:mono-cecil -r:System.Xml.dll -out:$@ $(framework_build_sources)

self-test: $(framework_SCRIPTS)
	mono --debug ../bin/gendarme.exe $(framework_SCRIPTS)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;
