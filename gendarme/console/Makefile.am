gendarmedir=$(pkglibdir)
gendarme_SCRIPTS = gendarme.exe
EXTRA_DIST = $(gendarme_sources) $(gendarme_sources_in)
CLEANFILES = gendarme.exe
DISTCLEANFILES = Makefile.in

gendarme_sources_in = ../AssemblyInfo.cs.in
gendarme_generated_sources = $(gendarme_sources_in:.in=)
gendarme_sources = ConsoleRunner.cs

gendarme_build_sources = $(addprefix $(srcdir)/, $(gendarme_sources))
gendarme_build_sources += $(gendarme_generated_sources)

gendarme.exe: $(gendarme_build_sources)
	$(MCS) -pkg:mono-cecil -r:../framework/Gendarme.Framework.dll -r:System.Xml.dll -out:$@ $(gendarme_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

