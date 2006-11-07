SUBDIRS=Test

ui_rulesdir=$(pkglibdir)
ui_rules_SCRIPTS = Gendarme.Rules.Ui.dll
EXTRA_DIST = $(ui_rules_sources) $(ui_rules_sources_in) Gendarme.Rules.Ui.xml \
	Gendarme.Rules.Ui.mdp Gendarme.Rules.Ui.csproj
CLEANFILES = Gendarme.Rules.Ui.dll Test.Rules.Ui.dll
DISTCLEANFILES = Makefile.in Gendarme.Rules.Ui.xml TestResult.xml

ui_rules_sources_in = ../../AssemblyInfo.cs.in
ui_rules_generated_sources = $(ui_rules_sources_in:.in=)
ui_rules_sources = ExecutableTargetRule.cs GtkSharpExecutableTargetRule.cs \
	SystemWindowsFormsExecutableTargetRule.cs

ui_rules_build_sources = $(addprefix $(srcdir)/, $(ui_rules_sources))
ui_rules_build_sources += $(ui_rules_generated_sources)

Gendarme.Rules.Ui.dll: $(ui_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -r:../../framework/Gendarme.Framework.dll -out:$@ $(ui_rules_build_sources)

# Install Unstable Mono Libraries (see configure.ac)

install-data-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Ui.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		$(INSTALL) -c -m 0755 $$ASM $(DESTDIR)$(pkglibdir); \
	done;

uninstall-hook:
	$(INSTALL) -c -m 0644 Gendarme.Rules.Ui.xml $(DESTDIR)$(pkglibdir);
	for ASM in $(INSTALLED_ASSEMBLIES); do \
		rm -f $(DESTDIR)$(pkglibdir)/`basename $$ASM`; \
	done;

ui_test_sources = ExecutableTargetTest.cs GtkSharpExecutableTargetTest.cs \
	SystemWindowsFormsExecutableTargetTest.cs

ui_test_build_sources = $(addprefix $(srcdir)/Test/, $(ui_test_sources))

Test.Rules.Ui.dll: $(ui_rules_build_sources)
	$(MCS) -target:library -pkg:mono-cecil -pkg:mono-nunit -r:../../framework/Gendarme.Framework.dll \
		-r:Gendarme.Rules.Ui.dll -out:$@ $(ui_test_build_sources)

test: Gendarme.Rules.Ui.dll Test.Rules.Ui.dll

run-test: Gendarme.Rules.Ui.dll Test.Rules.Ui.dll
	MONO_PATH=../../framework/:$(CECIL_PATH):$(MONO_PATH) nunit-console Test.Rules.Ui.dll

self-test: test
	gendarme Gendarme.Rules.Ui.dll
